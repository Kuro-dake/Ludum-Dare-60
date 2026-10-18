using System;
using System.Collections;
using System.Collections.Generic;
using IEnumRunner.Transitions;
using Unity.VisualScripting;
using UnityEngine;

public class Game : MonoBehaviour
{

    [SerializeField] private Vector3 offscreen, onscreen;
    [SerializeField]
    private GameEvent game_event_prefab;

    public static GameEvent GameEventFactory()
    {
        return Instantiate(inst.game_event_prefab);
        
    }

    private static Game _game;
    public static Game inst => _game == null ? _game = FindFirstObjectByType<Game>() : _game; 
    
    private Transform _player_chip_transform;
    Transform player_chip_transform => _player_chip_transform == null ? _player_chip_transform = BaseScene.Find("PlayerChip") :_player_chip_transform;
        
    private PlayerChip _player_chip;
    private PlayerChip player_chip => _player_chip != null ? _player_chip : _player_chip = player_chip_transform.GetComponent<PlayerChip>();
    public bool running { get; protected set; }

    void Start()
    {
        transform.localPosition = offscreen;
    }

    public enum EventType
    {
        avoid,
        collect, 
        spacebar,
        wait
        
    }
    
    public class GameEventData
    {
        
        public Vector2Int origin { get; protected set; }
        public Vector2Int direction { get; protected set; }

        public EventType type { get; protected set; }
        public float modifier { get; protected set; }

        public float windup, duration;
        
        public GameEventData(string data)
        {

            if (data[0] == 'w')
            {
                modifier = float.Parse(data.Substring(1, 1)) / 10f;
                type = EventType.wait;
                return;
            }
            if (data[0] == 's')
            {
                type = EventType.spacebar;
                return;
            }
            
            int column = 0;
            try
            {
                column = int.Parse(data.Substring(1, 1));
                if (column > 3 || column < 1)
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                throw new Exception($"Useless fukin event pos data '{data}'");
            }

            column -= 2;
            switch (data[0])
            {
                case 'u':
                    origin = new Vector2Int(column, 2);
                    direction = Vector2Int.down;
                    
                    break;
                case 'd':
                    origin = new Vector2Int(column, -2);
                    direction = Vector2Int.up;

                    break;
                case 'l':
                    origin = new Vector2Int(-2, column);
                    direction = Vector2Int.right;

                    break;
                case 'r':
                    origin = new Vector2Int(2, column);
                    direction = Vector2Int.left;

                    break;
                
                case '1':
                case '2':
                case '3':
                    origin = new Vector2Int(int.Parse(data.Substring(0,1)) - 2, column);
                    direction = Vector2Int.zero;
                    break;
                
                default:
                    throw new Exception($"Unknown identifier {data[0]}");
                    
                
            }

            switch (data[2])
            {
                case 'c':
                    type = EventType.collect;
                    break;
                case 'a':
                    type = EventType.avoid;
                    break;
                default:
                    throw new Exception($"Unknown event type {data[2]}");

            }
            
        }
    }

    public Action on_collect { get; protected set; } 
    public Action on_hit { get; protected set; }
    public Action on_loose { get; protected set; }
    public IEnumerator StartGame(List<string> events, float windup, float duration, Action on_collect, Action on_hit, Action on_loose)
    {
        this.on_collect = on_collect;
        this.on_hit = on_hit;
        this.on_loose = on_loose;
        running = true;
        yield return (
            Make.The(gameObject).In(.5f).MoveTo(onscreen + transform.parent.position)
            +
            Make.The(player_chip_transform).In(.5f).MoveTo(onscreen + transform.parent.position)
            ).Execute();

        
        foreach (var es in events)
        {
            GameEventData e = new(es);
            e.windup = windup;
            e.duration = duration;
            switch (e.type)
            {
                case EventType.spacebar:
                    PressSpace.waiting_for_space = true;
                    while (PressSpace.waiting_for_space)
                    {
                        yield return null;
                    }

                    break;
                case EventType.wait:
                    yield return new WaitForSeconds(e.modifier);
                    break;
                
                case EventType.avoid:
                case EventType.collect:
                    StartCoroutine(GameEventFactory().LifeCycle(e));
                    break;
            }
        }

        running = false;

        yield return StopGame();
    }
    
    public IEnumerator StopGame()
    {
        
        yield return (Make.The(player_chip_transform).In(.5f).MoveTo(offscreen + transform.parent.position)
            +
            Make.The(gameObject).In(.5f).MoveTo(offscreen + transform.parent.position)
            ).Execute();
        
    }

    // public IEnumerator WaitUntilGameFinished()
    // {
    //     while (running)
    //     {
    //         yield return null;
    //     }
    // }

    private Vector2Int _pos;
    public Vector2Int pos
    {
        get => _pos;
        protected set
        {
            _pos = new Vector2Int(Mathf.Clamp(value.x, -1, 1), Mathf.Clamp(value.y, -1, 1));
        }
    }

    private Dictionary<KeyCode, Vector2Int> directions = new()
    {
        { KeyCode.A, Vector2Int.left },
        { KeyCode.D, Vector2Int.right },
        { KeyCode.W, Vector2Int.up },
        { KeyCode.S, Vector2Int.down },
    };

    private Coroutine movement_routine = null;
    
    void DigestNextDir()
    {
        if (movement_routine != null)
        {
            return;
        }

        if (next_dir != null)
        {
            movement_routine = StartCoroutine(FollowDirection(next_dir.Value));
            next_dir = null;
        }
        
    }

    IEnumerator FollowDirection(Vector2Int next_dir)
    {
        yield return player_chip.FollowDirection(next_dir);
        movement_routine = null;
    }
    [SerializeField]
    float _chip_transition_duration = .5f, _grid_dim = .86f;

    public float grid_dim => _grid_dim;
    public float chip_transition_duration => _chip_transition_duration;
    
    
    
    private Vector2Int? _next_dir;

    private Vector2Int? next_dir
    {
        get => _next_dir;
        set{
            _next_dir = value;
            player_chip.SetNextDir(value);
        }
    }
    
    void Update()
    {
        if (running)
        {
            foreach(var kv in directions)
            {
                if (Input.GetKeyDown(kv.Key))
                {
                    next_dir = kv.Value;
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                next_dir = null;
            }            
        }
        
        DigestNextDir();
    }

     
}

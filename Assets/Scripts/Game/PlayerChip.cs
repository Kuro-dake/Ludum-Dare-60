using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using IEnumRunner.Transitions;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = IEnumRunner.Sequence;
using Vector2 = UnityEngine.Vector2;

public class PlayerChip : MonoBehaviour
{
    private Vector2Int _pos;
    private Game g => transform.GetComponentInParent<Game>();
    public Vector2Int pos
    {
        get => _pos;
        protected set
        {
            _pos = clampPos(value);
        }
    }

    Vector2Int clampPos(Vector2Int value)
    {
        return new Vector2Int(Mathf.Clamp(value.x, -1, 1), Mathf.Clamp(value.y, -1, 1));
    }
    public IEnumerator FollowDirection(Vector2Int where)
    {
        if (clampPos(pos + where) == pos)
        {
            SC.sounds.PlayResource("fluke");
            yield break;
        }

        float t = 0f;
        pos = where + pos;
        Vector2 target = (Vector2)pos * g.grid_dim + (Vector2)g.transform.position;
        
        yield return Make.The(transform).In(g.chip_transition_duration).MoveTo(target).Execute();
        

    }

    private Dictionary<Vector2Int?, int> rotations = new()
    {
        [Vector2Int.up] = 0,
        [Vector2Int.left] = 1,
        
        [Vector2Int.down] = 2,
        [Vector2Int.right] = 3,
        
    };

    private Transform dir_indicator => BaseScene.Find("directionMarker");
    private Vector2Int? currently_playing_dir = Vector2Int.zero;
    private Sequence currently_playing_dir_anim;

    public void SetNextDir(Vector2Int? dir)
    {
        if (dir == currently_playing_dir)
        {
            return;
        }
        
        currently_playing_dir_anim?.Stop();
        if (dir == null)
        {
            // currently_playing_dir_anim = Make.The(dir_indicator).In(.2f).ScaleTo(0f).Happen();
            
        }
        else
        {
            if(!rotations.ContainsKey(dir))
            {
                throw new Exception($"Fucking vector2Int {dir} for grid directions? Shove it up your arse.");
            }
            currently_playing_dir_anim = (
                Make.The(dir_indicator).instantly.MoveTo(g.grid_dim * (Vector2)clampPos(pos + dir.Value) + (Vector2)g.transform.position)
                &
                Make.The(dir_indicator).In(.2f).ScaleTo(.04708f)).Run();
   
        }

        if (dir == Vector2Int.zero)
        {
            throw new Exception("Srsly wtf zero? Who sends zero?");
        }
      
        
    }

    void Hit()
    {
        
    }

    void Collected()
    {
        
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        GameEvent gevent = other.GetComponentInParent<GameEvent>();
        if (gevent == null || !gevent.alive)
        {
            return;
        }
        
        gevent.Collided();
        if (gevent.game_event.type == Game.EventType.avoid)
        {
            Hit();
            Game.inst.on_hit?.Invoke();
        }
        else
        {
            Collected();
            Game.inst.on_collect?.Invoke();
        }
        
        
    }
}

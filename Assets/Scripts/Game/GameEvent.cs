using System;
using System.Collections;
using System.Collections.Generic;
using IEnumRunner;
using IEnumRunner.Transitions;
using UnityEngine;


public class GameEvent : MonoBehaviour
{

    public Game.GameEventData game_event { get; protected set; }
    public IEnumerator LifeCycle(Game.GameEventData game_event)
    {
        yield return null;
        this.game_event = game_event;

        SetColor();
        SetPosition();
        
        
        
        // signal intent
        yield return Signal();


        // move/age
        yield return Act();

        // cleanup
        yield return Cleanup();
    }

    private Vector3 orig_scale;
    private void Start()
    {
        orig_scale = transform.localScale;
    }

    void SetColor()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = game_event.type == Game.EventType.avoid ? Color.red : Color.green;
            Make.The(sr).instantly.AlphaTo(0f).Happen();

        }
    }

    void SetPosition()
    {
        Game g = Game.inst;
        transform.SetParent(g.transform);
        transform.localPosition = (Vector2)game_event.origin * g.grid_dim;
    }
    
    private Dictionary<Vector2Int?, int> rotations = new()
    {
        [Vector2Int.up] = 0,
        [Vector2Int.left] = 1,
        
        [Vector2Int.down] = 2,
        [Vector2Int.right] = 3,
        
    };

    private bool windup_counted = false;
    bool isWindupRunning() => !windup_counted;
    IEnumerator CountdownWindup()
    {
        windup_counted = false;
        yield return new WaitForSeconds(game_event.windup  + 5f);
        windup_counted = true;
    } 
    
    IEnumerator Signal()
    {
        // if (game_event.direction == Vector2Int.zero)
        // {
        Make.The(transform.Find("directionMarker"))
            .instantly.ScaleTo(0f)
            .Happen();
        // }
        // else
        // {
        //     Make.The(transform.Find("directionMarker"))
        //         .instantly.RotateTo(rotations[game_event.direction] * 90f)
        //         .Happen();            
        // }
        StartCoroutine(CountdownWindup());

        float windup = game_event.windup;

        bool blink_on = false;
        Make._Batch blink = Make.The(transform.Find("sprite"));
        float windup_blink_step = .1f;
        while (windup > windup_blink_step)
        {
            blink_on = !blink_on;
            blink = blink.instantly.AlphaTo(blink_on ? .5f : 0f).ThenWait(windup_blink_step).then;
            windup -= windup_blink_step;
        }
        
        yield return blink.Execute();
        yield return new WaitForSeconds(windup);
        
        // foreach (var sr in transform.GetComponentsInChildren<SpriteRenderer>())
        // {
        //     // already initialized in setColor
        //     Make.The(sr).In(.1f).AlphaTo(.5f).Happen();
        // }

        // yield return (
        //     Make.The(transform.Find("sprite")).In(game_event.windup).ScaleTo(0f)
        //         &    
        //     Make.The(transform.Find("directionMarker")).In(game_event.windup).AlphaTo(0f)
        //     ).Execute();
        
        Make.The(transform.Find("directionMarker")).instantly.AlphaTo(0f).Happen();
        Make.The(transform.Find("sprite")).instantly.AlphaTo(1f).ScaleTo(1f).Happen();
        
        
        yield return null;
    }

    public bool alive {
        get; protected set;
} = false;

    private Coroutine act_sequence;
    
    IEnumerator Act()
    {
        Game g = Game.inst;
        yield return Make.The(transform.Find("sprite")).In(.1f).ScaleTo(1.33f).Execute();

        alive = true;
        act_sequence = StartCoroutine(Make.The(transform).In(game_event.duration)
            .MoveTo(transform.position + new Vector3(game_event.direction.x, game_event.direction.y) * g.grid_dim * 4).Execute());
        yield return act_sequence;
        
        alive = false;
    }

    IEnumerator Cleanup()
    {
        yield return Make.The(transform).In(.1f).ScaleTo(0f).Execute();
        
        Destroy(gameObject);
    }

    public void Collided()
    {
        alive = false;
        if (act_sequence != null)
        {
            StopCoroutine(act_sequence);
        }

        StartCoroutine(Cleanup());
    } 
    
}

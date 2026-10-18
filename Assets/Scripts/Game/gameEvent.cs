using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameEvent : MonoBehaviour
{

    Game.GameEvent game_event;
    public IEnumerator LifeCycle(Game.GameEvent game_event)
    {
        yield return null;
        this.game_event = game_event;

        // signal intent

        // move/age

        // cleanup
    }

    void SetColor()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.color = game_event.type == Game.EventType.avoid ? Color.red : Color.green;
        }
    }
    
    IEnumerator Signal()
    {
        yield return null;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using IEnumRunner;
using IEnumRunner.Transitions;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class BaseScene: MonoBehaviour
{
    [SerializeField]
    public BaseScene next_scene; 
    public ScriptLine SCRLA(Func<IEnumerator> action) => new(new ScriptLine.LineAction(action));
    public ScriptLine SCRLA(Action action) => new(new ScriptLine.LineAction(action));
    // double combos
    public ScriptLine SCRLA(Func<IEnumerator> sugar_action, Func<IEnumerator> rabbit_action) => new(new ScriptLine.LineAction(sugar_action), new ScriptLine.LineAction(rabbit_action));
    public ScriptLine SCRLA(Action sugar_action, Action rabbit_action) => new(new ScriptLine.LineAction(sugar_action), new ScriptLine.LineAction(rabbit_action));
    public ScriptLine SCRLA(Action sugar_action, Func<IEnumerator> rabbit_action) => new(new ScriptLine.LineAction(sugar_action), new ScriptLine.LineAction(rabbit_action));
    public ScriptLine SCRLA(Func<IEnumerator> sugar_action, Action rabbit_action) => new(new ScriptLine.LineAction(sugar_action), new ScriptLine.LineAction(rabbit_action));

    public static Transform Find(string objname)
    {
        GameObject go = GameObject.Find(objname);
        if (go == null)
        {
            throw new Exception($"Object {objname} doesn't exist");
        }

        return go.transform;
    }    
    public static T Find<T>(string objname) => Find(objname).GetComponent<T>();

    // depends on objects having a child sprite renderer you can call/manipulate
    public static SpriteRenderer FindSR(string objname)
    {
        var go = GameObject.Find(objname);
        var sr = go.GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
        {
            sr = go.GetComponent<SpriteRenderer>();
        }

        if (sr == null)
        {
            throw new Exception($"{go}: by name of {objname} - SpriteRenderer not found");
        }

        return sr;
    } 
    
    protected Director director;
    public virtual string scene_name => GetType().ToString();
    protected virtual List<ScriptLine> GetScriptLines()
    {
        throw new NotImplementedException();
    }
    
    // What? It's a fukin gamejam. I need every fukin second
    public Transform SBC(string name) => SpeechBubblePosition(name);
    public Transform SpeechBubblePosition(string name)
    {
      var parent = GameObject.Find(name).transform;
      return parent.Find("bubblePos");  
    }

    protected ScriptLine break_line = null;
    
    public IEnumerable<ScriptLine> lines
    {
        get
        {
            if (director == null)
            {
                throw new Exception("A scene has to be initialized by a director");
            }
            
            foreach (var line in GetScriptLines())
            {
                if (break_line != null)
                {
                    yield return break_line;
                    yield break;
                }
                yield return line;
            }
        }
    }

    public void Initialize(Director d) => director = d;

    // public abstract void Initialize(Director director);

    protected ScriptLine RabbitDoorGoIn()
    {
        return SCRLA(() => RabbitDoorGoInStep());
    }
    protected ScriptLine RabbitDoorComeOut()
    {
        return SCRLA(() => RabbitDoorComeOutStep());
    }
    protected IEnumerator RabbitDoorGoInStep()
    {
        FindSR("Rabbit").flipX = false;
        yield return new WaitForSeconds(.3f);
        
        Make.The(FindSR("Rabbit")).In(Director.rabbit_in_out_duration).AlphaTo(0f).Happen();
        yield return Walk(Find("Rabbit"), Find("Rabbit").transform.position + Vector3.right, Director.rabbit_in_out_duration);    
        
         FindSR("Rabbit").flipX = true;
            // close door sound
         SC.sounds.PlayResource("door_shut", 2f);
         FindSR("Door").flipX = false;
        
    }
    
    protected IEnumerator RabbitDoorComeOutStep()
    {
        SC.sounds.PlayResource("door_open", 2f);
        FindSR("Door").flipX = true;
            
        // rabbit come out
        Make.The(FindSR("Rabbit")).In(Director.rabbit_in_out_duration).AlphaTo(1f).Happen();
        yield return Walk(Find("Rabbit"), Find("Rabbit").transform.position + Vector3.left, Director.rabbit_in_out_duration);
    }

    public static IEnumerator Walk(Transform actor, Vector2 where, float duration)
    {
        actor.GetComponentInChildren<Waddler>().blobber = false;
        yield return Make.The(actor).In(duration).MoveTo(where).Execute();
        actor.GetComponentInChildren<Waddler>().blobber = true;
    }

    protected virtual Dictionary<string, bool> visibilities => new()
    {
        
    };
    
    protected virtual Dictionary<string, Vector3> positions => new()
    {
        
    };

    protected virtual Dictionary<string, Vector3> scales => new()
        { };

    public void SetupScene()
    {
        foreach (var v in visibilities)
        {
            Make.The(FindSR(v.Key)).instantly.AlphaTo(v.Value ? 1f : 0f).Happen();
        }
        foreach (var v in positions)
        {
            Find(v.Key).localPosition = v.Value;
        }       
        foreach (var v in scales)
        {
            Find(v.Key).localScale = v.Value;
        }
    }

}

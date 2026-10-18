using System;
using System.Collections;
using System.Collections.Generic;
using IEnumRunner;
using IEnumRunner.Transitions;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This shit is supposed to be the aggregated script step for both sides. We're expecting many actions to be same in the
/// parallel/alternating scripts, but at least transformations will be different for futurevision on/off
///
/// There might also be different dialogue, but who has the time, right?
/// </summary>

public class ScriptLine : Dictionary<ScriptLine.LineActor, ScriptLine.LineAction>
{

    public ScriptLine(LineAction action)
    {
        this[LineActor.sugarboy] = action;  
        this[LineActor.rabbit] = action;
    }
    public ScriptLine(LineAction sugarboy_action, LineAction rabbit_action)
    {
        this[LineActor.sugarboy] = sugarboy_action;
        this[LineActor.rabbit] = rabbit_action;
    }

    public override string ToString()
    {
        return this[LineActor.rabbit] == this[LineActor.sugarboy]
            ? $"Same action: {this[LineActor.rabbit]}"
            : $"Sugarboy action: {this[LineActor.sugarboy]}\nRabbit action: {this[LineActor.rabbit]}";
    }
    
    // ugly yet elegant, and hopefully works
    // just shit whatever you need - a single or a double - into the queue
    //
    // e.g. a string : it will make it into a speech line for the last speaker
    // string -> Speech via operator -> ScriptLine via operator
    
    public static implicit operator ScriptLine(Make._Batch batch) => new(batch);
    public static implicit operator ScriptLine(Sequence seq) => new(seq);
    public static implicit operator ScriptLine(Speech speech) => new(speech);
    
    public static implicit operator ScriptLine(string line) => (Speech)line; 
    public static implicit operator ScriptLine((string rabbit_line, string sugarboy_line) lines) => 
        new (new Speech(lines.rabbit_line), new Speech(lines.sugarboy_line)); 
    public static implicit operator ScriptLine(float delay) => new (delay); 
    
    public static implicit operator ScriptLine((string text, Transform origin) line) => new Speech(line.text, line.origin);
    public static implicit operator ScriptLine((string text, Transform origin, float delay) line) => new Speech(line.text, line.origin, line.delay);
    public static implicit operator ScriptLine(Func<IEnumerator> action)=> new (action);
    public static implicit operator ScriptLine(Action action)=> new (action);


    public class LineAction {
        protected Make._Batch batch;
        protected Sequence sequence;
        protected Speech speech;
        protected Func<IEnumerator> action;
        protected Action action_plain;
        protected float? delay;
        
        public LineAction(Make._Batch batch) => this.batch = batch;
        public LineAction(Sequence sequence) => this.sequence = sequence;
        public LineAction(Speech speech) => this.speech = speech;
        public LineAction(Func<IEnumerator> action) => this.action = action;
        public LineAction(Action action) => this.action_plain = action;
        public LineAction(float delay) => this.delay = delay;

        public static implicit operator LineAction(Make._Batch batch)=> new (batch);
        public static implicit operator LineAction(Sequence sequence)=> new (sequence);
        public static implicit operator LineAction(Speech speech)=> new (speech);
        public static implicit operator LineAction(Func<IEnumerator> action)=> new (action);
        public static implicit operator LineAction(float delay)=> new (delay);
        public static implicit operator LineAction(Action action)=> new (action);

        private IEnumerator _to_execute;

        public IEnumerator to_execute
        {
            get
            {
                if (_to_execute == null)
                {
                    throw new Exception("The line action was never initialized!");
                }
                return _to_execute;
            }
            protected set
            {
                if (_to_execute != null)
                {
                    throw new Exception("Multiple actions set. This should never happen!");
                }

                _to_execute = value;
            }
        }

        IEnumerator WFS(float delay)
        {
            yield return new WaitForSeconds(delay);
        }

        IEnumerator PlainAction()
        {
            action_plain();
            yield break;
        }
        
        public void Initialize()
        {
            if (batch != null)
            {
                to_execute = batch.Execute();
            }
            else if (speech != null) 
            {
                to_execute = speech.Trigger();
            }
            else if (action != null)
            {
                to_execute = action();
            } 
            else if (sequence != null)
            {
                to_execute = sequence.Execute();
            } 
            else if (action_plain != null)
            {
                to_execute = PlainAction();
            }
            else if (delay != null)
            {
                to_execute = WFS(delay.Value);
            }
            else
            {
                throw new Exception("This action has ... no action! How could that happen?");
            }
            
        }
        
        public override string ToString()
        {
            string ret = ""; 
            if (batch != null)
            {
                ret = $"Batch of {batch.length} actions";
            }
            else if (speech != null) 
            {
                ret = $"Speech: {speech.text}";
            }
            else if (action != null)
            {
                ret = $"Action: {action}";
            }
            else if (sequence != null)
            {
                ret = $"Sequence: {sequence}";
            } 
            else if (action_plain != null)
            {
                ret = $"Plain action: {action}";
            }
            else if (delay != null)
            {
                ret = $"Delay of {delay} s";
            }
            else
            {
                ret = "No action.";
            }

            return $"Line Action({ret})";
        }
        
    }
    
    // This is the distinctor between futurevision on and off. I can not work with a bool that means
    // the futurevision on/off for the purposes of a fukin gamejam - I'm thinking about this in a certain way,
    // and I need to write the code in that exact manner
    [System.Serializable]
    public enum LineActor
    {
        sugarboy,
        rabbit
    }
}

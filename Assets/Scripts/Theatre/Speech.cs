using System;
using System.Collections;
using UnityEngine;

public class Speech
{
    // 
    public float delay { get; private set; }
    public string text { get; private set; }
    public Transform origin;
    public float write_speed;
    public float max_scale = 1f;
    
    public Speech(string text, Transform origin = null, float delay = -1f, float write_speed = 1f, float max_scale = 1f)
    {
        this.text = text;
        this.origin = origin != null ? origin : Director.last_speaker;
        this.delay = delay;
        this.write_speed = write_speed;
        this.max_scale = max_scale;
    }

    public static implicit operator Speech(string text) => new (text);
    public static implicit operator Speech((string text, Transform origin) line) => new (line.text, line.origin);
    public static implicit operator Speech((string text, Transform origin, float delay) line) => new (line.text, line.origin, line.delay);

    public IEnumerator Trigger()
    {
        if (origin)
        {
            Director.last_speaker = origin;
        }
        yield return SpeechBubble.Talk(text, Director.last_speaker, delay, write_speed, max_scale);

    }
    
}

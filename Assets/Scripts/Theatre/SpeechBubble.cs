using System;
using System.Collections;
using System.Collections.Generic;
using IEnumRunner.Transitions;
using TMPro;
using UnityEngine;

public class SpeechBubble : MonoBehaviour
{
    private static SpeechBubble _inst;

    [SerializeField] private float letter_delay, apper_disappear_duration;
    [SerializeField] private Vector2 offset;
    public static SpeechBubble instd
    {
        get
        {
            if (_inst == null)
            {
                _inst = FindFirstObjectByType<SpeechBubble>();
            }

            return _inst;
        }
    }

    private TextMeshProUGUI _tmpro;
    private TextMeshProUGUI tmpro => GetComponentInChildren<TextMeshProUGUI>();

    private string displayed
    {
        get => tmpro.text;
        set => tmpro.text = value;
    }

    private void Start()
    {
        transform.localScale = new Vector3(0f, 0f, 1f);
    }

    public static IEnumerator Talk(string text, Transform bubble_pos_pointer, float delay, float write_speed = 1f, float max_scale = 1f, float pitch =1f)
    {
        var instGO = GameObject.Find("SpeechBubble");
        if (bubble_pos_pointer.parent.name == "Narrator")
        {
            instGO = GameObject.Find("NarratorSpeech");
        }
        else if (text.Substring(0, 3) == "LB|")
        {
            instGO = GameObject.Find("SpeechBubbleLeft");
            text = text.Substring(3);
            
        }
        else if (text.Substring(0, 3) == "RB|")
        {
            instGO = GameObject.Find("SpeechBubbleRight");
            text = text.Substring(3);
            
        }

        var inst = instGO.GetComponent<SpeechBubble>();
        return inst._Talk(text, bubble_pos_pointer, delay, write_speed, max_scale);  
    }  
    [SerializeField]
    private float default_pitch = 1f, talksound_delay = .17f;

    [SerializeField] private FloatRange talksound_start = new FloatRange();
    public IEnumerator _Talk(string text, Transform bubble_pos_pointer, float delay, float write_speed = 1f, float max_scale = 1f, float? pitch = null)
    {
        text = text.Replace("future", "future™️");
        displayed = "";    
        yield return Make.The(gameObject).instantly.ScaleTo(0f).Execute();
        GetComponent<RectTransform>().position = bubble_pos_pointer.position + (Vector3)offset;
           
        yield return Make.The(gameObject).In(apper_disappear_duration).ScaleTo(max_scale).Execute();
        
        // if we wanted the typed effect, but fuggit 
        
        Queue<char> txt_list = new(text);
        float speed_inverse = 1f / write_speed;

        float current_talksound_delay = 0f;
        bool talksound_paused = false;
        IEnumerator PlayTalkSound()
        {
            float pvalue = pitch.HasValue ? pitch.Value : default_pitch;
            AudioSource talksound = SC.sounds.PlayResource("talk", 1f, new FloatRange(pvalue * .95f, pvalue * 1.05f));

            while (true)
            {
                talksound.time = talksound_start;
                if (!talksound_paused)
                {
                    talksound.Play();
                }
                
                bool yielded = false;
                while (current_talksound_delay < talksound_delay)
                {
                    if (!talksound_paused)
                    {
                        current_talksound_delay += Time.deltaTime * (write_speed > 1f ? 1f : write_speed);                        
                    }
                    yield return null;
                    yielded = true;
                }

                if (!yielded)
                {
                    yield return null;

                }

                current_talksound_delay = 0f;
            }
            
        }

        Coroutine talking_routine = StartCoroutine(PlayTalkSound());
        
        while (txt_list.Count > 0)
        {
            char next = txt_list.Dequeue();
            if (next == '|')
            {
                talksound_paused = true;
                yield return new WaitForSeconds(.1f);
            }
            else
            {
                talksound_paused = false;
                displayed += next;
                yield return new WaitForSeconds(letter_delay * speed_inverse);
                
            }
            
        }
        StopCoroutine(talking_routine);
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }
        else
        {
            // blocker, press space to continue
            PressSpace.waiting_for_space = true;
            while (PressSpace.waiting_for_space)
            {
                yield return null;
            }

            yield return null;
        }

        yield return Make.The(gameObject).In(apper_disappear_duration).ScaleTo(0f).Execute();

    }

}

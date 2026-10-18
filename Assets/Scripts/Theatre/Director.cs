using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using IEnumRunner.Transitions;
using UnityEngine;

[System.Serializable]
public class NamedFloat : Pair<string, float>
{
    
    public NamedFloat(string s, float f): base(s,f){}
}


public class Director : MonoBehaviour
{
    public static float rabbit_in_out_duration => named_params["rabbit_in_out_duration"];
    [SerializeField] private ScriptLine.LineActor current_vision;

    public static ScriptLine.LineActor vision
    {
        get { return inst.current_vision; }
        set { inst.current_vision = value; }
    }

    [System.Serializable]
    public class NParam
    {
        [SerializeField]
        private List<NamedFloat> _named_params;

        public float this[string s] => _named_params.Find(nf => nf.first == s).second;
    }
    [SerializeField]
    protected NParam _named_params = new();

    public static NParam named_params 
    {
        get
        {
            if(inst == null)
            {
                inst = FindFirstObjectByType<Director>();
            }

            return inst._named_params;            
        }
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (SC.sounds.already_playing_music)
            {
                SC.sounds.StopMusic(0.1f);
            }
            else
            {
                SC.sounds.PlayMusic("music", .3f);
            }
        }
    }

    static Director inst;
    
    [SerializeField]
    private SugarBoy _sugar_boy;
    [SerializeField]
    private Rabbit _rabbit;

    [SerializeField] private List<BaseScene> scenes = new ();
    
    public SugarBoy sugar_boy => _sugar_boy;
    public Rabbit rabbit => _rabbit;

    private static Transform _last_speaker;

    public static Transform last_speaker
    {
        get
        {
            if (_last_speaker == null)
            {
                Debug.LogWarning("Last speaker was not set. Was this intentional?");
            }

            return _last_speaker;
        }
        set => _last_speaker = value;
        
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SC.Initialize();
        start_scene.SetupScene();
        SC.sounds.PlayMusic("music", .3f);
        StartCoroutine(Direct(start_scene));
        StartCoroutine(WaddleTest());
        
    }

    [SerializeField] private Game _game;
    public static Game game
    {
        get
        {
            if(inst == null)
            {
                inst = FindFirstObjectByType<Director>();
            }

            return inst._game;
        }
    }

    [SerializeField]
    private float waddle_service_bounce_divider = 20f;
    private IEnumerator WaddleTest()
    {
        yield return null;
        bool bounce_only = false;
        while (true)
        {
            SC.waddler.Waddle(false, bounce_only);
            bounce_only = !bounce_only;
            yield return new WaitForSeconds(SC.waddler.bounce_speed * .5f / waddle_service_bounce_divider);
        }   
    }
    [SerializeField] private BaseScene start_scene;
    [SerializeField] private BaseScene current_scene;
    IEnumerator Direct(BaseScene s)
    {
        s.Initialize(this);
        foreach (var l in s.lines)
        {
            var action = l[current_vision];
            action.Initialize();
            yield return action.to_execute;
        }

        if (s.next_scene != null)
        {
            yield return Direct(s.next_scene);
        }
    }

}

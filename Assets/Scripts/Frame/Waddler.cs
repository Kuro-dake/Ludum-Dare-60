using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IEnumRunner.Transitions;
using UnityEngine.Serialization;

public class Waddler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        orig_scale = transform.localScale;
        SC.waddler.OnWaddleStart += OnWaddleStart;
        SC.waddler.OnWaddleStep += OnWaddleStep;
        SC.waddler.OnWaddleFinished += OnWaddleFinished;
    }
    [SerializeField]
    float bounce_height, bounce_x_distance, tilt_rotation_max;
    
    
    Vector3 orig_scale;
    // Update is called once per frame
    void Update()
    {
        
    }
    Coroutine finish_routine;
    
    bool left = false, skip;
    public bool inversed { get; protected set; } = false;
    public bool stopped { get; set; } = false;
    
    public void Inverse() => inversed = true;
    [SerializeField]
    Vector3 waddle_scale = new Vector3(.95f, 1.05f);
    
    [SerializeField]
    float sin_t_multiplier = 1f;

    [FormerlySerializedAs("bounce_duration_multiplier")] [SerializeField] private float duration_multiplier = .6f;
    
    void OnWaddleStart(object sender, WaddleManager.OnWaddleStepArgs args)
    {
        if (stopped)
        {
            return;
        }
        float bounce_duration = 1f / SC.waddler.bounce_speed;

        if (!args.bounce_only)
        {
            Make.The(gameObject).In(bounce_duration * duration_multiplier).FixedSinTTransition(sin_t_multiplier).ScaleTo(Common.MultiplyVectorBy(orig_scale, waddle_scale))
                //.then.In(bounce_duration * .4f).ScaleTo(Common.MultiplyVectorBy(orig_scale, new Vector2(y, x))).FixedSinTTransition(multiplier)
                //.then.In(bounce_duration * .2f).ScaleTo(Common.MultiplyVectorBy(orig_scale, new Vector2(x, y))).FixedSinTTransition(multiplier)
                .then.In(bounce_duration * duration_multiplier).FixedSinTTransition(sin_t_multiplier).ScaleTo(Common.MultiplyVectorBy(orig_scale, new Vector2(1f, 1f)))
                .Happen();            
        }
        

        orig_rotation = transform.localRotation;
        if (finish_routine != null)
        {
            StopCoroutine(finish_routine);
        }
        left = inversed ? !args.left : args.left;
        // if(!controls_active && blobber && blobbers_active)
        // {
        //     //GetComponentInParent<MakeTransitions>().Trigger("blob");
        //     blobbing = true;
        // }
    }

    public static bool blobbers_active = true;
    
    bool blobbing = false;
    [SerializeField]
    public bool blobber = false;
    bool controls_active => true;
    Quaternion orig_rotation;
    public float jump_height_multiplier = 5f;
    [SerializeField] private float sin_t_jump_mulitplier = 1f;
    void OnWaddleStep(object sender, WaddleManager.OnWaddleStepArgs args)
    {
        if (blobber || stopped)
        {
            return;
        }
        RectTransform rt = GetComponent<RectTransform>();
        Vector2 l_pos = rt != null ? rt.anchoredPosition : (Vector2)transform.localPosition;

        float orig_x = l_pos.x;
        float target_x = bounce_x_distance * (left ? 1 : -1);
        float target_rotation = tilt_rotation_max * (left ? -1 : 1);

        float bheight = blobbing ? 0f : bounce_height;
        float tx = blobbing ? 0f : target_x;
        float tr = blobbing ? target_rotation * .5f : target_rotation;
        
        l_pos = rt != null ? rt.anchoredPosition : (Vector2)transform.localPosition; ;
        l_pos.y = args.sin_t_half * bheight * (jumping ? jump_height_multiplier: 1f);
        l_pos.x = Mathf.Lerp(orig_x, tx, args.sin_t_half);

        transform.localRotation = Quaternion.Lerp(orig_rotation, Quaternion.Euler(Vector3.forward * tr), args.sin_t_half);

        if(rt != null)
        {
            rt.anchoredPosition = l_pos;
        }
        else
        {
            transform.localPosition = l_pos;
        }
        
    }

    public bool jumping = false;
    void OnWaddleFinished(object sender, object args)
    {
        blobbing = false;
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        if (finish_routine != null)
        {
            StopCoroutine(finish_routine);
        }

        if (stopped)
        {
            return;
        }
        finish_routine = StartCoroutine(OnWaddleFinishedStep());
    }

    [SerializeField] private float finish_speed = 2f;
    IEnumerator OnWaddleFinishedStep()
    {
        float t = 0f;
        orig_rotation = transform.localRotation;
        Vector3 orig_pos = transform.localPosition;
        while (t < 1f)
        {
            t += Time.deltaTime * finish_speed;
            transform.localRotation = Quaternion.Lerp(orig_rotation, Quaternion.identity, t);
            transform.localPosition = Vector3.MoveTowards(orig_pos, Vector3.zero, t);
            yield return null;
        }
        finish_routine = null;
    }
}

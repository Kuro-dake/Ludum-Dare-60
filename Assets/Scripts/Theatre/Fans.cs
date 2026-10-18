using System.Collections;
using System.Collections.Generic;
using IEnumRunner.Transitions;
using UnityEngine;


public class Fans : MonoBehaviour
{
    private Dictionary<string, Vector3> orig_poses = new();
    private Dictionary<string, Vector3> orig_scales = new();

    private Dictionary<string, Vector3> scatter_vectors = new()
    {
        { "Fan1", Vector2.up },
        { "Fan2", new Vector2(-2f, -1f).normalized },
        { "Fan3", new Vector2(2f, -1f).normalized },
        
    };

    public void TransparentFans()
    {
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            Make.The(sr).instantly.AlphaTo(0).Happen();
        }
    }
    
    void Start()
    {
        foreach(var t in transform.GetChildren())
        {
            orig_poses[t.name] = t.transform.localPosition;
            orig_scales[t.name] = t.transform.localScale;
        }
    }

    void ResetFans()
    {
        foreach (var t in transform.GetChildren())
        {
            t.transform.localPosition = orig_poses[t.name];
            t.transform.localScale = orig_scales[t.name];
        }
    }
    [SerializeField]
    float appearance_duration = .5f;
    public IEnumerator Appear(Vector2? pos = null)
    {
        ResetFans();
        if (pos != null)
        {
            transform.position = pos.Value;
        }
        
        foreach (var t in transform.GetChildren())
        {
            Make.The(t).instantly.MoveBy(scatter_vectors[t.name]).Happen();
            StartCoroutine(BaseScene.Walk(t, t.transform.position - scatter_vectors[t.name], appearance_duration));

        }
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            Make.The(sr).In(appearance_duration).AlphaTo(1f).Happen();
        }

        yield return new WaitForSeconds(appearance_duration);
    }

    private List<Coroutine> fan_shakes = new();

    public void StopShakeFans()
    {
        foreach (var v in fan_shakes)
        {
            StopCoroutine(v);
        }
        fan_shakes.Clear();
    }
    
    public void StartShakeFans()
    {
        StopShakeFans();

        foreach (var f in transform.GetChildren())
        {
            fan_shakes.Add(StartCoroutine(ShakeFan(f)));
        }
    }
    
    IEnumerator ShakeFan(Transform fan)
    {
        while (true)
        {
            Vector2 target_pos = orig_poses[fan.name] + transform.position;
            target_pos += Random.insideUnitCircle * Director.named_params["fan_seethe_radius"];
            bool yielded = false;
            while (Vector2.Distance(fan.position, target_pos) > .01f)
            {
                fan.position = Vector2.MoveTowards(fan.position, target_pos,
                    Time.deltaTime * Director.named_params["fan_seethe_speed"]);
                yield return null;
                yielded = true;
            }

            if (!yielded)
            {
                yield return null;                
            }
        }
    }
    
    
    
    
    
    public IEnumerator Scatter(float force = 1f, float speed = 1f, float rotate = 0f)
    {
        float mod_ad = appearance_duration / speed;
        int dir = 1;
        foreach (var t in transform.GetChildren())
        {
            float rotate_ = Random.Range(.5f, 1f) * 720f * rotate;
            dir = dir == 1 ? -1 : 1;
            (Make.The(t).In(mod_ad).MoveBy(scatter_vectors[t.name] * force).RotateBy(rotate_ * dir).then.instantly.RotateTo(0f)).Happen();
        }
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            Make.The(sr).In(mod_ad).AlphaTo(0f).Happen();
        }
        
        yield return new WaitForSeconds(mod_ad);
    }
    
    
    
}

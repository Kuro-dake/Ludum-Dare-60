using IEnumRunner;
using IEnumRunner.Transitions;
using UnityEngine;

public class altf4 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private Sequence s;
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            s?.Stop();
            s = Make.The(transform).In(.1f).MoveTo(new Vector3(98f, -52f)).then.In(3f).MoveTo(new Vector3(-160f, 70f))
                .Happen();
        }
    }
}

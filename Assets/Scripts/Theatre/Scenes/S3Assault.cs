using System;
using System.Collections;
using System.Collections.Generic;
using IEnumRunner.Transitions;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

[System.Serializable]
public class S3Assault : BaseScene
{
    
    protected override List<ScriptLine> GetScriptLines()    
    {

        
        return new()
        {
           
           0.1f,
           ("Adapt or get left behind!", SBC("Sugarboy")),
           SCRLA(()=>
               StartCoroutine(Walk(Find("Sugarboy"), Find("Door").transform.position + new Vector3(-.75f, -1f), 1.5f))),
           .3f,
           SCRLA(()=>
               StartCoroutine(Find<Fans>("Fans").Scatter())),
           (
               Make.The(Find("Main Camera")).In(1.5f).MoveTo(new Vector3(4.48f, -1.07f, -10f))
               &
               Make.The(FindSR("OutsideGrass")).In(.5f).AlphaTo(0f)
               & 
               Make.The(Find("ShitPile")).In(.5f).MoveBy(Vector3.up * 11f).ScaleTo(0.1f)
               &
               Make.The(Find("subShitPile")).In(.5f).ScaleTo(0.1f)
               & 
               Make.The(Find("FuturePile")).In(.5f).MoveBy(Vector3.up * 11f).ScaleTo(0.1f)
               
               ),
           
           
           
            
            
        };
    }
    
    protected override Dictionary<string, bool> visibilities => new()
    {
        ["Rabbit"] = false,
    };
    
    protected override Dictionary<string, Vector3> positions => new()
    {
        ["Main Camera"] = new Vector3(0f,0f,-10f),
        ["Sugarboy"] = new Vector3(-4.04f,-0.31f,0f),
        ["Rabbit"] = new Vector3(4.7f,-.13f,0f),
        ["OutsideCave"] = new Vector3(4.41f,1.03f,0f),
        ["OutsideGrass"] = new Vector3(-4.29f,0.22f,0f),
        ["Door"] = new Vector3(4.83f,0.87f,0f),
        ["Fans"] = new Vector3(-6.64f,-.57f)

    };
}

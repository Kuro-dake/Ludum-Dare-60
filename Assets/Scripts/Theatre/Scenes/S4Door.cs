using System;
using System.Collections;
using System.Collections.Generic;
using IEnumRunner.Transitions;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

[System.Serializable]
public class S4Door : BaseScene
{
    
    protected override List<ScriptLine> GetScriptLines()    
    {
        IEnumerator WaitForSpacebar()
        {
            while (!Input.GetKeyDown(KeyCode.Space))
            {
                yield return null;
            }
        }
        return new()
        {
           
           0.1f,
           ("LB|Open the door rabbit!", SBC("Sugarboy")),
           new Speech("RB|Never!", SBC("Door"), max_scale:.6f),
           SCRLA(()=>StartCoroutine(Director.game.StartGame(new()))),
           Make.The(Find("TutorialGroup")).In(.35f).MoveBy(Vector3.up * 340f),
           SCRLA(()=> WaitForSpacebar()),
           Make.The(Find("TutorialGroup")).In(.3f).CGAlphaTo(0f).MoveBy(Vector3.up * 340f),
           
           
           SCRLA(()=>Director.game.WaitUntilGameFinished()),
           
           
           SCRLA(()=>Director.game.StopGame()),
           SCRLA(()=>
               StartCoroutine(Walk(Find("Sugarboy"), Find("Door").transform.position + new Vector3(-.75f, -1f), 1.5f))),
           .3f,
           new Speech("RB|Honey, take the children and go! There's a madman outside!", SBC("Door"), max_scale:.6f),
           new Speech("RB|*Unintelligible speech in background*", SBC("Door"), .5f, max_scale:.3f),
           new ScriptLine(new Speech("RB|I don't know. He keeps talking about future, but I suspect he just wants to touch the kids.", SBC("Door"), max_scale:.6f),
               new Speech("RB|I don't know. He keeps talking about future, but he just took a giant shit outside.", SBC("Door"), max_scale:.6f)
               ),
           new ScriptLine( new ScriptLine.LineAction(()=>{}),new Speech("RB|Also he looks like he wants to touch the kids.", SBC("Door"), max_scale:.6f)
           ),
           new Speech("LB|I am the future! I am salvation!", SBC("Sugarboy")),
           
           
            
            
        };
    }
    
    protected override Dictionary<string, bool> visibilities => new()
    {
        ["Fan1"] = false,
        ["Fan2"] = false,
        ["Fan3"] = false,
        ["Rabbit"] = false,

        ["OutsideGrass"] = false,

    };
    
    protected override Dictionary<string, Vector3> positions => new()
    {
        ["Main Camera"] = new Vector3(4.48f,-1.07f,-10f),
        ["Sugarboy"] = new Vector3(4.08f,-0.13f,0f),
        ["Fan1"] = new Vector3(0f,-.4f,0f),
        ["Fan2"] = new Vector3(-.73f,-1.03f,0f),
        ["Fan3"] = new Vector3(0.68f,-1f,0f),
        ["Rabbit"] = new Vector3(4.7f,-.13f,0f),
        ["OutsideCave"] = new Vector3(4.41f,1.03f,0f),
        ["OutsideGrass"] = new Vector3(-4.29f,0.22f,0f),
        ["Door"] = new Vector3(4.83f,0.87f,0f)
    };
}

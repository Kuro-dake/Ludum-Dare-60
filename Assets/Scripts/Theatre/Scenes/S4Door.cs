using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            PressSpace.waiting_for_space = true;
            while (PressSpace.waiting_for_space)
            {
                yield return null;
            }
        }

        List<string> level = new()
        {
            "u1a",
            "w9",
            "u2a",
            "w9",
            "u3a",
            "w9",
            "d1c",
            "w9",
            "d2c",
            "w9",
            "d3c",
            "w9",
            "l1a",
            "w9",
            "l2a",
            "w9",
            "l3a",
            "w9",
            "r1c",
            "w9",
            "r2c",
            "w9",
            "r3c",
            "w9",
            "11c",
            "w9",
            "12c",
            "w9",
            "13c",
            "w9",
            "21c",
            "w9",
            "22c",
            "w9",
            "23c",
            "w9",
            "31c",
            "w9",
            "32c",
            "w9",
            "33c",
            "w9",
            "w9",
            "w9",
            "w9",
            "w9",
            "w9",
            
        };

        if (Director.vision == ScriptLine.LineActor.sugarboy)
        {
            level.Insert(0,"s");
        }

        void OnLoose()
        {
            break_line = new Speech("I'm a retard", SBC("Sugarboy"));
        }

        void OnCollect()
        {
            Make.The(Find("Sugarboy")).In(.1f).MoveBy(Vector3.right * .5f).then.MakeHappen(
                (() => SC.sounds.PlayResource("hit_door"))
            ).then.In(.1f).MoveBy(Vector3.left *.5f).Happen();
        }

        Queue<string> hitlines = new(new List<string>()
        {
            "S|LB|Ouch",
            "S|LB|Dammit",
            "R|RB|Is the door winning?",
            "S|LB|I am the salvation!",
            "S|LB|I cannot fail!",
            "R|RB|Is he still at it out there?",
        });
        
        
        void OnHit()
        {
            Make.The(Find("Sugarboy")).In(.1f).MoveBy(Vector3.right *.5f).then.MakeHappen(
                (() =>
                {
                    SC.sounds.PlayResource("hit_self");
                    string txt = hitlines.Count > 0 ? hitlines.Dequeue() : "R|RB|Please go away...";
                    string talker = txt[0] == 'S' ? "Sugarboy" : "Door";
                    txt = txt.Substring(2);
                    StartCoroutine(SpeechBubble.Talk(txt, SBC(talker), .5f, 1f, .6f));
                })
            ).then.In(.1f).MoveBy(Vector3.left *.5f).Happen();
        }
        
        return new()
        {
           
           0.1f,
           // ("LB|Open the door rabbit!", SBC("Sugarboy")),
           // new Speech("RB|Never!", SBC("Door"), max_scale:.6f),
           SCRLA(() =>
           {
               PressSpace.waiting_for_space = true;
               Make.The(Find("TutorialGroup")).In(.35f).MoveBy(Vector3.up * 340f)
                   .ThenWaitUntil(() => !PressSpace.waiting_for_space).then
                   .In(.3f).CGAlphaTo(0f).MoveBy(Vector3.up * 340f).Happen();
           }),
           SCRLA(()=>Director.game.StartGame(level, 2f, 2f, OnCollect, OnHit, OnLoose)),
           

           // SCRLA(()=>Director.game.WaitUntilGameFinished()),
           
           
           // SCRLA(()=>Director.game.StopGame()),
           // SCRLA(()=>
           //     StartCoroutine(Walk(Find("Sugarboy"), Find("Door").transform.position + new Vector3(-.75f, -1f), 1.5f))),
           .3f,
           new Speech("RB|Honey, take the children and run through the back door!", SBC("Door"), max_scale:.6f),
           new Speech("RB|There's a dangerous pervert outside and he's trying to break in!", SBC("Door"), max_scale:.6f),
           new Speech("RB|*Unintelligible speech in background*", SBC("Door"),  max_scale:.3f),
           new Speech("RB|I don't know. He keeps talking about future!", SBC("Door"), max_scale:.6f),

           new ScriptLine(new Speech("RB|But I suspect he's only here because he wants to touch our kids.", SBC("Door"), max_scale:.6f),
               new Speech("RB|And he just took a giant shit outside.", SBC("Door"), max_scale:.6f)
               ),
           new ScriptLine( new ScriptLine.LineAction(()=>{}),new Speech("RB|Also he looks like he's only here because he wants to touch our kids.", SBC("Door"), max_scale:.6f)
           ),
           new Speech("LB|The future is inevitable!", SBC("Sugarboy")),
           
           
            
            
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

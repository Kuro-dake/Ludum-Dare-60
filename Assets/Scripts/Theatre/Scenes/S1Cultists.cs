using System.Collections;
using System.Collections.Generic;
using IEnumRunner.Transitions;
using UnityEngine;
[System.Serializable]
public class S1Cultists : BaseScene
{
    
    protected override List<ScriptLine> GetScriptLines()    
    {

        return new()
        {
            
            ("rabbit!", SBC("Narrator")),
            
            // ding dong?
            
            Make.The(FindSR("Door")).In(.5f).AlphaTo(1f),
            "raaaabbit...",
            Make.The(FindSR("OutsideCave")).In(.5f).AlphaTo(1f),
            
            // ding dong?
            
            "Hey rabbit!",
            
            new Speech("oh my God...", SBC("Door"), -1f, 1f, .6f),
            Make.The(Find("Rabbit")).instantly.MoveBy(Vector2.right),
            RabbitDoorComeOut(),
            
            ("WHAT?", SBC("Rabbit")),
            SCRLA(() =>
            {
                Make.The(FindSR("OutsideGrass")).In(.5f).AlphaTo(1f).Happen();
                return Find<Fans>("Fans").Appear();
            
            }),
            
            
            ("the FUTUrE is here!", SBC("Fan2")),
            ("What...?", SBC("Rabbit")),
                        
            ("Don't you understand? The future is here!", SBC("Fan3")),
            ("Okay?", SBC("Rabbit")),
            ("What do you mean 'Okay?'?", SBC("Fan1")),
            ("You ignorant fool! It's the future!", SBC("Fan2")),
            ("The future is here!", SBC("Fan1")),
            ("We have arrived at the future!", SBC("Fan3")),
            ("Good for you️.", SBC("Rabbit")),
            ("Now|||.|||.|||.|||. |||||||||||| I'm going back inside.", SBC("Rabbit")),
            new Speech("Byyyeeeee.", null, .5f, 0.3f),
            
            /// rabbit turns around and goes inside
            0.3f,
            
            RabbitDoorGoIn()

            
            
            
        };
    }
    
    protected override Dictionary<string, bool> visibilities => new()
    {
        ["Sugarboy"] = false,
        ["Fan1"] = false,
        ["Fan2"] = false,
        ["Fan3"] = false,
        ["Rabbit"] = false,
        ["OutsideCave"] = false,
        ["OutsideGrass"] = false,
        ["Door"] = false
    };
    
    protected override Dictionary<string, Vector3> positions => new()
    {
        ["Main Camera"] = new Vector3(0f,0f,-10f),
        ["Sugarboy"] = new Vector3(-4.04f,-0.31f,0f),
        ["Fan1"] = new Vector3(0f,-.4f,0f),
        ["Fan2"] = new Vector3(-.73f,-1.03f,0f),
        ["Fan3"] = new Vector3(0.68f,-1f,0f),
        ["Rabbit"] = new Vector3(4.7f,-.13f,0f),
        ["OutsideCave"] = new Vector3(4.41f,1.03f,0f),
        ["OutsideGrass"] = new Vector3(-4.29f,0.22f,0f),
        ["Door"] = new Vector3(4.83f,0.87f,0f)
    };
}

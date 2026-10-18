using System;
using System.Collections;
using System.Collections.Generic;
using IEnumRunner.Transitions;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

[System.Serializable]
public class S5CompoOutro : BaseScene
{
    
    protected override List<ScriptLine> GetScriptLines()    
    {
        SetupScene();

        IEnumerator Freeze()
        {
            while (true)
            {
                yield return null;
            }
        }
        
        Speech N(string text)=> (text, SBC("Narrator"));

        if (Director.vision == ScriptLine.LineActor.sugarboy)
        {
            Director.vision = ScriptLine.LineActor.rabbit;
            return new()
            {
           
                0.1f,
                // ("LB|Open the door rabbit!", SBC("Sugarboy")),
                // new Speech("RB|Never!", SBC("Door"), max_scale:.6f),
                N("Okay, so: this obviously isn't the whole scope of what I had in mind."),
                N("I haven't done this in two and half years, \nso I kinda forgot how to manage time during compo."),
                N("It occured to me that starting to implement a reusable system for \"theatrical plays\" is a very good idea during compo."),
                N("And it actually kind of was... When else would I get to it? "),
                
                N("The minigame was supposed to decide the progression, but obviously affects almost nothing in the current state."),
                
                N("Anyway..."),
                N("the intended core feature was 'the removal of futurevision'."),
                N("Futurevision is that dumb helmet that sugarboy and the future fans wear."),
                N("Turns out you've been forced to wear one as well."),
                N("Now let's remove it, and see the things for what they truly are!"),
                2f            
            
            };            
        }
        else {
            return new()
            {
           
                0.1f,
                // ("LB|Open the door rabbit!", SBC("Sugarboy")),
                // new Speech("RB|Never!", SBC("Door"), max_scale:.6f),
                N("So that's it. That's the compo version."),
                N("Thanks for \"playing\", and if you found this vulgar stupid simple little thing as hilarious as I did, come back later. There might be an update or two."),
                N("Cheers and cYa. Made by kuro@dizztal for LD60 compo. Press ESC if you want to exit."),
                SCRLA(()=>Freeze()),
                    
            
            };            
        }
        
        
    }
    
    protected override Dictionary<string, bool> visibilities => new()
    {
        ["Fan1"] = false,
        ["Fan2"] = false,
        ["Fan3"] = false,
        ["Rabbit"] = false,

        ["OutsideGrass"] = false,
        ["OutsideCave"] = false,
        ["Door"] = false,
        ["Sugarboy"] = false,

    };
    
    protected override Dictionary<string, Vector3> positions => new()
    {
        ["Main Camera"] = new Vector3(0f,0f,-10f),
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

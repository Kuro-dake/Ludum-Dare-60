using System;
using System.Collections;
using System.Collections.Generic;
using IEnumRunner.Transitions;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

[System.Serializable]
public class S2SugarEntry : BaseScene
{
    
    protected override List<ScriptLine> GetScriptLines()    
    {


        IEnumerator HaltParticleGen()
        {
            ParticleSystem ps = Find<ParticleSystem>("ShitPile");
            ParticleSystem sps = Find<ParticleSystem>("subShitPile");
            ParticleSystem.MainModule mm = ps.main;
            ParticleSystem.MainModule smm = sps.main;
            
            while (mm.simulationSpeed > 0f)
            {
                mm.simulationSpeed = Mathf.MoveTowards(mm.simulationSpeed, 0f, Time.deltaTime * 5f);
                smm.simulationSpeed = mm.simulationSpeed;
                yield return null;
            }

            mm.simulationSpeed = smm.simulationSpeed = 0.0001f;
        }
        
        
        return new()
        {
           1f,
            new Speech("...", SBC("Narrator"), 2f, 0.025f),
            .5f,
           SCRLA(Find<Fans>("Fans").StartShakeFans),
           new Speech("Ignorant fool!", SBC("Fan2"), .3f, 10f),
           new Speech("How dare you not shit yourself from the mention of the future!", SBC("Fan1"),  write_speed: 10f),
           new Speech("Pencil pusher!", SBC("Fan3"), .3f, 10f),
           new Speech("you Technophobe!", SBC("Fan1"),.3f, 10f),
           new Speech("you reactionary!", SBC("Fan3"),.3f, 10f),
           new Speech("Anti-progress!", SBC("Fan1"), .3f, 10f),
           new Speech("reeeeeeeeee!", SBC("Fan2"), .3f, 10f),
           
           
           
           0.1f,
           SCRLA(()=>SC.sounds.PlayResource("sugarbeam")),
           // init sugar boy -> landing animation & fan scatter
           (Make.The(Find("Sugarboy")).instantly.ScaleTo(new Vector3(1.1f, 23f, 1f))
               &
               Make.The(FindSR("Sugarboy")).instantly.AlphaTo(0f)
               +
               Make.The(Find("Sugarboy")).In(.2f).ScaleTo(Vector3.one)
               &
               Make.The(FindSR("Sugarboy")).In(.2f).AlphaTo(1f))
            ,
           SCRLA(Find<Fans>("Fans").StopShakeFans),
           SCRLA(()=>StartCoroutine(Find<Fans>("Fans").Scatter(9f, 1f, 1f))),
           SCRLA(()=>SC.sounds.PlayResource("sugarland")),

           SCRLA(()=>Camera.main.GetComponent<ShakeScreen>().Shake("boom")),
           .5f,
           new Speech("What was that?", SBC("Door"), -1f, 1f, .6f),
           .1f,
           RabbitDoorComeOut(),
           .1f,
           ("What the fuck?", SBC("Rabbit")),
           
           SCRLA(()=>Find<Fans>("Fans").Appear(new Vector3(-6.64f, -.57f))),
           .5f,
           ("It is I!", SBC("Sugarboy")),
           new Speech("It is him!", SBC("Fan2"), 0.01f, 0.2f, .6f),
           new Speech("Shut up dude...", SBC("Fan3"), .5f, 2f, .6f),
           ("Sugarboy, the inventor of the future!", SBC("Sugarboy")),
           ("Now Behold! the future!", SBC("Sugarboy")),
           1f,
           SCRLA(
               () => SC.sounds.PlayResource("future_charge", 1f, new FloatRange(.25f)),
               () => SC.sounds.PlayResource("shitbeam", 1f, new FloatRange(1f))
               
           ),
           SCRLA(() =>
           {
               
               Find<Shaker>("Sugarboy").enabled = true;
               Find<Shaker>("Sugarboy").GetComponentInChildren<Waddler>().stopped = true;
           }),
           1f,
           SCRLA(()=>Find<ParticleSystem>("FutureBeam").Play(), ()=>Find<ParticleSystem>("ShitBeam").Play()),
           .3f,
           SCRLA(()=>Find<ParticleSystem>("FuturePile").Play(), ()=>Find<ParticleSystem>("ShitPile").Play()),
           // here go the fan's stupid comments
           new Speech("Wooooooweeeee it's happening!", SBC("Fan3"), .5f, 2f, .6f),
           new Speech("Oh my god we're living in the future!", SBC("Fan2"), .5f, 2f, .6f),
           new Speech("I just jizzed in my pants!", SBC("Fan1"), .5f, 2f, .6f),
           .3f,
           SCRLA(()=>Find<ParticleSystem>("FutureBeam").Stop(), ()=>Find<ParticleSystem>("ShitBeam").Stop()),
           SCRLA(()=>{}, ()=>HaltParticleGen()),
           .3f,
           SCRLA(() =>
           {
               Find<Shaker>("Sugarboy").StopShake();
               Find<Shaker>("Sugarboy").GetComponentInChildren<Waddler>().stopped = false;
           }),
           1f,
           
           new ScriptLine(new ScriptLine.LineAction(new Speech("Is that||.||.||.||", SBC("Rabbit"), .2f, .4f)), 
               new ScriptLine.LineAction(new Speech("Is that shit?!", SBC("Rabbit")))),
           
           new Speech("It's the future!", SBC("Sugarboy")),
           
           new ScriptLine(new ScriptLine.LineAction(new Speech("Well, the volume is very impressive for certain...", SBC("Rabbit"), .2f, .4f)), 
               new ScriptLine.LineAction(new Speech("... I'm pretty sure it's shit. Although that volume is certainly impressive...\"", SBC("Rabbit")))),
           
           ("You beheld the power of the future.", SBC("Sugarboy")),
           ("Now your outdated hole is getting modernized by the future!", SBC("Sugarboy")),
           
           ("What? No!", SBC("Rabbit") ),
           ("Yes! For you see, the future is inevitable!", SBC("Sugarboy") ),
           ("Ah hell no!", SBC("Rabbit") ),
           

           RabbitDoorGoIn(),
           new Speech("Honey! Call the cops!", SBC("Rabbit"),  max_scale:.6f ),

           
           
           
           // the pile of future
           
                
           
           
           
            
           
           
            
            
        };
        
        
    }
    protected override Dictionary<string, bool> visibilities => new()
    {
        ["Sugarboy"] = false,
        ["Rabbit"] = false,
        
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
        ["Door"] = new Vector3(4.83f,0.87f,0f),
    };
}

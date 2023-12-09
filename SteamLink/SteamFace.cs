using Elements.Core;
using ReSounding;

namespace Impressive;

public class SteamFace
{
    public float CheekPuffSuckL => CheekPuffL - CheekSuckL;
    public float CheekPuffSuckR => CheekPuffR - CheekSuckR;
    public float3 JawPos => new(JawRight - JawLeft, -JawDown, JawForward);


    // Cheeks
    [OSCMap("/sl/xrfb/facew/CheekPuffL")]
    public float CheekPuffL;

    [OSCMap("/sl/xrfb/facew/CheekSuckL")]
    public float CheekSuckL;

    [OSCMap("/sl/xrfb/facew/CheekPuffR")]
    public float CheekPuffR;

    [OSCMap("/sl/xrfb/facew/CheekSuckR")]
    public float CheekSuckR;



    // Jaw
    [OSCMap("/sl/xrfb/facew/JawSidewaysLeft")]
    public float JawLeft;

    [OSCMap("/sl/xrfb/facew/JawSidewaysRight")]
    public float JawRight;

    [OSCMap("/sl/xrfb/facew/JawDrop")]
    public float JawDown;

    [OSCMap("/sl/xrfb/facew/JawThrust")]
    public float JawForward;


    // Lips
    [OSCMap("/sl/xrfb/facew/LipsToward")]
    public float LipsToward; // Whether the lips are closed when the jaw is down

    [OSCMap("/sl/xrfb/facew/LipPuckerL")]
    public float LipPuckerL;

    [OSCMap("/sl/xrfb/facew/LipPuckerR")]
    public float LipPuckerR;
}
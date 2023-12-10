using Elements.Core;
using ReSounding;

namespace Impressive;

public class SteamFace
{
    public float CheekPuffSuckL => CheekPuffL - CheekSuckL;
    public float CheekPuffSuckR => CheekPuffR - CheekSuckR;
    public float3 JawPos => new(JawRight - JawLeft, -JawDown, JawForward);
    
    public float SmileFrownLeft => SmileL - FrownL;
    public float SmileFrownRight => SmileR - FrownR;

    public float LipBottomOverturn => (LipFunnelBottomL + LipFunnelBottomR) * 0.5f;
    public float LipTopOverturn => (LipFunnelTopL + LipFunnelTopR) * 0.5f;

    public float LipSuckTop => (LipSuckTopL + LipSuckTopR) * 0.5f;
    public float LipSuckbottom => (LipSuckBottomL + LipSuckBottomR) * 0.5f;


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


    // Mouth Left/Right
    [OSCMap("/sl/xrfb/facew/MouthLeft")]
    public float MouthLeft;

    [OSCMap("/sl/xrfb/facew/MouthRight")]
    public float MouthRight;



    // Lips
    [OSCMap("/sl/xrfb/facew/LipsToward")]
    public float LipsToward; // Whether the lips are closed when the jaw is down

    [OSCMap("/sl/xrfb/facew/LipPuckerL")]
    public float LipPuckerL;

    [OSCMap("/sl/xrfb/facew/LipPuckerR")]
    public float LipPuckerR;

    [OSCMap("/sl/xrfb/facew/LowerLipDepressorL")]
    public float LowerLeftLip;

    [OSCMap("/sl/xrfb/facew/LowerLipDepressorR")]
    public float LowerRightLip;

    [OSCMap("/sl/xrfb/facew/UpperLipRaiserL")]
    public float RaiseLeftLip;

    [OSCMap("/sl/xrfb/facew/UpperLipRaiserR")]
    public float RaiseRightLip;

    [OSCMap("/sl/xrfb/facew/LipFunnelerLB")]
    public float LipFunnelBottomL;

    [OSCMap("/sl/xrfb/facew/LipFunnelerRB")]
    public float LipFunnelBottomR;

    [OSCMap("/sl/xrfb/facew/LipFunnelerLT")]
    public float LipFunnelTopL;

    [OSCMap("/sl/xrfb/facew/LipFunnelerRT")]
    public float LipFunnelTopR;

    [OSCMap("/sl/xrfb/facew/LipSuckLB")]
    public float LipSuckBottomL;

    [OSCMap("/sl/xrfb/facew/LipSuckRB")]
    public float LipSuckBottomR;

    [OSCMap("/sl/xrfb/facew/LipSuckLT")]
    public float LipSuckTopL;

    [OSCMap("/sl/xrfb/facew/LipSuckRT")]
    public float LipSuckTopR;


    //Smiling/Frowning
    [OSCMap("/sl/xrfb/facew/LipCornerPullerL")]
    public float SmileL;

    [OSCMap("/sl/xrfb/facew/LipCornerPullerR")]
    public float SmileR;

    [OSCMap("/sl/xrfb/facew/LipCornerDepressorL")]
    public float FrownL;

    [OSCMap("/sl/xrfb/facew/LipCornerDepressorR")]
    public float FrownR;

}
using Elements.Core;
using ReSounding;

namespace Impressive;

public class SteamFace
{
    public float CheekPuffSuckL => CheekPuffL + CheekSuckL;
    public float CheekPuffSuckR => CheekPuffR + CheekSuckR;
    public float3 JawPos => new(JawLeft + JawRight, JawOpen, JawForward);


    [OSCMap("/sl/xrfb/facew/CheekPuffL")]
    public float CheekPuffL;

    [OSCMap("/sl/xrfb/facew/CheekSuckL")]
    public float CheekSuckL;

    [OSCMap("/sl/xrfb/facew/CheekPuffR")]
    public float CheekPuffR;

    [OSCMap("/sl/xrfb/facew/CheekSuckR")]
    public float CheekSuckR;


    [OSCMap("/sl/xrfb/facew/JawSidewaysLeft")]
    public float JawLeft;

    [OSCMap("/sl/xrfb/facew/JawSidewaysRight")]
    public float JawRight;

    [OSCMap("/sl/xrfb/facew/JawDrop")]
    public float JawOpen;

    [OSCMap("/sl/xrfb/facew/JawThrust")]
    public float JawForward;
}
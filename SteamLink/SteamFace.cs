using System.Runtime.InteropServices;
using Elements.Core;
using OSCMapper;

namespace Impressive;
public class SteamFace
{
    public SteamLinkEye EyeLeft = new();
    public SteamLinkEye EyeRight = new();
    public SteamLinkEye EyeCombined = new();

    #region Eyes

    #region EyesDir

    [OSCMap("/sl/eyeTrackedGazePoint")]
    public float3 LookDir { set => EyeCombined.EyeDirection = value; }

    // Center vec (whatever that means I guess)
    [OSCMap("/tracking/eye/CenterVecFull")]
    public float3 CenterVecFull;
    

    // Left eye direction
    [OSCMap("/avatar/parameters/LeftEyeX")]
    private float LeftEyeX { set => EyeLeft.DirX = value; }

    [OSCMap("/avatar/parameters/LeftEyeY")]
    private float LeftEyeY { set => EyeLeft.DirY = value; }

    

    // Right eye direction
    [OSCMap("/avatar/parameters/RightEyeX")]
    private float RightEyeX { set => EyeRight.DirX = value; }

    [OSCMap("/avatar/parameters/RightEyeY")]
    private float RightEyeY { set => EyeRight.DirY = value; }

    

    #endregion

    #region Eyelids

    // Right eyes
    [OSCMap("/avatar/parameters/RightEyeLid")]
    public float RightEyeLid { set => EyeRight.Eyelid = value; }

    [OSCMap("/avatar/parameters/RightEyeLidExpandedSqueeze")]
    public float RightEyeLidExpandedSqueeze { set => EyeRight.ExpandedSqueeze = value; }

    [OSCMap("/avatar/parameters/RightEyeSqueezeToggle")]
    public float RightEyeSqueezeToggle { set => EyeRight.SqueezeToggle = value; }

    [OSCMap("/avatar/parameters/RightEyeWidenToggle")]
    public float RightEyeWidenToggle { set => EyeRight.WidenToggle = value; }



    // Left eyes
    [OSCMap("/avatar/parameters/LeftEyeLid")]
    public float LeftEyeLid { set => EyeLeft.Eyelid = value; }

    [OSCMap("/avatar/parameters/LeftEyeLidExpandedSqueeze")]
    public float LeftEyeLidExpandedSqueeze { set => EyeLeft.ExpandedSqueeze = value; }

    [OSCMap("/avatar/parameters/LeftEyeSqueezeToggle")]
    public float LeftEyeSqueezeToggle { set => EyeLeft.SqueezeToggle = value; }

    [OSCMap("/avatar/parameters/LeftEyeWidenToggle")]
    public float LeftEyeWidenToggle { set => EyeLeft.WidenToggle = value; }



    [OSCMap("/tracking/eye/EyesClosedAmount")]
    public float EyesClosedAmount;

    #endregion

    #endregion
}

[StructLayout(LayoutKind.Sequential)]
public struct SteamLinkEye
{
    public float3 EyeDirection
    {

        get => new(DirX, DirY, 0f);
        set { DirX = value.x; DirY = value.y; }
    }

    public float DirX;
    public float DirY;

    public float Eyelid;
    public float ExpandedSqueeze;
    public float WidenToggle;
    public float SqueezeToggle;
}
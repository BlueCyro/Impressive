using System.Runtime.InteropServices;
using Elements.Core;
using ReSounding;

namespace Impressive;
public class SteamEyes
{
    public SteamLinkEye EyeLeft = new();
    public SteamLinkEye EyeRight = new();
    public SteamLinkEye EyeCombined => new()
    {
        Eyelid = MathX.Max(EyeLeft.Eyelid, EyeRight.Eyelid),
        EyeRotation = CombinedEyesDir
    };

    public floatQ CombinedEyesDir
    {
        get
        {
            if(EyeLeft.IsValid && EyeRight.IsValid && EyeLeft.IsTracking && EyeRight.IsTracking)
                _lastValidCombined = MathX.Slerp(EyeLeft.EyeRotation, EyeRight.EyeRotation, 0.5f);
            else if (EyeLeft.IsValid && EyeLeft.IsTracking)
                _lastValidCombined = EyeLeft.EyeRotation;
            else if (EyeRight.IsValid && EyeRight.IsTracking)
                _lastValidCombined = EyeRight.EyeRotation;

            return _lastValidCombined;
        }
    }

    private floatQ _lastValidCombined = floatQ.Identity;

    
    #region EyesDir


    // Left eye direction
    [OSCMap("/avatar/parameters/LeftEyeX")]
    public float LeftEyeX { set => EyeLeft.SetDirectionFromXY(X: value); }

    [OSCMap("/avatar/parameters/LeftEyeY")]
    public float LeftEyeY { set => EyeLeft.SetDirectionFromXY(Y: value); }

    

    // Right eye direction
    [OSCMap("/avatar/parameters/RightEyeX")]
    public float RightEyeX { set => EyeRight.SetDirectionFromXY(X: value); }

    [OSCMap("/avatar/parameters/RightEyeY")]
    public float RightEyeY { set => EyeRight.SetDirectionFromXY(Y: value); }

    

    #endregion

    #region Eyelids

    // Right eyes
    [OSCMap("/avatar/parameters/RightEyeLid")]
    public float RightEyeLid { set => EyeRight.Eyelid = 1f - MathX.Pow(value, 1.0f / 3.0f); }

    [OSCMap("/sl/xrfb/facew/UpperLidRaiserR")]
    public float RightEyeLidExpandedSqueeze { set => EyeRight.ExpandedSqueeze = value; }


    [OSCMap("/avatar/parameters/RightEyeSqueezeToggle")]
    public int RightEyeSqueezeToggle { set => EyeRight.SqueezeToggle = value; }

    [OSCMap("/avatar/parameters/RightEyeWidenToggle")]
    public int RightEyeWidenToggle { set => EyeRight.WidenToggle = value; }



    // Left eyes
    [OSCMap("/avatar/parameters/LeftEyeLid")]
    public float LeftEyeLid { set => EyeLeft.Eyelid = 1f - MathX.Pow(value, 1.0f / 3.0f); }

    [OSCMap("/sl/xrfb/facew/UpperLidRaiserL")]
    public float LeftEyeLidExpandedSqueeze { set => EyeLeft.ExpandedSqueeze = value; }


    [OSCMap("/avatar/parameters/LeftEyeSqueezeToggle")]
    public int LeftEyeSqueezeToggle { set => EyeLeft.SqueezeToggle = value; }

    [OSCMap("/avatar/parameters/LeftEyeWidenToggle")]
    public int LeftEyeWidenToggle { set => EyeLeft.WidenToggle = value; }


    #endregion
}

public struct SteamLinkEye
{
    private static readonly floatQ compensate = floatQ.Euler(new(0, -90, 0));

    public readonly bool IsTracking => IsValid && Eyelid > 0.1f;

    public readonly bool IsValid => EyeDirection.Magnitude > 0f && MathX.IsValid(EyeDirection);

    public float3 EyeDirection
    {
        readonly get => EyeRotation * float3.Forward;
        set => EyeRotation = floatQ.LookRotation(EyeDirection);
    }

    public floatQ EyeRotation;

    private float DirX;
    private float DirY;

    public float Eyelid;

    public float ExpandedSqueeze;
    public int WidenToggle;
    public int SqueezeToggle;

    public void SetDirectionFromXY(float? X = null, float? Y = null)
    {
        DirX = X ?? DirX;
        DirY = Y ?? DirY;

        // Get the angles out of the eye look
        float xAng = MathX.Asin(DirX);
        float yAng = MathX.Asin(DirY);

        // Convert to cartesian coordinates
        EyeRotation = floatQ.Euler(yAng * MathX.Rad2Deg, xAng * MathX.Rad2Deg, 0f);
    }
}
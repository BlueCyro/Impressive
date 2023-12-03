namespace Impressive;


using Elements.Core;
public class SteamFace : OSCMappedObject
{
    public float3 EyeLeft => new(LeftEyeX, LeftEyeY, 0f);
    public float3 EyeRight => new(RightEyeX, RightEyeY, 0f);

    #region Eyes

    #region EyesDir

    [OSCMap("/sl/eyeTrackedGazePoint")]
    public float3 LookDir;

    
    // Left eye direction
    [OSCMap("/avatar/parameters/LeftEyeX")]
    private float LeftEyeX;

    [OSCMap("/avatar/parameters/LeftEyeY")]
    private float LeftEyeY;

    
    // Right eye direction
    [OSCMap("/avatar/parameters/RightEyeX")]
    private float RightEyeX;

    [OSCMap("/avatar/parameters/RightEyeY")]
    private float RightEyeY;

    
    // Center vec (whatever that means I guess)
    [OSCMap("/tracking/eye/CenterVecFull")]
    public float3 CenterVecFull;

    #endregion

    #region Eyelids

    // Right eyes
    [OSCMap("/avatar/parameters/RightEyeLid")]
    public float RightEyeLid;

    [OSCMap("/avatar/parameters/RightEyeLidExpandedSqueeze")]
    public float RightEyeLidExpandedSqueeze;

    [OSCMap("/avatar/parameters/RightEyeSqueezeToggle")]
    public float RightEyeSqueezeToggle;

    [OSCMap("/avatar/parameters/RightEyeWidenToggle")]
    public float RightEyeWidenToggle;


    // Left eyes
    [OSCMap("/avatar/parameters/LeftEyeLid")]
    public float LeftEyeLid;

    [OSCMap("/avatar/parameters/LeftEyeLidExpandedSqueeze")]
    public float LeftEyeLidExpandedSqueeze;

    [OSCMap("/avatar/parameters/LeftEyeSqueezeToggle")]
    public float LeftEyeSqueezeToggle;

    [OSCMap("/avatar/parameters/LeftEyeWidenToggle")]
    public float LeftEyeWidenToggle;




    [OSCMap("/tracking/eye/EyesClosedAmount")]
    public float EyesClosedAmount;

    #endregion

    #endregion
}
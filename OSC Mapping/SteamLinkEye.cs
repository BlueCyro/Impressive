namespace Impressive;

using Elements.Core;
public class SteamLinkEye : OSCMappedObject
{
    [OSCMap("/test/lookDir")]
    public float3 lookDir;

    [OSCMap("/test/float")]
    public float testFloat;

    [OSCMap("/test/string")]
    public string? testString;

    [OSCMap("/oscControl/slider1")]
    public float slider1;

    [OSCMap("/oscControl/toggle1")]
    public float toggle1;

    [OSCMap("/oscControl/toggle2")]
    public float toggle2;
}

using Elements.Core;

namespace Impressive;
public static class OSCTypeConverters
{
    public static float3 Convert(object[] data)
    {
        return new((float)data[0], (float)data[1], (float)data[2]);
    }
}
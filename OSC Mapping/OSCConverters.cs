using System.Reflection;
using System.Collections.Frozen;
using Elements.Core;

namespace Impressive;
public static class OSCConverters
{
    // Scan OSCConverters for conversion methods. Must have only 1 parameter of type object[]
    public static readonly FrozenDictionary<Type, MethodInfo> Converters =
        typeof(OSCConverters)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Where(m => m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(object[]))
        .ToFrozenDictionary(k => k.ReturnType);
    
    public static float3 Convert(object[] objs)
    {
        return new((float)objs[0], (float)objs[1], (float)objs[2]);
    }
}

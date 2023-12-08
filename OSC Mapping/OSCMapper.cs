using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Linq.Expressions;
using System.Reflection;

namespace OSCMapper;
public static class OSCMapper
{
    // Concurrent dictionary since we do lazy caching for the OSC lookups that can come from multiple threads
    private static readonly ConcurrentDictionary<Type, FrozenDictionary<string, Action<object, object[]>>> fieldCaches = new();

    // Generates an accessor delegate for field or property members
    private static Action<object, object[]> MapMember(MemberInfo member)
    {
        // Ensure that the member is actually a property or a field
        Type? memberType =
            (member as FieldInfo)?.FieldType ??
            (member as PropertyInfo)?.PropertyType ??
            throw new NullReferenceException($"Something's gone terribly wrong and MapMember() was passed an incompatible type! ({member.GetType()})");
        

        // Define two parameters; one for a mapped object, and another for the object array to pass
        var targ = Expression.Parameter(typeof(object), "target");
        var obj = Expression.Parameter(typeof(object[]), "object");


        var targCasted = Expression.Convert(targ, member.DeclaringType); // Cast "target" to the declaring type since we know it
        

        bool got = OSCConverters.Converters.TryGetValue(memberType, out MethodInfo? conv); // Try to get a converter
        var first = Expression.ArrayAccess(obj, Expression.Constant(0)); // object[0], just in case.


        // If a converter is found, use it to convert the array, otherwise attempt to convert object[0] the good ol'-fashioned way.
        Expression convert = got ? Expression.Call(null, conv, obj) : Expression.Convert(first, memberType);


        Expression targAccessor = Expression.MakeMemberAccess(targCasted, member); // Make an accessor for the field or property
        var objAssign = Expression.Assign(targAccessor, convert);                  // Use the accessor to assign a value to the member


        // Finally, compile the lambda and return.
        return Expression.Lambda<Action<object, object[]>>(objAssign, targ, obj).Compile();
    }

    public static bool TryMapOSC(this object obj, string path, object[] data)
    {
        Type objType = obj.GetType();

        // Check if the type exists in our lookup.
        // If not, attempt to generate a lookup for the OSC-mapped fields
        if (!fieldCaches.TryGetValue(objType, out var lookup))
        {
            lookup = objType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) // Get the members
                .Select(mbr => (mbr, attr: mbr.GetCustomAttribute<OSCMapAttribute>())) // Get the attribute & pass it along
                .Where(m => m.attr != null)                                            // Make sure the attribute isn't null
                .ToFrozenDictionary(a => a.attr.Path, a => MapMember(a.mbr));          // Store the lookup in an immutable dict for efficiency
            
            fieldCaches.TryAdd(objType, lookup);                                       // Add the lookup to the type lookup dict
        }

        if (lookup.TryGetValue(path, out var action)) // See if the OSC path has an accessor
        {
            action.Invoke(obj, data);                 // Invoke the accessor with the data we want to set
            return true;
        }

        return false;
    }

    public static bool TryMapTo(this object[] data, object obj, string path)
    {
        return obj.TryMapOSC(path, data);
    }
}

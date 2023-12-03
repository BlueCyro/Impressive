using System.Collections.Frozen;
using System.Linq.Expressions;
using System.Reflection;

namespace Impressive;
public abstract class OSCMappedObject
{
    // This way we don't need to look up the fields for every instantiation.
    private static readonly Dictionary<Type, FrozenDictionary<string, Action<OSCMappedObject, object[]>>> fieldCaches = new();

    public OSCMappedObject()
    {
        lock (fieldCaches) // Lazy, but shouldn't really cause contention if a cache already exists.
        {
            if (fieldCaches.ContainsKey(GetType()))
            {
                Console.WriteLine($"Cache already contains accessors for {GetType()}");
                return;
            }
            
            Console.WriteLine($"Cache miss for {GetType()}");

            // Get all of the fields or properties on the inherited class which have the "OSCMapAttribute" applied.
            var members = GetType()
                .GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)
                .Where(m => m.MemberType == MemberTypes.Field || m.MemberType == MemberTypes.Property)
                .Select(m => new { info = m, attr = m.GetCustomAttribute<OSCMapAttribute>() })
                .Where(m => m.attr != null);
            

            Console.WriteLine("Fields instantiated");

            // Build efficient accessors for any annotated fields in the inherited classes.
            // This way, we can simply mark each field with a path that an OSC message should be routed to.
            Dictionary<string, Action<OSCMappedObject, object[]>> fieldDict = new();
            Console.WriteLine("Dict instantiated");
            foreach (var mapped in members)
            {
                // Define two parameters; one for a mapped object, and another for the object array to pass
                var targ = Expression.Parameter(typeof(OSCMappedObject), "target");
                var obj = Expression.Parameter(typeof(object[]), "object");
                var targInherited = Expression.Convert(targ, GetType()); // Cast the base type to the inherited one since we know it.

                Expression convert;
                Type memberType = typeof(Type);

                // Store the type of the member
                if (mapped.info is FieldInfo fieldInfo)
                    memberType = fieldInfo.FieldType;
                else if (mapped.info is PropertyInfo propInfo)
                    memberType = propInfo.PropertyType;

                // Choose which expression to use based on whether an explicit converter exists
                if (OSCConverters.Converters.TryGetValue(memberType, out MethodInfo? conv))
                {
                    // If a conversion method is found, use that to convert the value.
                    convert = Expression.Call(null, conv, obj);
                }
                else 
                {
                    // Otherwise, play it safe and try to cast object[0] to whatever type the field is
                    var first = Expression.ArrayAccess(obj, Expression.Constant(0));
                    convert = Expression.Convert(first, memberType);
                }

                // Get the member we're targetting from the inherited class and assign the result of the conversion expression to it.
                Expression targMember = Expression.MakeMemberAccess(targInherited, mapped.info);
                var objAssign = Expression.Assign(targMember, convert);

                // Finally, compile the lambda and add it to the accessor dictionary.
                var lmb = Expression.Lambda<Action<OSCMappedObject, object[]>>(objAssign, targ, obj).Compile();
                Console.WriteLine($"Instantiating accessor for field {mapped.info}");
                fieldDict.Add(mapped.attr.Path, lmb);
            }

            fieldCaches.Add(GetType(), fieldDict.ToFrozenDictionary());
        }
    }

    public bool TryMapOSC(string path, object[] data)
    {
        bool gotten = false;
        if (fieldCaches.TryGetValue(GetType(), out var lookup))
        {
            if (lookup.TryGetValue(path, out var action))
            {
                action.Invoke(this, data);
                gotten = true;
            }
        }
        else
        {
            throw new NullReferenceException($"Something is terribly wrong and field accessors were not generated for this type. ({GetType()})");
        }
        return gotten;
    }
}

namespace OSCMapper;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class OSCMapAttribute : Attribute
{
    public OSCMapAttribute(string path)
    {
        Path = path;
    }
    public readonly string Path;
}
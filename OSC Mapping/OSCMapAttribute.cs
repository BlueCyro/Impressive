namespace Impressive;
public class OSCMapAttribute : Attribute
{
    public OSCMapAttribute(string path)
    {
        Path = path;
    }
    public readonly string Path;
}
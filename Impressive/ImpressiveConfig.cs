using System.Reflection;
using ResoniteModLoader;

namespace Impressive;

public partial class Impressive : ResoniteMod
{
    [AutoRegisterConfigKey]
    internal static ModConfigurationKey<int> Port_Config = new("Port", "The port on which OSC will listen", () => 8000);
}
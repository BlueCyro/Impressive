using HarmonyLib;
using ResoniteModLoader;
using System;
using System.Reflection;
using FrooxEngine;
using Elements.Core;

namespace Impressive;

public class Impressive : ResoniteMod
{
    public override string Name => "Impressive";
    public override string Author => "Cyro";
    public override string Version => "1.0.0";
    public override string Link => "???";
    public static ModConfiguration? Config;

    public override void OnEngineInit()
    {
        Harmony harmony = new("net.Cyro.Impressive");
        Config = GetConfiguration();
        Config?.Save(true);
        harmony.PatchAll();
    }
}

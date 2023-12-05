﻿using HarmonyLib;
using ResoniteModLoader;
using FrooxEngine;

namespace Impressive;

public partial class Impressive : ResoniteMod
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
        // harmony.PatchAll();
        Msg("Patched successfully!");
        Engine engine = Engine.Current;
        engine.RunPostInit(() => engine.InputInterface.RegisterInputDriver(new SteamLinkDriver()));
    }
}

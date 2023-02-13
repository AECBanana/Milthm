using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Mod
{
    AutoPlay, Vertical, NoDead, Mirror, PerfectionismI, PerfectionismII, PerfectionismIII
}

public static class Mods
{
    public static readonly Dictionary<Mod, bool> Data;

    static Mods()
    {
        Data = new()
        {
            [Mod.AutoPlay] = false,
            [Mod.Vertical] = false,
            [Mod.NoDead] = false,
            [Mod.Mirror] = false,
            [Mod.PerfectionismI] = false,
            [Mod.PerfectionismII] = false,
            [Mod.PerfectionismIII] = false
        };
    }
}

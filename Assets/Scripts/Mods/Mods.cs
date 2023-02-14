using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Mod
{
    AutoPlay, Vertical, NoDead, Mirror, 
    PerfectionismI, PerfectionismII, PerfectionismIII, 
    Invisible, Dance, Bed
}

public static class Mods
{
    public static readonly Dictionary<Mod, bool> Data;

    static Mods()
    {
        Data = new Dictionary<Mod, bool>
        {
            [Mod.AutoPlay] = false,
            [Mod.Vertical] = false,
            [Mod.NoDead] = false,
            [Mod.Mirror] = false,
            [Mod.PerfectionismI] = false,
            [Mod.PerfectionismII] = false,
            [Mod.PerfectionismIII] = false,
            [Mod.Invisible] = false,
            [Mod.Dance] = false,
            [Mod.Bed] = false
        };
    }
}

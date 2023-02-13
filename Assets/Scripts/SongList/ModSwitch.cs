using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ModSwitch : MonoBehaviour
{
    public Mod mod;
    public List<Mod> repelMods;
    public Sprite active, deactivate;
    private Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
    }
    public void Click()
    {
        Mods.Data[mod] = !Mods.Data[mod];
        SndPlayer.Play("UI_Buttons_Pack2\\" + (Mods.Data[mod] ? "Button_21_Pack2" : "Button_19_Pack2"));
        if (!Mods.Data[mod]) return;
        foreach (var m in repelMods)
        {
            Mods.Data[m] = false;
        }
    }

    private void Update()
    {
        image.sprite = (Mods.Data[mod] ? active : deactivate);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using UnityEngine;
using UnityEngine.EventSystems;

public class SongItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public SongItemController controller;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (controller.RemoveBtn.activeSelf)
            return;
        controller.animator.SetBool("MouseIn", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (controller.RemoveBtn.activeSelf)
            return;
        controller.animator.SetBool("MouseIn", false);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            controller.DeleteMode();
            return;
        }
        controller.Preview.SongItem = controller;
        controller.Preview.Show(controller.Beatmap);
        PlayerPrefs.SetString("LastSong", controller.Beatmap);
    }
}

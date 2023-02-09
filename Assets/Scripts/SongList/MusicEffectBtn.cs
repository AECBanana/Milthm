using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicEffectBtn : MonoBehaviour
{
    public static Sprite CurrentSprite;
    public static bool Clicked = false;
    public AudioHighPassFilter Filter;
    public Image Background;
    public GameObject Effect;
    public bool Open;

    public void Click()
    {
        if (Clicked)
            return;
        Clicked = true;
        Filter.enabled = false;
        Effect.SetActive(true);
        if (CurrentSprite != null)
            Background.sprite = CurrentSprite;
    }

    public void ResetClick()
    {
        Clicked = false;
    }

    private void Update()
    {
        if (Open)
            return;
        if (Input.GetMouseButtonUp(0) && !Clicked)
        {
            Clicked = true;
            Filter.enabled = true;
            GetComponent<Animator>().Play("MusicEffectHide", 0, 0.0f);
        }
    }
}

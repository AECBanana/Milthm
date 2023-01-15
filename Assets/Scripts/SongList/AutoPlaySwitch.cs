using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlaySwitch : MonoBehaviour
{
    public Sprite Active, Deactive;
    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = (GamePlayLoops.AutoPlay ? Active : Deactive);
    }
    public void Click()
    {
        GamePlayLoops.AutoPlay = !GamePlayLoops.AutoPlay;
        image.sprite = (GamePlayLoops.AutoPlay ? Active : Deactive);
    }
}

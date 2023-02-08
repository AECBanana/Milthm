using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerticalSwitch : MonoBehaviour
{
    public Sprite Active, Deactive;
    Image image;
    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = (GamePlayLoops.Vertical ? Active : Deactive);
    }
    public void Click()
    {
        GamePlayLoops.Vertical = !GamePlayLoops.Vertical;
        image.sprite = (GamePlayLoops.Vertical ? Active : Deactive);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewSwitchBtn : MonoBehaviour
{
    public void ClickLeft()
    {
        SongPreviewController.LeftBtn = true;
    }
    public void ClickRight()
    {
        SongPreviewController.RightBtn = true;
    }
}

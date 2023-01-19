using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBtn : MonoBehaviour
{
    public void ClickPause()
    {
        GamePlayLoops.PauseBtn = true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackBtn : MonoBehaviour
{
    public void Click()
    {
        GamePlayAdapter.Instance.DeleteRain();
        Loading.Run("MainScene");
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            Click();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������淵�ذ�ť
/// </summary>
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

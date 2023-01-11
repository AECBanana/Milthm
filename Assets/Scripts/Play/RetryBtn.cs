using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryBtn : MonoBehaviour
{
    bool clicked = false;
    public void Click()
    {
        if (clicked)
            return;
        clicked = true;
        Loading.Run("PlayScene");
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
            Click();
    }
}

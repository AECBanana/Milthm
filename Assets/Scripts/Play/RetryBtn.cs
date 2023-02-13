using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryBtn : MonoBehaviour
{
    public void Click()
    {
        if (HitJudge.Result.DeadTime == DateTime.MinValue)
            GamePlayAdapter.Instance.Retry();
    }
    public void LoadRetry()
    {
        BeatmapLoader.Instance.Load("", BeatmapLoader.Playing);
        if (GamePlayLoops.Instance.PauseScreen.activeSelf)
            GamePlayLoops.Instance.PauseScreen.GetComponent<Animator>().Play("HidePausePanel", 0, 0.0f);
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
            Click();
    }
}

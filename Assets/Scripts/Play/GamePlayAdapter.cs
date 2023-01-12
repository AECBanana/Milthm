using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayAdapter : MonoBehaviour
{
    List<GameObject> Rains = new List<GameObject>();
    public AudioSource BGM;
    public void PlaySummarySnd()
    {
        if (HitJudge.Result.Dead)
            SndPlayer.Play("Fail");
        else
            SndPlayer.Play("UI_Buttons_Pack2\\Button_11_Pack2");
    }
    public void HideLines()
    {
        HitJudge.Result.DeadTime = System.DateTime.MinValue;
    }
    public void PlaySummaryBGM()
    {
        BGM.clip = BeatmapLoader.Instance.Audio.clip;
        BGM.Play();
        if (BeatmapLoader.Playing.PreviewTime != -1)
            BGM.time = BeatmapLoader.Playing.PreviewTime;
        GameObject rain = Instantiate(GamePlayLoops.Instance.Rain, GamePlayLoops.Instance.Rain.transform.parent);
        Rains.Add(rain);
        rain.SetActive(true);
    }
    public void ContinuePlay()
    {
        GamePlayLoops.Instance.CountDown.SetActive(true);
        GamePlayLoops.Instance.PauseScreen.GetComponent<Animator>().Play("HidePausePanel", 0, 0.0f);
    }
    public void DeleteRain()
    {
        foreach (GameObject go in Rains)
            Destroy(go);
        Rains.Clear();
    }
}

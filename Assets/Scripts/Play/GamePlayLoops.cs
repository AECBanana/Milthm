using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayLoops : MonoBehaviour
{
    public static GamePlayLoops Instance;
    public Transform ProgressBar, HPBar;
    public float display_width;
    public Text Score, Combo, Accuracy, Pitch;
    public Animator DangerAni, SummaryAni;
    public SummaryInfoCollector SummaryInfo;

    private void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        ProgressBar.localScale = new Vector3(AudioUpdate.Time / AudioUpdate.Audio.clip.length, 1f, 1f);
        Score.text = HitJudge.Result.Score.ToString("0000000");
        Accuracy.text = HitJudge.Result.Accuracy.ToString("P");
        Combo.text = HitJudge.Result.Combo + " <b>Combo</b>";

        float width = HitJudge.Result.HP / 100.0f;
        display_width += (width - display_width) / (Time.deltaTime / (1.0f / 60f) * 60f);
        HPBar.transform.localScale = new Vector3(display_width, 1f, 1f);

        for (int i = 0;i < HitJudge.CaptureOnce.Count; i++)
        {
            if (i >= HitJudge.CaptureOnce.Count) break;
            if (Input.GetKeyUp(HitJudge.CaptureOnce[i]))
            {
                HitJudge.CaptureOnce.RemoveAt(i);
                i--;
            }   
        }  

        if (!BeatmapLoader.Instance.Audio.isPlaying && !HitJudge.Result.Dead && !HitJudge.Result.Win && HitJudge.Result.Hit > 0)
        {
            HitJudge.Result.Win = true;
            GamePlayLoops.Instance.SummaryInfo.UpdateInfo();
            GamePlayLoops.Instance.SummaryAni.Play("WinSummary", 0, 0.0f);
        }
    }
}

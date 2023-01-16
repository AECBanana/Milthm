using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// 游玩主控制
/// </summary>
public class GamePlayLoops : MonoBehaviour
{
    public static bool AutoPlay = false;
    public static GamePlayLoops Instance;
    public Transform ProgressBar, HPBar;
    public float display_width;
    public Text Score, Combo, Accuracy, Pitch;
    public Animator DangerAni, SummaryAni;
    public SummaryInfoCollector SummaryInfo;
    public GameObject BlackScreen, PauseScreen, CountDown, Rain, AutoPlayTip;

    private void Awake()
    {
        Instance = this;
        AutoPlayTip.SetActive(AutoPlay);
    }
    void Update()
    {
        // 更新游玩基本信息
        ProgressBar.localScale = new Vector3(AudioUpdate.Time / AudioUpdate.Audio.clip.length, 1f, 1f);
        Score.text = HitJudge.Result.Score.ToString("0000000");
        Accuracy.text = HitJudge.Result.Accuracy.ToString("P");
        Combo.text = HitJudge.Result.Combo + " <b>Combo</b>";

        float width = HitJudge.Result.HP / 100.0f;
        display_width += (width - display_width) / (Time.deltaTime / (1.0f / 60f) * 60f);
        HPBar.transform.localScale = new Vector3(display_width, 1f, 1f);

        // 重置已捕获的输入
        string keys = "";
        for (int i = 0;i < HitJudge.CaptureOnce.Count; i++)
        {
            if (i >= HitJudge.CaptureOnce.Count) break;
            if (!HitJudge.IsHolding(HitJudge.CaptureOnce[i]))
            {
                HitJudge.BindNotes[HitJudge.CaptureOnce[i]] = null;
                HitJudge.CaptureOnce.RemoveAt(i);
                i--;
            }
            else
            {
                keys += HitJudge.CaptureOnce[i] + ",";
            }
        }

        // 关卡结束判定
        if (!HitJudge.Result.Dead && BeatmapLoader.Playing != null && !HitJudge.Result.Win)
        {
            DebugInfo.Output("SongLength", BeatmapLoader.Playing.SongLength.ToString());
            DebugInfo.Output("Hit/FC", HitJudge.Result.Hit + "/" + HitJudge.Result.FullCombo);
            // 未指定歌曲长度则在最后一个note击打后结束
            if (BeatmapLoader.Playing.SongLength == -1f)
            {
                if (LineController.Lines.Count == 0)
                {
                    HitJudge.Result.Win = true;
                    SummaryInfo.UpdateInfo();
                    SummaryAni.Play("WinSummary", 0, 0.0f);
                }
            }
            else
            {
                float length = BeatmapLoader.Playing.SongLength;
                if (length == 0)
                    length = BeatmapLoader.Instance.Audio.clip.length;
                if (BeatmapLoader.Instance.Audio.time >= length)
                {
                    HitJudge.Result.Win = true;
                    SummaryInfo.UpdateInfo();
                    SummaryAni.Play("WinSummary", 0, 0.0f);
                }
            }
        }

        // 暂停界面控制
        if (Input.GetKeyUp(KeyCode.Escape) && !PauseScreen.activeSelf && !HitJudge.Result.Dead && !HitJudge.Result.Win && AudioUpdate.Started && !CountDown.activeSelf)
        {
            HitJudge.CaptureOnce.Clear();
            AudioUpdate.Audio.Pause();
            PauseScreen.SetActive(true);
        }
    }
}

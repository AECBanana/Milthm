using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// 游玩主控制
/// </summary>
public class GamePlayLoops : MonoBehaviour
{
    public static float FlowSpeedFactor = 1.0f;
    public static bool PauseBtn = false;
    public static GamePlayLoops Instance;
    public Transform ProgressBar, HPBar;
    public float display_width;
    public TextMeshProUGUI Combo, Score, Accuracy, ComboTip, Skip;
    public GameObject Early, Late, SkipTip;
    public RectTransform SkipBack, SkipFore;
    public Animator DangerAni, SummaryAni;
    public SummaryInfoCollector SummaryInfo;
    public GameObject BlackScreen, PauseScreen, CountDown, Rain, AutoPlayTip;
    public string lastKey = "";
    DateTime skipTime = DateTime.MinValue;

    private void Awake()
    {
        Instance = this;
        AutoPlayTip.SetActive(Mods.Data[Mod.AutoPlay]);
        HitJudge.Record = bool.Parse(PlayerPrefs.GetString("ExportJudge", "False"));
    }
    private void OnDestroy()
    {
        foreach(AudioClip clip in SongResources.HitSnd.Values)
        {
            Destroy(clip);
        }
        SongResources.HitSnd.Clear();
    }
    void Update()
    {
        // 更新游玩基本信息
        ProgressBar.localScale = new Vector3(AudioUpdate.Time / AudioUpdate.Audio.clip.length, 1f, 1f);
        Score.text = HitJudge.Result.Score.ToString("0000000");
        Accuracy.text = HitJudge.Result.Accuracy.ToString("P");
        Combo.text = HitJudge.Result.Combo + " <b>COMBO</b>";

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
                if (HitJudge.Record)
                    HitJudge.RecordLog.AppendLine("[Self-Released] " + HitJudge.CaptureOnce[i] + " released.");
                HitJudge.BindNotes[HitJudge.CaptureOnce[i]] = null;
                HitJudge.CaptureOnce.RemoveAt(i);
                i--;
            }
            else
            {
                keys += HitJudge.CaptureOnce[i] + ",";
            }
        }
        if (keys != lastKey && HitJudge.Record)
        {
            if (keys == "")
                HitJudge.RecordLog.AppendLine("[Log] NO INPUTS this frame <At " + AudioUpdate.Time + ">");
            else
                HitJudge.RecordLog.AppendLine("[Log] " + keys + " are holding this frame <At " + AudioUpdate.Time + ">");
        } 
        lastKey = keys;

        // 关卡结束判定
        if (!HitJudge.Result.Dead && BeatmapLoader.Playing != null && !HitJudge.Result.Win)
        {
            DebugInfo.Output("SongLength", BeatmapLoader.Playing.SongLength.ToString());
            DebugInfo.Output("Hit/FC", HitJudge.Result.Hit + "/" + HitJudge.Result.FullCombo);
            // 未指定歌曲长度则在最后一个note击打后结束
            if (Math.Abs(BeatmapLoader.Playing.SongLength - (-1f)) < 0.001f)
            {
                if (LineController.UnhitLines.Count == 0)
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

        int bpm = 0; float t = AudioUpdate.Time;
        for (int i = 0;i < BeatmapLoader.Playing!.BPMList.Count; i++)
        {
            if (t < BeatmapLoader.Playing.BPMList[i].Start)
                break;
            else
                bpm = i;
        }
        FlowSpeedFactor = Mathf.Clamp(BeatmapLoader.Playing.BPMList[bpm].BPM / 250f, 0.8f, 1.2f);
        //Debug.Log("Current BPM: " + BeatmapLoader.Playing.BPMList[bpm].BPM + ", flowspeed factor: " + FlowSpeedFactor);

        // 暂停界面控制
        if ((Input.GetKeyUp(KeyCode.Escape) || PauseBtn) && !PauseScreen.activeSelf && !HitJudge.Result.Dead && !HitJudge.Result.Win && AudioUpdate.Started && !CountDown.activeSelf)
        {
            PauseBtn = false;
            HitJudge.CaptureOnce.Clear();
            AudioUpdate.Audio.Pause();
            PauseScreen.SetActive(true);
        }

        // 跳过控制
        if (HitJudge.HitList.Count > 0 && HitJudge.HitList[0].Count > 0 && !HitJudge.Result.Win && !HitJudge.Result.Dead)
        {
            float time = HitJudge.HitList[0][0] switch
            {
                TapController tap => tap.From,
                HoldController hold => hold.From,
                _ => 0
            };
            if (time - AudioUpdate.Time > 3 && AudioUpdate.Started)
            {
                Skip.gameObject.SetActive(true);
                Skip.text = "Relax  " + Mathf.Round(time - AudioUpdate.Time - 3) + "s";
                if (Input.anyKey && time - AudioUpdate.Time > 4)
                {
                    if (skipTime == DateTime.MinValue)
                    {
                        Debug.Log("Skip started.");
                        SkipTip.SetActive(false);
                        SkipTip.SetActive(true);
                        SkipFore.sizeDelta = new Vector2(SkipFore.sizeDelta.x, 0);
                        skipTime = DateTime.Now;
                    }
                    else
                    {
                        float pro = (float)((DateTime.Now - skipTime).TotalSeconds / 1.0);
                        if (pro > 1)
                            pro = 1;
                        SkipFore.sizeDelta = new Vector2(SkipFore.sizeDelta.x, pro * SkipBack.sizeDelta.y);
                        if (pro >= 1)
                        {
                            AudioUpdate.Audio.time = time - 3f;
                            AudioUpdate.m_Time = time - 3f;
                            AudioUpdate.b_Time = time - 3f;
                            AudioUpdate.updateWatch.Restart();
                            skipTime = DateTime.MinValue;
                            Debug.Log("Skip done.");
                            SkipTip.GetComponent<Animator>().Play("SkipTipDone", 0, 0.0f);
                        }
                    }
                } 
                else if (skipTime != DateTime.MinValue)
                {
                    Debug.Log("Skip released.");
                    if ((DateTime.Now - skipTime).TotalSeconds >= 0.25f)
                        SkipTip.GetComponent<Animator>().Play("SkipTipHide", 0, 0.0f);
                    else
                        SkipTip.SetActive(false);
                    skipTime = DateTime.MinValue;
                }
            }
            else
            {
                Skip.gameObject.SetActive(false);
                if (skipTime != DateTime.MinValue)
                {
                    Debug.Log("Skiped.");
                    if ((DateTime.Now - skipTime).TotalSeconds >= 0.25f)
                        SkipTip.GetComponent<Animator>().Play("SkipTipHide", 0, 0.0f);
                    else
                        SkipTip.SetActive(false);
                    skipTime = DateTime.MinValue;
                }
            }
        }
        else
        {
            Skip.gameObject.SetActive(false);
        }
    }
}

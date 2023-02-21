using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SummaryInfoCollector : MonoBehaviour
{
    public GameObject NewRecord, AutoPlayTip;
    public TMP_Text Perfect2, Perfect, Good, BadAndMiss, Score, Accuracy, Title, Credits, EarlyAndLate, Difficulty, MaxCombo;
    public Image Word, Level;
    public Sprite R, SS, S, A, B, C, F, FC, FCW, AP, Complete, Fail, ROR;

    public void UpdateInfo()
    {
        if (HitJudge.Record)
        {
            File.WriteAllText(Application.persistentDataPath + "/" + "judge_record_" + Guid.NewGuid().ToString() + ".txt", HitJudge.RecordLog.ToString());
            HitJudge.RecordLog.Clear();
        }
        GamePlayLoops.Instance.DangerAni.gameObject.SetActive(false);
        Perfect2.text = HitJudge.Result.Perfect2.ToString();
        Perfect.text = HitJudge.Result.Perfect.ToString();
        Good.text = HitJudge.Result.Good.ToString();
        BadAndMiss.text = HitJudge.Result.Bad + "/" + HitJudge.Result.Miss;
        MaxCombo.text = HitJudge.Result.MaxCombo.ToString();
        Score.text = HitJudge.Result.Score.ToString("0000000");
        Accuracy.text = HitJudge.Result.Accuracy.ToString("P");
        Title.text = BeatmapLoader.Playing.Title;
        EarlyAndLate.text = HitJudge.Result.Early + "/" + HitJudge.Result.Late;
        Credits.text = BeatmapLoader.Playing.Composer;
        string source = "";
        
        Difficulty.text = $"{BeatmapLoader.Playing.Difficulty}    <b>{BeatmapLoader.Playing.DifficultyValue}</b>";

        long score = (long)HitJudge.Result.OriginScore;
        string grade = "";
        Word.sprite = Complete;
        if (HitJudge.Result.Perfect2 == HitJudge.Result.FullCombo)
        {
            Word.sprite = ROR;
            Level.sprite = R;
            grade = "R";
        }
        else if (HitJudge.Result.Perfect2 + HitJudge.Result.Perfect == HitJudge.Result.FullCombo)
        {
            Word.sprite = AP;
            Level.sprite = SS;
            grade = "AP";
        }
        else if (HitJudge.Result.Miss == 0)
        {
            Word.sprite = FCW;
            Level.sprite = FC;
            grade = "FC";
        }
        else if (score >= 1000000)
        {
            Level.sprite = S;
            grade = "S";
        }
        else if (score >= 920000)
        {
            Level.sprite = A;
            grade = "A";
        }
        else if (score >= 870000)
        {
            Level.sprite = B;
            grade = "B";
        }
        else if (score >= 820000)
        {
            Level.sprite = C;
            grade = "C";
        }
        else
        {
            Level.sprite = F;
            grade = "F";
        }
        if (HitJudge.Result.Dead || grade == "F")
        {
            Word.sprite = Fail;
            Level.sprite = F;
            grade = "F";
        }    
        Level.SetNativeSize();
        Word.SetNativeSize();

        AutoPlayTip.SetActive(Mods.Data[Mod.AutoPlay]);
        if (Mods.Data[Mod.AutoPlay] || Mods.Data[Mod.Bed])
        {
            NewRecord.SetActive(false);
            return;
        }

        bool newrecord = false;
        if (HitJudge.Result.Score > PlayerPrefs.GetInt(BeatmapLoader.PlayingUID + "." + BeatmapLoader.PlayingIndex + ".score"))
        {
            PlayerPrefs.SetInt(BeatmapLoader.PlayingUID + "." + BeatmapLoader.PlayingIndex + ".score", (int)HitJudge.Result.Score);
            PlayerPrefs.SetString(BeatmapLoader.PlayingUID + "." + BeatmapLoader.PlayingIndex + ".grade", grade);
            newrecord = true;
        }
        if (HitJudge.Result.Accuracy > PlayerPrefs.GetFloat(BeatmapLoader.PlayingUID + "." + BeatmapLoader.PlayingIndex + ".acc"))
        {
            PlayerPrefs.SetFloat(BeatmapLoader.PlayingUID + "." + BeatmapLoader.PlayingIndex + ".acc", HitJudge.Result.Accuracy);
            newrecord = true;
        }

        NewRecord.SetActive(newrecord);
    }
}

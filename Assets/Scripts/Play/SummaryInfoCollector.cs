using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummaryInfoCollector : MonoBehaviour
{
    public Text Perfect2, Perfect, Good, Bad, Miss, Score, Accuracy, Title, Credits, Early, Late, Source, MaxCombo;
    public Image Word, Level;
    public Sprite R, SS, S, A, B, C, F, FC, FCW, AP, Complete, Fail, ROR;

    public void UpdateInfo()
    {
        Perfect2.text = HitJudge.Result.Perfect2.ToString();
        Perfect.text = HitJudge.Result.Perfect.ToString();
        Good.text = HitJudge.Result.Good.ToString();
        Bad.text = HitJudge.Result.Bad.ToString();
        Miss.text = HitJudge.Result.Miss.ToString();
        MaxCombo.text = HitJudge.Result.MaxCombo.ToString();
        Score.text = HitJudge.Result.Score.ToString("0000000");
        Accuracy.text = HitJudge.Result.Accuracy.ToString("P");
        Title.text = BeatmapLoader.Playing.Title;
        Early.text = HitJudge.Result.Early.ToString();
        Late.text = HitJudge.Result.Late.ToString();
        Credits.text = BeatmapLoader.Playing.Difficulty + "(" + BeatmapLoader.Playing.DifficultyValue + ")";
        string source = "";

        if (BeatmapLoader.Playing.Source != "")
            source += " / " + BeatmapLoader.Playing.Source;
        if (BeatmapLoader.Playing.GameSource != "")
            source += " / " + BeatmapLoader.Playing.GameSource;

        if (source.Length > 0)
        {
            Source.text = "From " + source.Substring(3);
        }
        else
        {
            Source.text = "";
        }

        long score = HitJudge.Result.Score;
        Word.sprite = Complete;
        if (HitJudge.Result.Perfect2 == HitJudge.Result.FullCombo)
        {
            Word.sprite = ROR;
            Level.sprite = R;
        }
        else if (HitJudge.Result.Perfect2 + HitJudge.Result.Perfect == HitJudge.Result.FullCombo)
        {
            Word.sprite = AP;
            Level.sprite = SS;
        }
        else if (HitJudge.Result.Miss == 0)
        {
            Word.sprite = FCW;
            Level.sprite = FC;
        }
        else if (score >= 1000000)
        {
            Level.sprite = S;
        }
        else if (score >= 920000)
        {
            Level.sprite = A;
        }
        else if (score >= 870000)
        {
            Level.sprite = B;
        }
        else if (score >= 820000)
        {
            Level.sprite = C;
        }
        else
        {
            Level.sprite = F;
        }
        if (HitJudge.Result.Dead)
        {
            Word.sprite = Fail;
            Level.sprite = F;
        }    
        Level.SetNativeSize();
        Word.SetNativeSize();
    }
}

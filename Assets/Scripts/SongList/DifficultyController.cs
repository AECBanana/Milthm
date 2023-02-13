using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DifficultyController : MonoBehaviour
{
    public static DifficultyController Active;
    public static bool Ready = false;
    public Image Grade, Back;
    public TMP_Text Score, Accuracy, Title, Difficulty, Info;
    public Sprite ActiveSprite, DeactiveSprite;
    public string uid;
    public int index;

    private void Start()
    {
        if (PlayerPrefs.GetString(uid + "." + index + ".grade") == "")
        {
            Score.text = "0000000";
            Accuracy.text = "00.00%";
            Grade.sprite = Resources.Load<Sprite>("Level\\Unknown");
            Grade.SetNativeSize();
        }
        else
        {
            Score.text = PlayerPrefs.GetInt(uid + "." + index + ".score").ToString("0000000");
            Accuracy.text = PlayerPrefs.GetFloat(uid + "." + index + ".acc").ToString("P");
            Grade.sprite = Resources.Load<Sprite>("Level\\" + PlayerPrefs.GetString(uid + "." + index + ".grade"));
            Grade.SetNativeSize();
        }
        double diff = BeatmapDifficulty.Caculate(uid, SongResources.Beatmaps[uid][index]);
        Difficulty.text = diff.ToString("0.0");
        TimeSpan length = TimeSpan.FromSeconds(SongResources.Songs[uid].length);
        Info.text = "物量   " + SongResources.Beatmaps[uid][index].NoteList.Count + "   长度   " + length.Minutes + ":" + length.Seconds.ToString("00");
        if (Title.text == "")
            Title.text = "未命名的谱面";
    }

    public void Touch()
    {
        if (Active == this)
        {
            if (!Ready)
            {
                SongPreviewController.Instance.PanelAnimator.Play("ReadyPlay", 0, 0.0f);
                Ready = true;
            }
        }
        else
        {
            if (Ready)
            {
                SongPreviewController.Instance.PanelAnimator.Play("CancelPlay", 0, 0.0f);
                Ready = false;
            }
            Active.Back.sprite = DeactiveSprite;
            Back.sprite = ActiveSprite;
            Active = this;
            BeatmapModel m = SongResources.Beatmaps[uid][index];
            SongPreviewController.Instance.Description.text = "[曲]" + m.Composer + " [谱]" + m.Beatmapper + " [美]" + m.Illustrator;
            SongPreviewController.Instance.FakeBg.sprite = SongPreviewController.Instance.Illustration.sprite;
            SongPreviewController.Instance.FakeCover.gameObject.SetActive(false);
            SongPreviewController.Instance.TranslucentSource.maxUpdateRate = float.PositiveInfinity;
            SongPreviewController.Instance.TranslucentCamera.Render();
            SongPreviewController.Instance.TranslucentSource.maxUpdateRate = 0;
            SongPreviewController.Instance.FakeCover.gameObject.SetActive(true);
            SongPreviewController.Instance.Illustration.sprite = SongResources.Illustration[uid][m.IllustrationFile];
            SongPreviewController.Instance.Bg2.sprite = SongPreviewController.Instance.Illustration.sprite;
        }
    }
}

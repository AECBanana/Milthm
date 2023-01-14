using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyController : MonoBehaviour
{
    public static DifficultyController Active;
    public Image Grade, Back;
    public Text Score, Accuracy, Title;
    public Sprite ActiveSprite, DeactiveSprite;
    public string uid;
    public int index;

    private void Start()
    {
        if (PlayerPrefs.GetString(uid + "." + index + ".grade") == "")
        {
            Grade.gameObject.SetActive(false);
            Score.gameObject.SetActive(false);
            Accuracy.gameObject.SetActive(false);
        }
        else
        {
            Score.text = PlayerPrefs.GetInt(uid + "." + index + ".score").ToString("0000000");
            Accuracy.text = PlayerPrefs.GetFloat(uid + "." + index + ".acc").ToString("P");
            Grade.sprite = Resources.Load<Sprite>("Level\\" + PlayerPrefs.GetString(uid + "." + index + ".grade"));
        }
    }

    public void Touch()
    {
        if (Active == this)
        {
            BeatmapLoader.PlayingUID = uid;
            BeatmapLoader.PlayingIndex = index;
            PlayerPrefs.SetInt(uid + ".lastPlay", index);
            BeatmapLoader.Playing = SongResources.Beatmaps[uid][index];
            Loading.Run("PlayScene", "PlayLoadingPrefab");
        }
        else
        {
            Active.Back.sprite = DeactiveSprite;
            Back.sprite = ActiveSprite;
            Active = this;
            BeatmapModel m = SongResources.Beatmaps[uid][index];
            SongPreviewController.Instance.Description.text = "[Çú]" + m.Composer + " [Æ×]" + m.Beatmapper + " [ÃÀ]" + m.Illustrator;
            SongPreviewController.Instance.FakeCover.sprite = SongPreviewController.Instance.Illustration.sprite;
            SongPreviewController.Instance.FakeCover.gameObject.SetActive(false);
            SongPreviewController.Instance.FakeCover.gameObject.SetActive(true);
            SongPreviewController.Instance.Illustration.sprite = SongResources.Illustration[uid][m.IllustrationFile];
            SongPreviewController.Instance.Background.sprite = SongPreviewController.Instance.Illustration.sprite;
        }
    }
}

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

    public void Touch()
    {
        if (Active == this)
        {
            Debug.Log("���棡");
        }
        else
        {
            Active.Back.sprite = DeactiveSprite;
            Back.sprite = ActiveSprite;
            Active = this;
            BeatmapModel m = SongResources.Beatmaps[uid][index];
            SongPreviewController.Instance.Description.text = "���֣�" + m.Composer + " ���棺" + m.Beatmapper + " ���棺" + m.Illustrator;
            SongPreviewController.Instance.Illustration.sprite = SongResources.Illustration[uid][m.IllustrationFile];
        }
    }
}

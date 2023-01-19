using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayLoadScreen : MonoBehaviour
{
    public Image Bg, Illustration;
    public TMP_Text Title, Difficulty, Composor, Beatmapper, Illustrator, From;

    private void Awake()
    {
        BeatmapModel m = BeatmapLoader.Playing;
        string uid = BeatmapLoader.PlayingUID;

        Illustration.sprite = SongResources.Illustration[uid][m.IllustrationFile];
        Bg.sprite = Illustration.sprite;

        Title.text = m.Title;
        Composor.text = "����    " + m.Composer;
        Beatmapper.text = "����    " + m.Beatmapper;
        Illustrator.text = "����    " + m.Illustrator;

        string source = "";

        if (m.Source != "")
            source += " / " + m.Source;
        if (m.GameSource != "")
            source += " / " + m.GameSource;

        if (source.Length > 0)
        {
            From.text = "From " + source.Substring(3);
        }
        else
        {
            From.text = "";
        }

        Difficulty.text = m.Difficulty + (m.DifficultyValue == -1f ? "" : " (" + m.DifficultyValue + ")");
    }
}

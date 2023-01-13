using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongItemController : MonoBehaviour
{
    public string Beatmap;
    public Image Illustration;
    public Text Description;
    public SongPreviewController Preview;

    public void Touch()
    {
        Preview.Show(Beatmap);
    }
}

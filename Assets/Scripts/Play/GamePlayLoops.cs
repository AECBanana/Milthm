using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayLoops : MonoBehaviour
{
    public static GamePlayLoops Instance;
    public Transform ProgressBar;
    public Text Score, Combo, Accuracy, Pitch;
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

        for(int i = 0;i < HitJudge.CaptureOnce.Count; i++)
        {
            if (i >= HitJudge.CaptureOnce.Count) break;
            if (Input.GetKeyUp(HitJudge.CaptureOnce[i]))
            {
                HitJudge.CaptureOnce.RemoveAt(i);
                i--;
            }   
        }  
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DelayManager : MonoBehaviour
{
    public AudioSource Audio;
    public Animator[] SndTip;
    public Text DelayText;
    public GameObject DoneTip;
    public RectTransform progress;
    int lastTip = 0;
    List<AudioSource> capture = new List<AudioSource>();
    float delay = 0;
    int testCnt = 0, testCnt2 = 0;
    float display_delay = 0, temp_delay = 0;
    float display_width = 0, max_width;
    float single_delay = 0;
    int status = 0;
    bool tested = false;
    private void Start()
    {
        max_width = progress.sizeDelta.x;
        progress.sizeDelta = new Vector2(0, progress.sizeDelta.y);
        CaptureAudios();
    }
    void CaptureAudios()
    {
        foreach (AudioSource audio in GameObject.FindObjectsOfType<AudioSource>())
        {
            if (audio != Audio)
            {
                audio.mute = true;
                capture.Add(audio);
            }
        }
    }
    void ReleaseAudios()
    {
        foreach (AudioSource audio in capture)
        {
            audio.mute = false;
        }
    }
    void Update()
    {
        if (status == 0)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                status = 1;
                Audio.Play();
                DelayText.text = "+0 <b>ms</b>";
            }
        }
        else if (status == 1)
        {
            if (Audio.time >= lastTip * 1.0f && Audio.time < 3f)
            {
                if (lastTip <= 2)
                    SndTip[lastTip].Play("SndTipBurst", 0, 0.0f);
                if (lastTip == 0)
                    tested = false;
                lastTip++;
            }
            if (Audio.time >= 3f && lastTip > 1)
            {
                testCnt++;
                lastTip = 0;
                delay += single_delay;
                if (testCnt == 5)
                {
                    status = 2;
                    DoneTip.SetActive(true);
                    //progress.gameObject.SetActive(false);
                    ReleaseAudios();
                    Audio.Stop();
                }
            }
                
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (!tested)
                testCnt2++;
                single_delay = (Audio.time - 2f) * 1000f;
                float d = Mathf.Round((delay + single_delay) / testCnt2);
                temp_delay = d;
                tested = true;
            }
            display_delay += (temp_delay - display_delay) / (Time.deltaTime / (1.0f / 60f) * 60f);
            float t = Mathf.Round(display_delay);
            DelayText.text = (t >= 0 ? "+" : "") + t + " <b>ms</b>";

            float target_width = testCnt / 4.0f * max_width;
            display_width += (target_width - display_width) / (Time.deltaTime / (1.0f / 60f) * 60f);
            progress.sizeDelta = new Vector2(display_width, progress.sizeDelta.y);
        }
        else if (status == 2)
        {
            float target_width = 0;
            display_width += (target_width - display_width) / (Time.deltaTime / (1.0f / 60f) * 60f);
            progress.sizeDelta = new Vector2(display_width, progress.sizeDelta.y);
            progress.GetComponent<Image>().color = new Color(1f, 1f, 1f, 74f / 255f * display_width / max_width);
            if (Input.GetKeyDown(KeyCode.Z))
            {
                GetComponent<Animator>().Play("CanvasQuit", 0, 0.0f);
                PlayerPrefs.SetFloat("Delay", temp_delay);
                DoneTip.SetActive(false);
                status = 3;
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                delay = 0; single_delay = 0; display_delay = 0; temp_delay = 0;
                progress.GetComponent<Image>().color = new Color(1f, 1f, 1f, 74f / 255f);
                testCnt = 0; testCnt2 = 0;
                lastTip = 0; display_width = 0;
                DoneTip.SetActive(false);
                status = 1;
                CaptureAudios();
                Audio.Play();
                DelayText.text = "+0 <b>ms</b>";
            }
        }
    }
}

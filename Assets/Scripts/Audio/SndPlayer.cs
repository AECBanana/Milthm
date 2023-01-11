using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 音效播放器
/// </summary>
public class SndPlayer : MonoBehaviour
{
    public static Dictionary<string, AudioClip> LoadedClips = new Dictionary<string, AudioClip>();
    public static AudioSource ActiveSndPlayer = null;
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="snd">音效文件名</param>
    public static void Play(string snd)
    {
        try
        {
            if (!LoadedClips.ContainsKey(snd))
                LoadedClips.Add(snd, Resources.Load<AudioClip>("Snd\\" + snd));
            Play(LoadedClips[snd]);
        }
        catch
        {
            Debug.Log("无法播放：" + snd);
        }
    }
    public static void Play(AudioClip snd)
    {
        if (ActiveSndPlayer != null)
        {
            DebugInfo.Tick("Audio Clips/s");
            ActiveSndPlayer.PlayOneShot(snd);
            return;
        }
        GameObject SndPlayerPrefab = (GameObject)Resources.Load("SndPlayer");
        GameObject go = Instantiate(SndPlayerPrefab);
        AudioSource audio = go.GetComponent<AudioSource>();
        go.SetActive(true);
        DontDestroyOnLoad(go);
        ActiveSndPlayer = audio;
        ActiveSndPlayer.PlayOneShot(snd);
        DebugInfo.Tick("Audio Clips/s");
    }

    public AudioSource BGM;
    public void PlaySnd(string snd)
    {
        SndPlayer.Play(snd);   
    }
    public void PlaySummarySnd()
    {
        if (HitJudge.Result.Dead)
            SndPlayer.Play("Fail");
        else
            SndPlayer.Play("UI_Buttons_Pack2\\Button_11_Pack2");
        HitJudge.Result.DeadTime = new System.DateTime(0);
    }
    public void PlaySummaryBGM()
    {
        BGM.clip = BeatmapLoader.Instance.Audio.clip;
        BGM.Play();
        if (BeatmapLoader.Playing.PreviewTime != -1)
            BGM.time = BeatmapLoader.Playing.PreviewTime;
    }
}

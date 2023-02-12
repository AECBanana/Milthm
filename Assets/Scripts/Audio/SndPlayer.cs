using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 音效播放器
/// </summary>
public class SndPlayer : MonoBehaviour
{
    private static readonly Dictionary<string, AudioClip> LoadedClips = new Dictionary<string, AudioClip>();
    private static readonly AudioSource _activeSndPlayer = null;
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
            DebugInfo.Output("无法播放", snd);
        }
    }
    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="snd">音效资源</param>
    public static void Play(AudioClip snd)
    {
        DebugInfo.Tick("Audio Clips/s");
        _activeSndPlayer.PlayOneShot(snd);
    }

    static SndPlayer()
    {
        var sndPlayerPrefab = (GameObject)Resources.Load("SndPlayer");
        var go = Instantiate(sndPlayerPrefab);
        var audio = go.GetComponent<AudioSource>();
        go.SetActive(true);
        DontDestroyOnLoad(go);
        _activeSndPlayer = audio;
    }
}

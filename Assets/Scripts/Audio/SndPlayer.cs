using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��Ч������
/// </summary>
public class SndPlayer : MonoBehaviour
{
    private static readonly Dictionary<string, AudioClip> LoadedClips = new Dictionary<string, AudioClip>();
    private static readonly AudioSource _activeSndPlayer = null;
    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="snd">��Ч�ļ���</param>
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
            DebugInfo.Output("�޷�����", snd);
        }
    }
    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="snd">��Ч��Դ</param>
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

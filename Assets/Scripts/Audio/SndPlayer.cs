using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��Ч������
/// </summary>
public class SndPlayer : MonoBehaviour
{
    public static Dictionary<string, AudioClip> LoadedClips = new Dictionary<string, AudioClip>();
    public static AudioSource ActiveSndPlayer = null;
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
            Debug.Log("�޷����ţ�" + snd);
        }
    }
    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="snd">��Ч��Դ</param>
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
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

/// <summary>
/// 音频状态更新器
/// </summary>
public class AudioUpdate : MonoBehaviour
{
    /// <summary>
    /// 绑定音频
    /// </summary>
    public static AudioSource Audio;
    /// <summary>
    /// 开启游玩的时间
    /// </summary>
    public static DateTime StartTime;
    /// <summary>
    /// 是否开始游戏
    /// </summary>
    public static bool Started = false;
    public static AudioUpdate Instance;
    public bool PreviewMode = false;
    public static float m_Time;         // 歌曲播放进度缓存
    public static float b_Time;
    static bool playing = false;
    public static Stopwatch updateWatch;

    public static float Time
    {
        get
        {
            // 若尚未开始游戏
            if (!Started && Audio.time == 0 &&!Instance.PreviewMode)
            {
                // 返回负数时间让谱面正常下落
                m_Time = (float)(DateTime.Now - StartTime).TotalSeconds - 3.0f;
            }
            if (m_Time < -3f)
                m_Time = -3f;
            return m_Time;
        }
    }
    private void Awake()
    {
        updateWatch = new Stopwatch();
        Audio = GetComponent<AudioSource>();
        Instance = this;
    }
    private void OnDestroy()
    {
        updateWatch.Stop();
    }
    private void Update()
    {
        if (playing)
        {
            DebugInfo.Tick("TimeUpdate");
            m_Time = b_Time + updateWatch.ElapsedMilliseconds / 1000f;
        }
        if (Audio.isPlaying)
        {
            if (!playing)
            {
                playing = true;
                updateWatch.Restart();
                b_Time = Audio.time;
                m_Time = Audio.time;
            }
        }
        else
        {
            if (playing)
            {
                playing = false;
                updateWatch.Stop();
                m_Time = Audio.time;
            }
        }
        //UnityEngine.Debug.Log("Update delta time: " + UnityEngine.Time.deltaTime);
        //m_Time = Audio.time;

        // 如果正在暂停
        if (!Started && BeatmapLoader.Playing != null)
        {
            // 三秒后继续播放音乐
            if ((DateTime.Now - StartTime).TotalSeconds >= 3.0)
            {
                Started = true;
                Audio.Play();
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
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
    static bool updated = false;        // 歌曲播放进度更新状态
    static bool playing = false;
    public static DateTime updateTime = DateTime.MinValue;
    Timer updateTimer;

    public static float Time
    {
        get
        {
            if (!updated)
            {
                updated = true;
                if (Audio.time - m_Time >= 0.2f)
                {
                    Debug.Log("Update timer is too slow!!");
                    updateTime = DateTime.Now;
                    b_Time = Audio.time;
                    m_Time = Audio.time;
                }
                else if (m_Time - Audio.time >= 0.2f)
                {
                    Debug.Log("Update timer is too fast!!");
                    updateTime = DateTime.Now;
                    b_Time = Audio.time;
                    m_Time = Audio.time;
                }
                //DebugInfo.Output("同步状况", m_Time + " -> " + Audio.time);
            }
            // 若尚未开始游戏
            if (!Started && Audio.time == 0 &&!Instance.PreviewMode)
            {
                // 返回负数时间让谱面正常下落
                m_Time = (float)(DateTime.Now - StartTime).TotalSeconds - 3.0f;
            }
            return m_Time;
        }
    }
    private void Awake()
    {
        Audio = GetComponent<AudioSource>();
        Instance = this;
        updateTimer = new Timer((e) =>
        {
            if (playing)
            {
                m_Time = b_Time + (float)(DateTime.Now - updateTime).TotalSeconds;
                updated = false;
            }
        }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(5.0));
    }
    private void OnDestroy()
    {
        updateTimer.Dispose();
    }
    private void Update()
    {
        if (Audio.isPlaying)
        {
            if (!playing)
            {
                playing = true;
                updateTime = DateTime.Now;
                b_Time = Audio.time;
                m_Time = Audio.time;
            }
        }
        else
        {
            if (playing)
            {
                playing = false;
                m_Time = Audio.time;
            }
        }
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

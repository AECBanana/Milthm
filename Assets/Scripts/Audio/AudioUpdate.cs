using System;
using System.Collections;
using System.Collections.Generic;
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
    static float m_Time;                // 歌曲播放进度缓存
    static bool updated = false;        // 歌曲播放进度更新状态
    
    public static float Time
    {
        get
        {
            if (!updated)
            {
                updated = true;
                m_Time = Audio.time;
            }
            // 若尚未开始游戏
            if (!Started && Audio.time == 0)
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
    }
    private void FixedUpdate()
    {
        updated = false;
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

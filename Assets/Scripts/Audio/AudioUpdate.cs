using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// ��Ƶ״̬������
/// </summary>
public class AudioUpdate : MonoBehaviour
{
    /// <summary>
    /// ����Ƶ
    /// </summary>
    public static AudioSource Audio;
    /// <summary>
    /// ���������ʱ��
    /// </summary>
    public static DateTime StartTime;
    /// <summary>
    /// �Ƿ�ʼ��Ϸ
    /// </summary>
    public static bool Started = false;
    public static AudioUpdate Instance;
    public bool PreviewMode = false;
    public static float m_Time;         // �������Ž��Ȼ���
    public static float b_Time;
    static bool updated = false;        // �������Ž��ȸ���״̬
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
                //DebugInfo.Output("ͬ��״��", m_Time + " -> " + Audio.time);
            }
            // ����δ��ʼ��Ϸ
            if (!Started && Audio.time == 0 &&!Instance.PreviewMode)
            {
                // ���ظ���ʱ����������������
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

        // ���������ͣ
        if (!Started && BeatmapLoader.Playing != null)
        {
            // ����������������
            if ((DateTime.Now - StartTime).TotalSeconds >= 3.0)
            {
                Started = true;
                Audio.Play();
            }
        }
    }
}

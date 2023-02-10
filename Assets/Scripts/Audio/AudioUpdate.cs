using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    static bool playing = false;
    public static Stopwatch updateWatch;

    public static float Time
    {
        get
        {
            // ����δ��ʼ��Ϸ
            if (!Started && Audio.time == 0 &&!Instance.PreviewMode)
            {
                // ���ظ���ʱ����������������
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
    private void FixedUpdate()
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
    }
    private void Update()
    {
        //UnityEngine.Debug.Log("Update delta time: " + UnityEngine.Time.deltaTime);
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

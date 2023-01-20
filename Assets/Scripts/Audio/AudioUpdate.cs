using System;
using System.Collections;
using System.Collections.Generic;
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
    static float m_Time;                // �������Ž��Ȼ���
    static bool updated = false;        // �������Ž��ȸ���״̬
    
    public static float Time
    {
        get
        {
            if (!updated)
            {
                updated = true;
                if (Audio.time > m_Time)
                {
                    m_Time = Audio.time;
                }
                else if (m_Time - Audio.time >= 1f / 10f)
                {
                    m_Time += (Audio.time - m_Time) / 10f;
                }
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
    }
    private void Update()
    {
        if (updated && m_Time >= 0)
        {
            m_Time += UnityEngine.Time.deltaTime;
            if (!Audio.isPlaying)
                m_Time = Audio.time;
        }
        updated = false;
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

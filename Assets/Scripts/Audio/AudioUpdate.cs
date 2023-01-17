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
    static float m_Time;                // �������Ž��Ȼ���
    static bool updated = false;        // �������Ž��ȸ���״̬
    
    public static float Time
    {
        get
        {
            if (!updated)
            {
                updated = true;
                m_Time = Audio.time;
            }
            // ����δ��ʼ��Ϸ
            if (!Started && Audio.time == 0)
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
    }
    private void FixedUpdate()
    {
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

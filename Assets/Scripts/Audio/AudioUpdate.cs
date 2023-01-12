using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUpdate : MonoBehaviour
{
    public static AudioSource Audio;
    public static DateTime StartTime;
    public static bool Started = false;
    static float m_Time;
    static bool updated = false;
    
    public static float Time
    {
        get
        {
            if (!updated)
            {
                updated = true;
                m_Time = Audio.time;
            }
            if (!Started && Audio.time == 0)
            {
                m_Time = (float)(DateTime.Now - StartTime).TotalSeconds - 3.0f;
            }
            return m_Time;
        }
    }
    private void Awake()
    {
        Audio = GetComponent<AudioSource>();
    }
    private void Update()
    {
        updated = false;
        if (!Started && BeatmapLoader.Playing != null)
        {
            if ((DateTime.Now - StartTime).TotalSeconds >= 3.0)
            {
                Started = true;
                Audio.Play();
            }
        }
    }
}

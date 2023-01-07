using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUpdate : MonoBehaviour
{
    public static AudioSource Audio;
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
    }
}

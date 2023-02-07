using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSettings : MonoBehaviour
{
    public static List<VolumeSettings> Volumes = new List<VolumeSettings>();
    public bool Changed = false;
    public string SettingName;
    [Range(0f, 1f)]
    public float DefaultVolume;

    private void Awake()
    {
        Volumes.Add(this);
        GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(SettingName, DefaultVolume);
    }

    private void OnDestroy()
    {
        Volumes.Remove(this);
    }

    private void Update()
    {
        if (Changed)
        {
            GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat(SettingName, DefaultVolume);
            Changed = false;
        }
    }
}

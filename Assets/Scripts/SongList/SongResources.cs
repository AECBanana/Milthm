using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongResources : MonoBehaviour
{
    public static Dictionary<string, Dictionary<string, Sprite>> Illustration = new Dictionary<string, Dictionary<string, Sprite>>();
    public static Dictionary<string, List<BeatmapModel>> Beatmaps = new Dictionary<string, List<BeatmapModel>>();
    public static Dictionary<string, string> Path = new Dictionary<string, string>();
    public static Dictionary<string, AudioClip> Songs = new Dictionary<string, AudioClip>();
    private static string mDirectory = null;
    public static string DataPath
    {
        get
        {
            if (mDirectory == null)
            {
                mDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Milthm";
                if (!Directory.Exists(mDirectory))
                    Directory.CreateDirectory(mDirectory);
            }
            return mDirectory;
        }
    }

    private void OnApplicationQuit()
    {
        foreach(Dictionary<string, Sprite> dict in Illustration.Values)
        {
            foreach(Sprite sprite in dict.Values)
            {
                Destroy(sprite);
            }
        }
        foreach (AudioClip clip in Songs.Values)
            Destroy(clip);
    }
}

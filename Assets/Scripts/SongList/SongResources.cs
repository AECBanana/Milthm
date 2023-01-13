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
}

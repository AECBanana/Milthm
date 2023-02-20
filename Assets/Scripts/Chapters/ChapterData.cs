using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Chapter xxx", menuName = "Milthm/ChapterData")]
public class ChapterData : ScriptableObject
{
    [Serializable]
    public struct SongData
    {
        public AudioClip music;
        public Sprite illustration;
        public Sprite lockedIllustration;
        public string artist, title;
        public List<BeatmapModel> beatmaps;
    }

    public List<SongData> songs;
}

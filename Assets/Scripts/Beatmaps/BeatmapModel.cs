using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 谱面数据模型
/// </summary>
[Serializable]
public class BeatmapModel
{

    #region 元数据
    public string Title;
    public string Composer;
    public string Illustrator;
    public string Beatmapper;
    public string BeatmapUID = Guid.NewGuid().ToString();
    public string Difficulty;
    public double DifficultyValue;
    public string AudioFile;
    public string Source;
    public string GameSource;
    public float PreviewTime = -1f;
    public float SongLength = 0;
    #endregion

    #region BPM数据
    [Serializable]
    public class BPMData
    {
        public float From, To;
        public float BPM;
    }
    public List<BPMData> BPMList = new List<BPMData>();
    #endregion

    #region 物件数据
    [Serializable]
    public class NoteData
    {
        public int Line;
        public int[] From, To;
        public int BPM;
        public KeyCode SpecificKey = KeyCode.None;
        public float FromBeat
        {
            get
            {
                return From[0] + From[1] * 1.0f / From[2];
            }
        }
        public float ToBeat
        {
            get
            {
                return To[0] + To[1] * 1.0f / To[2];
            }
        }
    }
    public List<NoteData> NoteList = new List<NoteData>();
    #endregion

    #region 物件线数据
    public enum LineDirection
    {
        Left, Right, Up, Down
    }
    [Serializable]
    public class LineData
    {
        public LineDirection Direction;
        public float FlowSpeed;
        public KeyCode KeyOverride = KeyCode.None;
    }
    public List<LineData> LineList = new List<LineData>();
    #endregion

    #region 演出数据
    public enum PerformanceOperation
    {
        Move, Rotate, Transparent, ChangeDirection, ChangeKey, FlowSpeed
    }
    public enum PerformanceEaseType
    {
        Linear, BezierEaseIn, BezierEaseOut, BezierEase, ParabolicEase
    }
    [Serializable]
    public class PerformanceData
    {
        public float From, To;
        public int Line, Note;
        public PerformanceOperation Operation;
        public string Value;
        public PerformanceEaseType Ease;
    }
    public List<PerformanceData> PerformanceList = new List<PerformanceData>();
    #endregion

    public void Export(string Path)
    {
        File.WriteAllText(Path, JsonUtility.ToJson(this));
    }

    public int DetermineBPM(float time)
        => BPMList.FindIndex(x => x.From <= time && x.To >= time);


    public int[] ConvertByBPM(float time, int beat)
    {
        BPMData BPM = BPMList[DetermineBPM(time)];
        float beattime = 60.0f / BPM.BPM;
        int basebeat = (int)(Math.Floor((time - BPM.From) / beattime));
        return new int[]{
            basebeat,
            (int)(Math.Round((time - BPM.From - basebeat * beattime) / (beattime / beat))),
            beat
        };
    }

    public (float, float) ToRealTime(NoteData note)
    {
        return (BPMList[note.BPM].From + note.FromBeat * (60.0f / BPMList[note.BPM].BPM), BPMList[note.BPM].From + note.ToBeat * (60.0f / BPMList[note.BPM].BPM));
    }

    public static BeatmapModel Read(string Path)
    {
        return JsonUtility.FromJson<BeatmapModel>(File.ReadAllText(Path));
    }

    public float BezierCubic(float t, float a, float b, float c, float d)
    {
        return (float)((a * Math.Pow(1 - t, 3)) + (3 * b * t * Math.Pow(1 - t, 2)) + (3 * c * (1 - t) * Math.Pow(t, 2)) + (d * Math.Pow(t, 3)));
    }
}

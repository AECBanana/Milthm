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
        public int From, To;
        public int BPM;
        public KeyCode SpecificKey = KeyCode.None;
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


    public int ConvertByBPM(float time)
    {
        BPMData BPM = BPMList[DetermineBPM(time)];
        return (int)(Math.Ceiling((time - BPM.From) / (60.0 / BPM.BPM / 16)));
    }

    public (float, float) ToRealTime(NoteData note)
    {
        return (BPMList[note.BPM].From + note.From * (60.0f / BPMList[note.BPM].BPM / 16), BPMList[note.BPM].From + note.To * (60.0f / BPMList[note.BPM].BPM / 16));
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

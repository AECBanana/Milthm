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
    public string IllustrationFile;
    public string Source;
    public string GameSource;
    public float PreviewTime = -1f;
    public float SongLength = 0;
    public float SongOffset = 0;
    public string FormatVersion;
    public string SndSet;
    #endregion

    #region BPM数据
    [Serializable]
    public class BPMData
    {
        public float Start;
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
        public int Type;
        public string Snd;
        public float FromBeat => From[0] + From[1] * 1.0f / From[2];

        public float ToBeat => To[0] + To[1] * 1.0f / To[2];
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

    /// <summary>
    /// 导出.milthm谱面数据
    /// </summary>
    /// <param name="Path">导出路径</param>
    public void Export(string Path)
    {
        File.WriteAllText(Path, JsonUtility.ToJson(this));
    }

    /// <summary>
    /// 决定BPM
    /// </summary>
    /// <param name="time">歌曲时间</param>
    /// <returns>根据BPM列表搜索指定BPM的序号</returns>
    public int DetermineBPM(float time)
    {
        if (BPMList.Count == 1)
        {
            return 0;
        }
        var ret = 0;
        for (var i = 0;i < BPMList.Count; i++)
        {
            if (time < BPMList[i].Start)
                break;
            ret = i;
        }
        return ret;
    }

    /// <summary>
    /// 将时间转化为BPM表示法
    /// </summary>
    /// <param name="time">歌曲时间</param>
    /// <param name="beat">拍数</param>
    /// <returns></returns>
    public int[] ConvertByBPM(float time, int beat, int bpm = -1)
    {
        if (bpm == -1)
            bpm = DetermineBPM(time);
        var BPM = BPMList[bpm];
        var beattime = 60.0f / BPM.BPM;
        var basebeat = (int)(Math.Floor((time - BPM.Start) / beattime));
        return new int[]{
            basebeat,
            (int)(Math.Round((time - BPM.Start - basebeat * beattime) / (beattime / beat))),
            beat
        };
    }

    /// <summary>
    /// 获取note的歌曲时间
    /// </summary>
    /// <param name="note">note</param>
    /// <returns>(开始时间,结束时间)</returns>
    public (float, float) ToRealTime(NoteData note)
    {
        return (BPMList[note.BPM].Start + note.FromBeat * (60.0f / BPMList[note.BPM].BPM), BPMList[note.BPM].Start + note.ToBeat * (60.0f / BPMList[note.BPM].BPM));
    }

    /// <summary>
    /// 读取谱面数据
    /// </summary>
    /// <param name="Path">谱面文件路径</param>
    /// <returns></returns>
    public static BeatmapModel Read(string Path)
    {
        return JsonUtility.FromJson<BeatmapModel>(File.ReadAllText(Path));
    }

    /// <summary>
    /// 三阶贝塞尔曲线
    /// </summary>
    public float BezierCubic(float t, float a, float b, float c, float d)
    {
        return (float)((a * Math.Pow(1 - t, 3)) + (3 * b * t * Math.Pow(1 - t, 2)) + (3 * c * (1 - t) * Math.Pow(t, 2)) + (d * Math.Pow(t, 3)));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MalodyConverter : MonoBehaviour
{
    /// <summary>
    /// 转换谱面
    /// </summary>
    /// <param name="file">.osu谱面文件</param>
    /// <param name="FlowSpeed">谱面流速</param>
    /// <returns>加载状态</returns>
    public static SongListLoader.LoadStatus Convert(string file, float FlowSpeed = 9.0f)
    {
        MalodyModel data = null;
        try
        {
            data = JsonUtility.FromJson<MalodyModel>(File.ReadAllText(file));
        }
        catch
        {
            Debug.Log("反序列化失败！");
            return SongListLoader.LoadStatus.Failed;
        }
        if (data.meta.mode != 0)
        {
            Debug.Log("不支持的格式。");
            return SongListLoader.LoadStatus.NotSupported;
        }
        // 初始化谱面数据模型
        BeatmapModel model = new BeatmapModel
        {
            DifficultyValue = -1,
            Illustrator = "Unknown",
            GameSource = "Malody",
            SongLength = -1,
            FormatVersion = GameSettings.FormatVersion
        };
        // 读取元数据
        model.Beatmapper = data.meta.creator;
        model.Composer = data.meta.song.artist;
        model.Title = data.meta.song.title;
        model.PreviewTime = data.meta.preview / 1000f;
        model.Difficulty = data.meta.version;
        model.BeatmapUID = "Malody-" + data.meta.id + "-" + data.meta.song.id;
        model.IllustrationFile = data.meta.background;
        MalodyTime lstBPM = null;
        foreach(MalodyTime bpm in data.time)
        {
            BeatmapModel.BPMData b = new BeatmapModel.BPMData
            {
                BPM = bpm.bpm,
                Start = 0
            };
            if (lstBPM != null)
            {
                b.Start = model.ToRealTime(new BeatmapModel.NoteData
                {
                    BPM = model.BPMList.Count - 1,
                    From = bpm.beat,
                    To = bpm.beat
                }).Item1;
            }
            model.BPMList.Add(b);
            lstBPM = bpm;
        }
        for(int i = 1; i <= data.meta.mode_ext.column; i++)
        {
            model.LineList.Add(new BeatmapModel.LineData
            {
                Direction = BeatmapModel.LineDirection.Up,
                FlowSpeed = FlowSpeed
            });
        }
        for(int i = 0; i < data.note.Count - 1; i++)
        {
            MalodyNote note = data.note[i];
            model.NoteList.Add(new BeatmapModel.NoteData
            {
                BPM = 0,
                From = note.beat,
                Snd = note.sound,
                Line = note.column,
                To = (note.endbeat == null ? note.beat : note.endbeat)
            });
        }
        model.AudioFile = data.note[^1].sound;
        model.SongOffset = data.note[^1].offset / 1000f;

        // 导出为.milthm谱面文件
        model.Export(Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".milthm");

        return SongListLoader.LoadStatus.Success;
    }
}

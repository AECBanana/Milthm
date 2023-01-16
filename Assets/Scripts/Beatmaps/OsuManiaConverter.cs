using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Osu!Mania 谱面转换器
/// 侵删
/// </summary>
public class OsuManiaConverter
{
    /// <summary>
    /// 转换谱面
    /// </summary>
    /// <param name="file">.osu谱面文件</param>
    /// <param name="FlowSpeed">谱面流速</param>
    /// <returns>加载状态</returns>
    public static SongListLoader.LoadStatus Convert(string file, float FlowSpeed = 9.0f)
    {
        string[] data = File.ReadAllText(file).Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        bool start = false;
        // 初始化谱面数据模型
        BeatmapModel model = new BeatmapModel
        {
            DifficultyValue = -1,
            Illustrator = "Unknown",
            GameSource = "Osu!Mania",
            SongLength = -1,
        };
        // 读取元数据
        bool readBg = false;
        int lineCount = 4;
        foreach (string line in data)
        {
            // 读取到击打物件时停止
            if (line == "[HitObjects]")
                break;
            // 标题
            if (line.StartsWith("TitleUnicode:"))
                model.Title = line.Split(':')[1];
            // 曲师
            if (line.StartsWith("ArtistUnicode:"))
                model.Composer = line.Split(':')[1];
            // 谱师
            if (line.StartsWith("Creator:"))
                model.Beatmapper = line.Split(':')[1];
            // 来源
            if (line.StartsWith("Source:"))
                model.Source = line.Split(':')[1];
            // 难度标题
            if (line.StartsWith("Version:"))
                model.Difficulty = line.Split(':')[1];
            // 音频文件名
            if (line.StartsWith("AudioFilename:"))
                model.AudioFile = line.Split(':')[1].Trim();
            // 谱面ID
            if (line.StartsWith("BeatmapID:"))
                model.BeatmapUID = "Osu!Mania-" + line.Split(':')[1];
            // 预览音乐开始时间
            if (line.StartsWith("PreviewTime:"))
                model.PreviewTime = float.Parse(line.Split(':')[1].Trim()) / 1000f;
            // 轨道数量
            if (line.StartsWith("CircleSize"))
                lineCount = int.Parse(line.Split(':')[1].Trim());
            // 等到进入事件区域再读取
            if (line == "[Events]")
            {
                readBg = true;
                continue;
            }
            // 读取背景图片文件名
            if (!line.StartsWith("//") && readBg && !line.StartsWith("Video"))
            {
                readBg = false;
                model.IllustrationFile = line.Split('"')[1];
            }
            // 检查是否为Osu!Mania谱面
            if (line.StartsWith("Mode:"))
            {
                if (line.Split(':')[1].Trim() != "3")
                {
                    return SongListLoader.LoadStatus.NotSupported;
                }
            }
        }
        // 添加BPM，由于Osu!谱面BPM未知，使用600BPM尽量表示所有音符的时间
        model.BPMList.Add(new BeatmapModel.BPMData
        {
            BPM = 600.0f,
            From = 0f,
            To = 0
        });
        // 根据轨道数创建轨道
        for(int i = 0;i < lineCount; i++)
        {
            model.LineList.Add(new BeatmapModel.LineData
            {
                Direction = BeatmapModel.LineDirection.Up,
                FlowSpeed = FlowSpeed
            });
        }
        // 读取所有X坐标
        List<int> xs = new List<int>();
        foreach (string line in data)
        {
            if (start)
            {
                string[] t = line.Split(',');
                if (t.Length == 6)
                {
                    int l = int.Parse(t[0]);
                    if (!xs.Contains(l))
                        xs.Add(l);
                }
                else
                {
                    continue;
                }
            }
            if (line == "[HitObjects]") start = true;
        }
        // 进行排列以从X坐标转换到轨道序号
        xs.Sort((x,y) => x.CompareTo(y));
        start = false;
        // 读取notes
        foreach (string line in data)
        {
            if (start)
            {
                string[] t = line.Split(',');
                float from = float.Parse(t[2]) / 1000, to;
                if (t.Length == 6)
                {
                    int l = xs.FindIndex(x => x == int.Parse(t[0]));
                    to = float.Parse(t[5].Split(':')[0]) / 1000;
                    if (to == 0)
                        to = from;
                    model.NoteList.Add(new BeatmapModel.NoteData
                    {
                        BPM = 0,
                        Line = l,
                        From = model.ConvertByBPM(from, 100),
                        To = model.ConvertByBPM(to, 100)
                    });
                }
                else
                {
                    continue;
                }
            }
            if (line == "[HitObjects]") start = true;
        }

        // 导出为.milthm谱面文件
        model.Export(Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".milthm");

        return SongListLoader.LoadStatus.Success;
    }
}

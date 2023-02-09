using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Osu!Mania ����ת����
/// ��ɾ
/// </summary>
public class OsuManiaConverter
{
    /// <summary>
    /// ת������
    /// </summary>
    /// <param name="file">.osu�����ļ�</param>
    /// <param name="FlowSpeed">��������</param>
    /// <returns>����״̬</returns>
    public static SongListLoader.LoadStatus Convert(string file, float FlowSpeed = 9.0f)
    {
        string[] data = File.ReadAllText(file).Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        bool start = false;
        // ��ʼ����������ģ��
        BeatmapModel model = new BeatmapModel
        {
            DifficultyValue = -1,
            Illustrator = "Unknown",
            GameSource = "Osu!Mania",
            SongLength = -1,
            FormatVersion = GameSettings.FormatVersion
        };
        // ��ȡԪ����
        bool readBg = false;
        int lineCount = 4;
        foreach (string line in data)
        {
            // ��ȡ���������ʱֹͣ
            if (line == "[TimingPoints]")
                break;
            if (line.StartsWith("SampleSet: "))
            {
                string set = line.Split(':')[1].Trim().ToLower();
                if (set == "drum")
                    model.SndSet = "osu-drum";
                else if (set == "soft")
                    model.SndSet = "osu-soft";
                else if (set == "normal")
                    model.SndSet = "osu-normal";
            }
            // ����
            if (line.StartsWith("TitleUnicode:"))
                model.Title = line.Split(':')[1];
            // ��ʦ
            if (line.StartsWith("ArtistUnicode:"))
                model.Composer = line.Split(':')[1];
            // ��ʦ
            if (line.StartsWith("Creator:"))
                model.Beatmapper = line.Split(':')[1];
            // ��Դ
            if (line.StartsWith("Source:"))
                model.Source = line.Split(':')[1];
            // �Ѷȱ���
            if (line.StartsWith("Version:"))
                model.Difficulty = line.Split(':')[1];
            // ��Ƶ�ļ���
            if (line.StartsWith("AudioFilename:"))
                model.AudioFile = line.Split(':')[1].Trim();
            // ����ID
            if (line.StartsWith("BeatmapID:"))
                model.BeatmapUID = "Osu!Mania-" + line.Split(':')[1];
            // Ԥ�����ֿ�ʼʱ��
            if (line.StartsWith("PreviewTime:"))
                model.PreviewTime = float.Parse(line.Split(':')[1].Trim()) / 1000f;
            // �������
            if (line.StartsWith("CircleSize"))
                lineCount = int.Parse(line.Split(':')[1].Trim());
            // �ȵ������¼������ٶ�ȡ
            if (line == "[Events]")
            {
                readBg = true;
                continue;
            }
            if (line.StartsWith("[") && readBg)
            {
                readBg = false;
                continue;
            }
            // ��ȡ����ͼƬ�ļ���
            if (!line.StartsWith("//") && readBg && !line.StartsWith("Video"))
            {
                readBg = false;
                model.IllustrationFile = line.Split('"')[1];
            }
            // ����Ƿ�ΪOsu!Mania����
            if (line.StartsWith("Mode:"))
            {
                if (line.Split(':')[1].Trim() != "3")
                {
                    return SongListLoader.LoadStatus.NotSupported;
                }
            }
        }
        float lstBPM = 0f;
        bool timingpoint = false;
        foreach (string line in data)
        {
            // ��ȡ���������ʱֹͣ
            if (line.StartsWith("[") && timingpoint)
                break;
            if (line == "[TimingPoints]")
            {
                timingpoint = true;
            }
            if (timingpoint)
            {
                // 1019,480,4,2,1,100,1,0
                string[] t = line.Split(',');
                if (t.Length > 2)
                {
                    BeatmapModel.BPMData bpm = new BeatmapModel.BPMData
                    {
                        BPM = float.Parse(t[1]),
                        Start = float.Parse(t[0]) / 1000f,
                    };
                    if (bpm.BPM < 0)
                        bpm.BPM = lstBPM * (-1f) * bpm.BPM / 100f;
                    else
                        lstBPM = bpm.BPM;
                    model.BPMList.Add(bpm);
                }
            }
        }
        // ���ݹ�����������
        for(int i = 0;i < lineCount; i++)
        {
            model.LineList.Add(new BeatmapModel.LineData
            {
                Direction = BeatmapModel.LineDirection.Up,
                FlowSpeed = FlowSpeed
            });
        }
        // ��ȡ����X����
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
        // ���������Դ�X����ת����������
        xs.Sort((x,y) => x.CompareTo(y));
        Debug.Log("�������" + model.LineList.Count + ", ʵ�������������" + xs.Count);
        start = false;
        // ��ȡnotes
        foreach (string line in data)
        {
            if (start)
            {
                string[] t = line.Split(',');
                float from = float.Parse(t[2]) / 1000, to;
                if (t.Length == 6)
                {
                    int l = Mathf.RoundToInt((int.Parse(t[0]) - xs[0]) * 1.0f / (xs[^1] - xs[0]) * (model.LineList.Count - 1f));
                    to = float.Parse(t[5].Split(':')[0]) / 1000;
                    if (to == 0 || to <= from)
                        to = from;
                    int bpm = model.DetermineBPM(from);
                    model.NoteList.Add(new BeatmapModel.NoteData
                    {
                        BPM = bpm,
                        Line = l,
                        From = model.ConvertByBPM(from, 16, bpm),
                        To = model.ConvertByBPM(to, 16, bpm),
                        Snd = t[^1].Split(':')[^1]
                    });
                }
                else
                {
                    continue;
                }
            }
            if (line == "[HitObjects]") start = true;
        }

        // ����Ϊ.milthm�����ļ�
        model.Export(Path.GetDirectoryName(file) + "/" + Path.GetFileNameWithoutExtension(file) + ".milthm");

        return SongListLoader.LoadStatus.Success;
    }
}

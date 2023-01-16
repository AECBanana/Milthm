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
        };
        // ��ȡԪ����
        bool readBg = false;
        int lineCount = 4;
        foreach (string line in data)
        {
            // ��ȡ���������ʱֹͣ
            if (line == "[HitObjects]")
                break;
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
        // ���BPM������Osu!����BPMδ֪��ʹ��600BPM������ʾ����������ʱ��
        model.BPMList.Add(new BeatmapModel.BPMData
        {
            BPM = 600.0f,
            From = 0f,
            To = 0
        });
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

        // ����Ϊ.milthm�����ļ�
        model.Export(Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + ".milthm");

        return SongListLoader.LoadStatus.Success;
    }
}

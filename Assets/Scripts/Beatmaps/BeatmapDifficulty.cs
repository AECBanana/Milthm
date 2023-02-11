using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatmapDifficulty : MonoBehaviour
{
    public static double Caculate(string uid, BeatmapModel map)
    {
        double ret = 0, buff = 0;
        float spanBeat = 0f;
        int ccnt = 0, bcnt = 0, scnt = 0;
        for(int i = 0; i < map.NoteList.Count - 1; i++)
        {
            BeatmapModel.NoteData note = map.NoteList[i], next = map.NoteList[i + 1];
            if (note.FromBeat == next.FromBeat)
                scnt++;
            if (note.FromBeat == note.ToBeat && next.FromBeat == next.ToBeat)
            {
                float span = (next.FromBeat - note.FromBeat);
                if (span != 0)
                {
                    bool cc = false, bc = false;
                    if (span == spanBeat)
                    {
                        cc = (note.Line == next.Line);
                        bc = true;
                    }
                    if (cc)
                        ccnt++;
                    if (bc)
                        bcnt++;
                    if (!cc)
                    {
                        double a = 1 / span;
                        double b = (ccnt <= 6) ? 0.9f : 1.1f;
                        buff += (1 + a / 16) * b * (ccnt * 1.0 / map.NoteList.Count);
                        // 1 / x = span，x = 1 / span
                        //Debug.Log("发现一个" + a + "分" + (ccnt <= 6 ? "短纵" : "长纵") + "。");
                        ccnt = 0;
                    }
                    if (!bc)
                    {
                        double a = 1 / span;
                        double b = (bcnt <= 6) ? 0.9f : 1.1f;
                        // 1 / x = span，x = 1 / span
                        buff += (0.5 + a / 16) * b * (bcnt * 1.0 / map.NoteList.Count);
                        //Debug.Log("发现一个" + a + "分" + (bcnt <= 6 ? "短交互" : "长交互") + "。");
                        bcnt = 0;
                    }
                    spanBeat = span;
                }
            }
        }
        double bpm = 0;
        foreach (BeatmapModel.BPMData bs in map.BPMList)
            bpm += bs.BPM;
        bpm /= map.BPMList.Count;
        Debug.Log("平均BPM：" + bpm);
        // 物件密度
        double p = (map.NoteList.Count - scnt) * 1.0 / (map.ToRealTime(map.NoteList[^1]).Item1 - map.ToRealTime(map.NoteList[0]).Item1);
        Debug.Log("物量平均密度：" + p + "/s, 配置加成：" + buff);

        ret = Math.Pow(p / 5, 1.3) * (1 + buff) * 10 * Math.Pow(bpm / 300, 0.5);
        //Debug.Log("定数：" + ret);
        return ret;
    }
}

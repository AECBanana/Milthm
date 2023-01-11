using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBeatmapGenerator : MonoBehaviour
{
    public AudioSource Audio;
    BeatmapModel model;
    bool loaded = false;
    private void Awake()
    {
        model = new BeatmapModel
        {
            Composer = "Zris",
            Beatmapper = "Buger404",
            Difficulty = "5.9"
        };
        model.BPMList.Add(new BeatmapModel.BPMData
        {
            BPM = 94.97f,
            From = 0f,
            To = 129f
        });
        model.LineList.Add(new BeatmapModel.LineData
        {
            Direction = BeatmapModel.LineDirection.Up,
            FlowSpeed = 5.0f
        });
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            model.NoteList.Add(new BeatmapModel.NoteData
            {
                Line = 0,
                From = model.ConvertByBPM(Audio.time, 64),
                To = model.ConvertByBPM(Audio.time, 64),
                BPM = model.DetermineBPM(Audio.time)
            });
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            model.NoteList.Add(new BeatmapModel.NoteData
            {
                Line = 0,
                From = model.ConvertByBPM(Audio.time, 64),
                BPM = model.DetermineBPM(Audio.time)
            });
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            model.NoteList[model.NoteList.Count - 1].To = model.ConvertByBPM(Audio.time, 64);
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            model.Export("D:\\test.json");
        }
        if (Input.GetKeyUp(KeyCode.L) && !loaded)
        {
            model = BeatmapModel.Read("D:\\test.json");
            GetComponent<BeatmapLoader>().Load(model);
            loaded = true;
        }
    }
}

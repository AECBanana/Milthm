using B83.Win32;
using System;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using System.Windows.Forms;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using UnityEngine.Device;
#if UNITY_ANDROID
using NativeFilePickerNamespace;
#endif

public class BeatmapImporter : MonoBehaviour
{

#if UNITY_ANDROID
    void Click()
    {
        NativeFilePicker.PickFile((data) =>
        {
            ImportBeatmap(data);
        }, "*/*");
    }
#else
    void OnEnable()
    {
        // must be installed on the main thread to get the right thread id.
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }
    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }
    void OnDestroy()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    void OnFiles(List<string> files, POINT pos)
    {
        foreach (string file in files)
            ImportBeatmap(file);
    }
    void Click()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Title = "导入谱面到Milthm";
        dialog.Filter = "Milthm谱面|*.mlt|Osu!谱面|*.osz|Malody谱面|*.mcz";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            ImportBeatmap(dialog.FileName);
        }
    }
#endif
    void ImportBeatmap(string file)
    {
        SongListLoader.LoadStatus status = SongListLoader.LoadStatus.Failed;
        string path = SongResources.DataPath + "/" + Guid.NewGuid().ToString();

        while (Directory.Exists(path))
            path = SongResources.DataPath + "/" + Guid.NewGuid().ToString();

        try
        {

            ZipFile.ExtractToDirectory(file, path);

            foreach (string f in Directory.GetFiles(path))
            {
                if (f.ToLower().EndsWith(".mc"))
                {
                    // Malody谱面，开始转换
                    ImportMalody(path);
                    Debug.Log("转换成功！");
                    break;
                }
                else if (f.ToLower().EndsWith(".osu"))
                {
                    string mode = File.ReadAllText(f).Split("[General]")[1].Split("[Editor]")[0].Split("Mode:")[1].Split('\n')[0].Replace("\r", "").Trim();
                    if (mode == "3")
                    {
                        // Osu!Mania谱面，开始转换
                        ImportOsuMania(path);
                        Debug.Log("转换成功！");
                        break;
                    }
                }
                else if (f.ToLower().EndsWith(".milthm"))
                {
                    // Milthm谱面，无需转换
                    break;
                }
            }

            if (SongListLoader.Instance == null)
                status = SongListLoader.Load(path, null);
            else
                status = SongListLoader.Load(path, SongListLoader.Instance.LoadToUIWithAni);
        }
        catch(Exception err)
        {
            Debug.Log("导入谱面时发生错误：" + err.Message + "\n" + err.StackTrace);
            status = SongListLoader.LoadStatus.Failed;
        }

        if (status != SongListLoader.LoadStatus.Success)
        {
            Directory.Delete(path, true);
        }

        if (status == SongListLoader.LoadStatus.NotSupported)
            DialogController.Show("导入谱面失败!", "已识别谱面类型，但暂不支持转换。");
        else if (status == SongListLoader.LoadStatus.Duplicated)
            DialogController.Show("重复的谱面", "你已经导入过这个谱面了哦~");
        else if (status == SongListLoader.LoadStatus.Failed)
            DialogController.Show("导入谱面失败!", "不支持的谱面类型，无法导入。");
    }

    void ImportOsuMania(string path)
    {
        foreach (string f in Directory.GetFiles(path))
        {
            if (f.ToLower().EndsWith(".osu"))
            {
                OsuManiaConverter.Convert(f);
                File.Delete(f);
            }
        }
    }

    void ImportMalody(string path)
    {
        foreach (string f in Directory.GetFiles(path))
        {
            if (f.ToLower().EndsWith(".mc"))
            {
                MalodyConverter.Convert(f);
                File.Delete(f);
            }
        }
    }

}

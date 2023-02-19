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
using UnityEngine.Android;
using UnityEngine.Device;
using Application = UnityEngine.Application;
#if UNITY_ANDROID
using NativeFilePickerNamespace;
#endif

public class BeatmapImporter : MonoBehaviour
{

#if UNITY_ANDROID
    public void Click()
    {
        NativeFilePicker.PickFile(ImportBeatmap, "*/*");
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
    public void Click()
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

        if (Application.platform == RuntimePlatform.Android)
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
                Permission.RequestUserPermission(Permission.ExternalStorageRead);
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
        
        try
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string zipFile = SongResources.DataPath + "/tmp.zip";
            File.Copy(file,zipFile, true);
            ZipFile.ExtractToDirectory(zipFile, path, true);

            if (Directory.GetDirectories(path).Length == 1 && Directory.GetFiles(path).Length == 0)
            {
                // 可能是.mcz谱面。。。
                foreach (string f in Directory.GetFiles(Directory.GetDirectories(path)[0]))
                {
                    File.Copy(f, Path.Combine(path, Path.GetFileName(f)));
                    File.Delete(f);
                }
            }

            foreach (string f in Directory.GetFiles(path))
            {
                if (f.ToLower().EndsWith(".mc"))
                {
                    // Malody谱面，开始转换
                    ImportMalody(path);
                    Debug.Log("转换成功！");
                    break;
                }

                if (f.ToLower().EndsWith(".osu"))
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
            
            File.Delete(zipFile);
        }
        catch(Exception err)
        {
            File.AppendAllText(Application.persistentDataPath + "/import.txt", err.Message + "\n" + err.StackTrace + "\n");
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
        int success = 0;
        foreach (string f in Directory.GetFiles(path))
        {
            if (f.ToLower().EndsWith(".mc"))
            {
                if (MalodyConverter.Convert(f) == SongListLoader.LoadStatus.Success)
                    success++;
                File.Delete(f);
            }
        }
        if (success == 0)
            DialogController.Show("导入谱面失败!", "已识别谱面类型，但暂不支持转换，仅支持Malody 多k谱面。");
    }

}

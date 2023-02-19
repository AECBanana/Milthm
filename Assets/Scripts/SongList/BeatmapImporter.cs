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
        dialog.Title = "�������浽Milthm";
        dialog.Filter = "Milthm����|*.mlt|Osu!����|*.osz|Malody����|*.mcz";
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
                // ������.mcz���档����
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
                    // Malody���棬��ʼת��
                    ImportMalody(path);
                    Debug.Log("ת���ɹ���");
                    break;
                }

                if (f.ToLower().EndsWith(".osu"))
                {
                    string mode = File.ReadAllText(f).Split("[General]")[1].Split("[Editor]")[0].Split("Mode:")[1].Split('\n')[0].Replace("\r", "").Trim();
                    if (mode == "3")
                    {
                        // Osu!Mania���棬��ʼת��
                        ImportOsuMania(path);
                        Debug.Log("ת���ɹ���");
                        break;
                    }
                }
                else if (f.ToLower().EndsWith(".milthm"))
                {
                    // Milthm���棬����ת��
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
            DialogController.Show("��������ʧ��!", "��ʶ���������ͣ����ݲ�֧��ת����");
        else if (status == SongListLoader.LoadStatus.Duplicated)
            DialogController.Show("�ظ�������", "���Ѿ���������������Ŷ~");
        else if (status == SongListLoader.LoadStatus.Failed)
            DialogController.Show("��������ʧ��!", "��֧�ֵ��������ͣ��޷����롣");
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
            DialogController.Show("��������ʧ��!", "��ʶ���������ͣ����ݲ�֧��ת������֧��Malody ��k���档");
    }

}

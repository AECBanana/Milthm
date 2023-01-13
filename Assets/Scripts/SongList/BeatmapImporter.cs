using B83.Win32;
using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using UnityEngine.Device;

public class BeatmapImporter : MonoBehaviour
{
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

    void OnFiles(List<string> files, POINT pos)
    {
        foreach (string file in files)
            ImportBeatmap(file);
    }

    void ImportBeatmap(string file)
    {
        SongListLoader.LoadStatus status = SongListLoader.LoadStatus.Failed;
        string path = SongResources.DataPath + "\\" + Guid.NewGuid().ToString();

        while (Directory.Exists(path))
            path = SongResources.DataPath + "\\" + Guid.NewGuid().ToString();

        try
        {

            ZipFile.ExtractToDirectory(file, path);

            foreach (string f in Directory.GetFiles(path))
            {
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
        }
        catch(Exception err)
        {
            Debug.Log("��������ʱ��������" + err.Message + "\n" + err.StackTrace);
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

    public void Click()
    {
        if (UnityEngine.Application.platform != RuntimePlatform.WindowsEditor)
            return;
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Title = "�������浽Milthm";
        dialog.Filter = "Milthm����|*.mlt|Osu!����|*.osz";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            ImportBeatmap(dialog.FileName);
        }
    }
}
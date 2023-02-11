using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    public static float DelayValue;
    public static SettingsController Instance;
    static bool updateing = false;
    public GameObject Line;
    public Slider FlowSpeed, Scale, BGMVolume, HitVolume;
    public Toggle ExportJudge, NoDead, NoCustomSnd, NoPerfect;
    public TMP_Dropdown Resolution, NoFSJudge;
    public TMP_InputField Delay;
    private void Awake()
    {
        if (FlowSpeed == null)
            return;
        UpdateStatus();
    }
    private void Start()
    {
        if (FlowSpeed == null)
            return;
        Instance = this;
        UpdateLine();
    }
    public void UpdateStatus()
    {
        updateing = true;
        FlowSpeed.value = PlayerPrefs.GetFloat("FlowSpeed", Application.platform == RuntimePlatform.Android ? 0.25f : 0.5f);
        BGMVolume.value = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        HitVolume.value = PlayerPrefs.GetFloat("HitVolume", 1f);
        Scale.value = PlayerPrefs.GetFloat("Scale", Application.platform == RuntimePlatform.Android ? 0.45f : 0.0f);
        DelayValue = PlayerPrefs.GetFloat("Delay", 0.0f);
        ExportJudge.isOn = bool.Parse(PlayerPrefs.GetString("ExportJudge", "False"));
        NoDead.isOn = bool.Parse(PlayerPrefs.GetString("NoDead", "False"));
        NoCustomSnd.isOn = bool.Parse(PlayerPrefs.GetString("NoCustomSnd", "False"));
        NoPerfect.isOn = bool.Parse(PlayerPrefs.GetString("NoPerfect", "False"));
        NoFSJudge.value = PlayerPrefs.GetInt("JudgeMode", Application.platform == RuntimePlatform.Android ? 1 : 0);
        Delay.text = DelayValue.ToString();
        Resolution.value = PlayerPrefs.GetInt("Resolution", 0);
        updateing = false;
    }
    public void ResetSettings()
    {
        PlayerPrefs.SetFloat("BGMVolume", 0.5f);
        PlayerPrefs.SetFloat("HitVolume", 1f);
        PlayerPrefs.SetFloat("Delay", 0.0f);
        PlayerPrefs.SetFloat("FlowSpeed", Application.platform == RuntimePlatform.Android ? 0.25f : 0.5f);
        PlayerPrefs.SetFloat("Scale", Application.platform == RuntimePlatform.Android ? 0.45f : 0.0f);
        PlayerPrefs.SetString("NoDead", "False");
        PlayerPrefs.SetString("ExportJudge", "False");
        PlayerPrefs.SetString("NoCustomSnd", "False");
        PlayerPrefs.SetString("NoPerfect", "False");
        PlayerPrefs.SetInt("JudgeMode", Application.platform == RuntimePlatform.Android ? 1 : 0);
        PlayerPrefs.SetInt("Resolution", 0);
        PlayerPrefs.SetInt("JudgeRange", 1);
        Instance.UpdateStatus();
        UpdateLine();
    }
    public void ShowDelayManager()
    {
        Instantiate(Resources.Load<GameObject>("TestDelayCanvas")).SetActive(true);
    }
    public void UpdateLine()
    {
        Instance.Line.transform.localScale = new Vector3(32.4f * (1.0f + Instance.Scale.value), 32.4f * (1.0f + Instance.Scale.value), 32.4f * (1.0f + Instance.Scale.value));
        Instance.Line.GetComponent<LineController>().FlowSpeed = 9f * Mathf.Pow(0.5f + Instance.FlowSpeed.value, 1.2f);
    }
    public void SaveSettings()
    {
        if (updateing)
            return;
        if (Instance == null)
            return;
        try
        {
            DelayValue = float.Parse(Instance.Delay.text == "" ? "0" : Instance.Delay.text);
        }
        catch
        {
            DelayValue = 0;
        }
        PlayerPrefs.SetFloat("Delay", DelayValue);
        PlayerPrefs.SetFloat("FlowSpeed", Instance.FlowSpeed.value);
        PlayerPrefs.SetFloat("Scale", Instance.Scale.value);
        PlayerPrefs.SetFloat("BGMVolume", Instance.BGMVolume.value);
        PlayerPrefs.SetFloat("HitVolume", Instance.HitVolume.value);
        PlayerPrefs.SetString("ExportJudge", ExportJudge.isOn.ToString());
        PlayerPrefs.SetString("NoDead", NoDead.isOn.ToString());
        PlayerPrefs.SetString("NoCustomSnd", NoCustomSnd.isOn.ToString());
        PlayerPrefs.SetString("NoPerfect", NoPerfect.isOn.ToString());
        PlayerPrefs.SetInt("JudgeMode", NoFSJudge.value);
        PlayerPrefs.SetInt("Resolution", Resolution.value);
        foreach (VolumeSettings vs in VolumeSettings.Volumes)
            vs.Changed = true;
        if (Resolution.value == 0)
        {
            Screen.fullScreen = true;
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
        else
        {
            Screen.fullScreen = false;
            Screen.SetResolution(int.Parse(Resolution.captionText.text.Split('x')[0]), int.Parse(Resolution.captionText.text.Split('x')[1]), false);
        }
        UpdateLine();
    }
}

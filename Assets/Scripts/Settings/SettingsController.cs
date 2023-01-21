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
    public Slider FlowSpeed, Scale;
    public Toggle ExportJudge;
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
        FlowSpeed.value = PlayerPrefs.GetFloat("FlowSpeed", 0.5f);
        Scale.value = PlayerPrefs.GetFloat("Scale", 0.0f);
        DelayValue = PlayerPrefs.GetFloat("Delay", 0.0f);
        ExportJudge.isOn = bool.Parse(PlayerPrefs.GetString("ExportJudge", "False"));
        Delay.text = DelayValue.ToString();
        updateing = false;
    }
    public void ResetSettings()
    {
        PlayerPrefs.SetFloat("Delay", 0.0f);
        PlayerPrefs.SetFloat("FlowSpeed", 0.5f);
        PlayerPrefs.SetFloat("Scale", 0.0f);
        PlayerPrefs.SetString("ExportJudge", "False");
        Instance.UpdateStatus();
        UpdateLine();
    }
    public void ShowDelayManager()
    {
        Instantiate(Resources.Load<GameObject>("TestDelayCanvas")).SetActive(true);
    }
    public void UpdateLine()
    {
        Instance.Line.transform.localScale = new Vector3(0.3f * (1.0f + Instance.Scale.value), 0.3f * (1.0f + Instance.Scale.value), 0.3f * (1.0f + Instance.Scale.value));
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
        PlayerPrefs.SetString("ExportJudge", ExportJudge.isOn.ToString());
        UpdateLine();
    }
}

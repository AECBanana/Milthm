using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    public static SettingsController Instance;
    public Slider FlowSpeed, Scale;
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
    }
    public void UpdateStatus()
    {
        FlowSpeed.value = PlayerPrefs.GetFloat("FlowSpeed", 0.5f);
        Scale.value = PlayerPrefs.GetFloat("Scale", 0.5f);
        Delay.text = PlayerPrefs.GetFloat("Delay", 0.0f).ToString();
    }
    public void ResetSettings()
    {
        PlayerPrefs.SetFloat("Delay", 0.0f);
        PlayerPrefs.SetFloat("FlowSpeed", 0.5f);
        PlayerPrefs.SetFloat("Scale", 0.5f);
        Instance.UpdateStatus();
    }
    public void ShowDelayManager()
    {
        Instantiate(Resources.Load<GameObject>("TestDelayCanvas")).SetActive(true);
    }
    public void SaveSettings()
    {
        if (Instance == null)
            return;
        PlayerPrefs.SetFloat("Delay", float.Parse(Instance.Delay.text == "" ? "0" : Instance.Delay.text));
        PlayerPrefs.SetFloat("FlowSpeed", Instance.FlowSpeed.value);
        PlayerPrefs.SetFloat("Scale", Instance.Scale.value);
    }
}

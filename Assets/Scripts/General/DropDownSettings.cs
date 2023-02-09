using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropDownSettings : MonoBehaviour
{
    public string SettingName;
    public int DefaultValue;
    TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.value = PlayerPrefs.GetInt(SettingName, DefaultValue);
    }
    public void Save()
    {
        PlayerPrefs.SetInt(SettingName, dropdown.value);
    }
}

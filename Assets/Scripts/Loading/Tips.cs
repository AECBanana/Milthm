using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tips : MonoBehaviour
{
    public TextAsset tipText;
    public TMP_Text text;

    private void Awake()
    {
        var tips = tipText.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        text.text = "Tips: " + tips[Random.Range(0, tips.Length)];
    }
}

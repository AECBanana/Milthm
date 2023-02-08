using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TouchDetail : MonoBehaviour
{
    public string Title;
    TMP_Text text;
    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }
    public void Click()
    {
        DialogController.Show(Title, text.text);
    }
}

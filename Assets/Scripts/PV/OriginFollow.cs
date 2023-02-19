using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriginFollow : MonoBehaviour
{
    public RectTransform text, text2;
    private RectTransform me;
    
    private void Awake()
    {
        me = GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector3 pos = me.localPosition;
        pos.x = Math.Max(text.localPosition.x + text.sizeDelta.x + 2f, text2.localPosition.x + text2.sizeDelta.x + 20f);
        me.localPosition = pos;
    }
}

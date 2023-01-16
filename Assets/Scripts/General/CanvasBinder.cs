using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Canvas相机绑定器
/// </summary>
[RequireComponent(typeof(Canvas))]
public class CanvasBinder : MonoBehaviour
{
    // 自动绑定主相机
    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Canvas�������
/// </summary>
[RequireComponent(typeof(Canvas))]
public class CanvasBinder : MonoBehaviour
{
    // �Զ��������
    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}

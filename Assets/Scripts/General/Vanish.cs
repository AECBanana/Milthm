using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 销毁/隐藏通用工具类
/// </summary>
public class Vanish : MonoBehaviour
{
    public void PauseAni()
    {
        GetComponent<Animator>().SetFloat("Speed", 0.0f);
    }
    public void AniCallback()
    {
        Destroy(gameObject);
    }
    public void Delete()
        => AniCallback();
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void QuitCanvas()
    {
        GetComponent<Animator>().Play("CanvasQuit", 0, 0.0f);
    }
    public void HideCanvas()
    {
        GetComponent<Animator>().Play("CanvasHide", 0, 0.0f);
    }
}

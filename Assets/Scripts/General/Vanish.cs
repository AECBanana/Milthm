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
        if (TryGetComponent<Canvas>(out _) && gameObject.transform.parent != null)
            Destroy(gameObject.transform.parent.gameObject);
        else
            Destroy(gameObject);
    }
    public void Delete()
    {
        Destroy(gameObject);
    }
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

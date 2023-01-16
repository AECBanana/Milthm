using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 消息框控制器
/// </summary>
public class DialogController : MonoBehaviour
{
    public Text Title, Content;
    bool closed = false;
    /// <summary>
    /// 销毁消息框（播放动画）
    /// </summary>
    public void HideCanvas()
    {
        if (closed)
            return;
        closed = true;
        GetComponent<Animator>().Play("CanvasQuit", 0, 0.0f);
    }

    public static GameObject prefab;
    /// <summary>
    /// 弹出新的消息框
    /// </summary>
    /// <param name="Title">标题</param>
    /// <param name="Content">内容</param>
    public static void Show(string Title, string Content)
    {
        if (prefab == null)
            prefab = Resources.Load<GameObject>("DialogCanvas");
        GameObject go = Instantiate(prefab);
        DialogController dialog = go.GetComponent<DialogController>();
        dialog.Title.text = Title;
        dialog.Content.text = Content;
        go.SetActive(true);
    }
}

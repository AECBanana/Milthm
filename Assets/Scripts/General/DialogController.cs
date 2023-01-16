using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��Ϣ�������
/// </summary>
public class DialogController : MonoBehaviour
{
    public Text Title, Content;
    bool closed = false;
    /// <summary>
    /// ������Ϣ�򣨲��Ŷ�����
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
    /// �����µ���Ϣ��
    /// </summary>
    /// <param name="Title">����</param>
    /// <param name="Content">����</param>
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

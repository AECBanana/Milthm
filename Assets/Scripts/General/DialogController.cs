using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public Text Title, Content;
    bool closed = false;
    public void HideCanvas()
    {
        if (closed)
            return;
        closed = true;
        GetComponent<Animator>().Play("CanvasQuit", 0, 0.0f);
    }

    public static GameObject prefab;
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

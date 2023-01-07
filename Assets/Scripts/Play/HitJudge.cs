using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitJudge : MonoBehaviour
{
    static GameObject Perfect, Good, Miss;
    public static Animator Judge(Transform AniParent)
    {
        if (Perfect == null)
            Perfect = Resources.Load<GameObject>("Perfect");
        if (Good == null)
            Good = Resources.Load<GameObject>("Good");
        if (Miss == null)
            Miss = Resources.Load<GameObject>("Miss");
        GameObject go = Instantiate(Perfect, AniParent);
        go.transform.localPosition = new Vector3(4.1f, 0, 0);
        go.SetActive(true);
        return go.GetComponent<Animator>();
    }
}

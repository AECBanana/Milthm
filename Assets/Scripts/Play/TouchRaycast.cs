using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchRaycast : MonoBehaviour
{
    public static TouchRaycast Instance;
    Dictionary<GameObject, LineController> GoLine = new Dictionary<GameObject, LineController>();
    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        Instance = this;
    }

    public void Register(GameObject go, LineController line)
        => GoLine.Add(go, line);

    public void Revoke(GameObject go)
        => GoLine.Remove(go);

    void Update()
    {
        if (HitJudge.JudgeMode != 1)
            return;

        for (int i = 0; i < Input.touchCount; i++)
        {
            foreach (RaycastHit2D hit in Physics2D.RaycastAll(cam.ScreenToWorldPoint(Input.touches[i].position), Vector2.zero))
            {
                GameObject go = hit.collider.gameObject;
                if (GoLine.ContainsKey(go))
                {
                    if (Input.touches[i].phase == TouchPhase.Ended || Input.touches[i].phase == TouchPhase.Canceled)
                    {
                        GoLine[go].TouchUp();
                    }
                    else
                    {
                        GoLine[go].TouchDown();
                        GoLine[go].FirstHold = (Input.touches[i].phase == TouchPhase.Began);
                    }
                }
            }
        }
    }
}

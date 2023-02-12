using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchRaycast : MonoBehaviour
{
    public static TouchRaycast Instance;
    private Dictionary<GameObject, LineController> GoLine = new Dictionary<GameObject, LineController>();
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        Instance = this;
    }

    public void Register(GameObject go, LineController line)
        => GoLine.Add(go, line);

    public void Revoke(GameObject go)
        => GoLine.Remove(go);

    private void Update()
    {
        if (HitJudge.JudgeMode != 1)
            return;

        for (var i = 0; i < Input.touchCount; i++)
        {
            foreach (var hit in Physics2D.RaycastAll(cam.ScreenToWorldPoint(Input.touches[i].position), Vector2.zero))
            {
                var go = hit.collider.gameObject;
                if (!GoLine.ContainsKey(go)) continue;
                if (Input.touches[i].phase is TouchPhase.Ended or TouchPhase.Canceled)
                {
                    GoLine[go].Holding = false;
                }
                else
                {
                    GoLine[go].Holding = true;
                    GoLine[go].FirstHold = (Input.touches[i].phase == TouchPhase.Began);
                }
            }
        }
    }
}

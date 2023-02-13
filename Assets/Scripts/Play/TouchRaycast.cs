using System;
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

        foreach (var line in GoLine.Values)
        {
            line.Holding = false;
            line.FirstHold = false;
        }
        
        for (var i = 0; i < Input.touchCount; i++)
        {
            Vector2 pos = cam.ScreenToWorldPoint(Input.touches[i].position);
            var min = float.MaxValue;
            LineController line = null;
            foreach (var hit in Physics2D.RaycastAll(pos, Vector2.zero))
            {
                var go = hit.collider.gameObject;
                if (!GoLine.ContainsKey(go)) continue;
                if (Input.touches[i].phase is not (TouchPhase.Ended or TouchPhase.Canceled))
                {
                    var delta = Mathf.Abs(pos.x - go.transform.position.x);
                    if (delta < min)
                    {
                        min = delta;
                        line = GoLine[go];
                    }
                }
            }
            if (line != null)
            {
                line.Holding = true;
                line.FirstHold |= (Input.touches[i].phase == TouchPhase.Began);
            }
        }
    }
}

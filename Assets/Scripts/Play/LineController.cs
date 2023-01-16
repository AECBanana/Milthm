using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public static List<LineController> Lines = new List<LineController>();
    public const float MoveArea = 50f;
    public float FlowSpeed;
    public int Index;
    public BeatmapModel.LineDirection Direction;
    public List<MonoBehaviour> HitObjects = new List<MonoBehaviour>();
    public Transform Line;
    bool hide = false;

    private void Awake()
    {
        Lines.Add(this);
    }
    private void OnDestroy()
    {
        Lines.Remove(this);
    }
    private void Update()
    {
        if (HitJudge.Result.Dead)
        {
            if (HitJudge.Result.DeadTime == DateTime.MinValue)
            {
                Destroy(gameObject);
                return;
            }
            float pass = (float)(DateTime.Now - HitJudge.Result.DeadTime).TotalSeconds / 1f;
            if (pass > 1f) pass = 1f;
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - pass);
            Line.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - pass);
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - pass);
        }
        if (transform.childCount == 1 && !hide)
        {
            hide = true;
            GetComponent<Animator>().Play("LineHide", 0, 0.0f);
        }
        for (int i = 0; i < HitObjects.Count; i++)
        {
            if (i >= HitObjects.Count) break;
            if (!HitObjects[i].gameObject.activeSelf)
            {
                float x = 0;
                if (HitObjects[i] is TapController tap)
                {
                    x = (tap.Time - AudioUpdate.Time) * FlowSpeed * 5;
                }
                else if (HitObjects[i] is HoldController hold)
                {
                    x = (hold.From - AudioUpdate.Time) * FlowSpeed * 5 - 1.66f;
                }
                if (x <= MoveArea)
                {
                    HitObjects[i].gameObject.SetActive(true);
                    HitObjects.RemoveAt(i);
                    i--;
                }
                else
                {
                    return;
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public const float MoveArea = 50f;
    public float FlowSpeed;
    public KeyCode KeyOverride = KeyCode.None;
    public BeatmapModel.LineDirection Direction;
    public List<MonoBehaviour> HitObjects = new List<MonoBehaviour>();
    public List<(SpriteRenderer, SpriteRenderer)> ObjectRenders = new List<(SpriteRenderer, SpriteRenderer)>();
    public Transform Line, KeyTip;
    bool hide = false;
    public KeyCode Key
    {
        get
        {
            if (KeyOverride != KeyCode.None)
            {
                return KeyOverride;
            }
            else
            {
                if (Direction == BeatmapModel.LineDirection.Left)
                    return KeyCode.LeftArrow;
                else if (Direction == BeatmapModel.LineDirection.Right)
                    return KeyCode.RightArrow;
                else if (Direction == BeatmapModel.LineDirection.Up)
                    return KeyCode.UpArrow;
                else if (Direction == BeatmapModel.LineDirection.Down)
                    return KeyCode.DownArrow;
            }
            return KeyCode.None;
        }
    }


    private void Update()
    {
        if (KeyTip.localEulerAngles.z != -1 * transform.localEulerAngles.z)
            KeyTip.localEulerAngles = new Vector3(0, 0, -1 * transform.localEulerAngles.z);
        if (HitJudge.Result.Dead)
        {
            if (HitJudge.Result.DeadTime.Year == 0)
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
        if (transform.childCount == 2 && !hide)
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

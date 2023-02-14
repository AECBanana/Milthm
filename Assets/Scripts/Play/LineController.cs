using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ¹ìµÀ¿ØÖÆÆ÷
/// </summary>
public class LineController : MonoBehaviour
{
    public static List<LineController> Lines = new List<LineController>();
    public static List<LineController> UnhitLines = new List<LineController>();
    public Transform JudgePoint;
    public const float MoveArea = 70f;
    public float FlowSpeed;
    public int Index;
    public bool Holding = false;
    public bool FirstHold = false;
    public BeatmapModel.LineDirection Direction;
    public int RemainingNote = 0;
    public List<MonoBehaviour> HitObjects = new List<MonoBehaviour>();
    public List<MonoBehaviour> ShowedObjects = new List<MonoBehaviour>();
    public Transform Line;
    private SpriteRenderer lineRender;
    bool hide = false;

    private void Awake()
    {
        if (!(HitJudge.JudgeMode == 1 && Application.platform == RuntimePlatform.Android))
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        UnhitLines.Add(this);
        lineRender = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        if (TouchRaycast.Instance != null)
            TouchRaycast.Instance.Register(gameObject, this);
    }
    private void OnDestroy()
    {
        UnhitLines.Remove(this);
        if (TouchRaycast.Instance != null)
            TouchRaycast.Instance.Revoke(gameObject);
    }
    private void LateUpdate()
    {
        if (HitJudge.Result.Dead && !AudioUpdate.Instance.PreviewMode)
        {
            if (HitJudge.Result.DeadTime == DateTime.MinValue)
            {
                Destroy(gameObject);
                return;
            }
            float pass = (float)(DateTime.Now - HitJudge.Result.DeadTime).TotalSeconds / 1f;
            if (pass > 1f) pass = 1f;
            lineRender.color = new Color(1f, 1f, 1f, 1f - pass);
            Line.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - pass);
            transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f - pass);
        }
        /**if (FirstHold)
        {
            lineRender.color = Color.red;
        }
        else if (Holding)
        {
            lineRender.color = Color.green;
        }
        else
        {
            lineRender.color = Color.white;
        }**/
        if (RemainingNote == 0 && !hide)
        {
            if (AudioUpdate.Instance.PreviewMode)
            {
                if (AudioUpdate.Time < 0.5f)
                {
                    RemainingNote = ShowedObjects.Count;
                    foreach (var note in ShowedObjects)
                    {
                        HitObjects.Add(note);
                        note.gameObject.SetActive(false);
                    }
                    ShowedObjects.Clear();
                }
            }
            else
            {
                hide = true;
                GetComponent<Animator>().Play("LineHide", 0, 0.0f);
            }
        }

        float realFlowSpeed = FlowSpeed * BeatmapLoader.FlowSpeed *
                              GamePlayLoops.FlowSpeedFactor * 5;
        for (int i = 0; i < HitObjects.Count; i++)
        {
            if (i >= HitObjects.Count) break;
            if (HitObjects[i].gameObject.activeSelf) continue;
            float x = HitObjects[i] switch
            {
                TapController tap => (tap.From - AudioUpdate.Time) * realFlowSpeed,
                HoldController hold => (hold.From - AudioUpdate.Time) * realFlowSpeed - 1.66f,
                _ => 0
            };
            if (x <= MoveArea)
            {
                HitObjects[i].gameObject.SetActive(true);
                if (AudioUpdate.Instance.PreviewMode)
                    ShowedObjects.Add(HitObjects[i]);
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

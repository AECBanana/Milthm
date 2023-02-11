using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapController : MonoBehaviour
{
    public LineController Line;
    public float Time;
    public Sprite DoubleSprite;
    public bool Hit = false;
    public string Snd;
    public KeyCode Key;
    public int Index;
    int holdKey = -1;
    bool Missed = false;
    SpriteRenderer Renderer;
    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Renderer.color = new Color(1f, 1f, 1f, 0f);
    }
    private void UpdateGraphics()
    {
        //Debug.Log("Total flowspeed: " + Line.FlowSpeed * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed);
        //Debug.Log("Line: " + Line.FlowSpeed  + ", BPM: " + GamePlayLoops.FlowSpeedFactor + ", Beatmap: " + BeatmapLoader.FlowSpeed);
        //Debug.Log("delta time:" + (Time - AudioUpdate.Time));
        float x = (Time - AudioUpdate.Time) * Line.FlowSpeed * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed * 5, y = 0;
        if (AudioUpdate.Instance.PreviewMode)
            x = (Time - AudioUpdate.Time + SettingsController.DelayValue / 1000f) * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed * Line.FlowSpeed * 5;
        if (x > LineController.MoveArea)
        {
            if (Renderer.color.a != 0)
                Renderer.color = new Color(1f, 1f, 1f, 0f);
            return;
        }
        if (Renderer.color.a == 0)
            Renderer.color = new Color(1f, 1f, 1f, 1f);
        if (HitJudge.Result.Dead)
        {
            Missed = true;
            float pass = (float)(DateTime.Now - HitJudge.Result.DeadTime).TotalSeconds;
            x -= pass * (10 + (Time - AudioUpdate.Time) * 10f);
            y -= pass * (Index % 2 == 0 ? 1 : -1);
            transform.localEulerAngles = new Vector3(0, 0, pass * 30 * (Index % 2 == 0 ? 1 : -1));
        }
        transform.localPosition = new Vector3(x, y, 0);
        if (x < 0)
        {
            float d = -1 * x / 2f;
            if (d > 1f) d = 1f;
            Renderer.color = new Color(1f, 1f, 1f, 1f - d);
        }
    }
    public void Judge()
    {
        if (AudioUpdate.Time - Time > GameSettings.Bad && !Missed)
        {
            if (HitJudge.Record)
                HitJudge.RecordLog.AppendLine("[AutoMiss-TooLate] " + Index + "(Tap) Missed <At " + Time + ">");
            HitJudge.JudgeMiss(transform.parent, this);
            Missed = true;
            Line.RemainingNote--;
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        if (!Missed && !Hit && Mathf.Abs(Time - AudioUpdate.Time) <= GameSettings.Valid)
        {
            holdKey = HitJudge.IsPress(this);
            if (holdKey != 0)
            {
                HitJudge.Judge(transform.parent, this, AudioUpdate.Time - Time, Snd, ref Missed);
                HitJudge.BindNotes[holdKey] = null;
                if (Missed)
                {
                    /**if (HitJudge.Record)
                        HitJudge.RecordLog.AppendLine("[Release] " + Index + "(Tap) released " + holdKey);
                    HitJudge.BindNotes[holdKey] = null;**/
                }
                else
                {
                    Hit = true;
                }
                Line.RemainingNote--;
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }
        if (GamePlayLoops.AutoPlay && Mathf.Abs(Time - AudioUpdate.Time) <= GameSettings.Perfect2)
        {
            HitJudge.Judge(transform.parent, this, AudioUpdate.Time - Time, Snd, ref Missed);
            Hit = true;
            Line.RemainingNote--;
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (!AudioUpdate.Instance.PreviewMode)
        {
            Judge();
        }
        else
        {
            if (Mathf.Abs(Time - AudioUpdate.Time + SettingsController.DelayValue / 1000) <= GameSettings.Perfect2)
            {
                HitJudge.PlayPerfect(transform.parent);
                Line.RemainingNote--;
                gameObject.SetActive(false);
            }
        }
        UpdateGraphics();
    }

}

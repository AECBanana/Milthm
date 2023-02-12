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
    private int holdKey = -1;
    private bool missed = false;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
    }
    private void UpdateGraphics()
    {
        float x = (Time - AudioUpdate.Time) * Line.FlowSpeed * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed * 5, y = 0;
        if (AudioUpdate.Instance.PreviewMode)
            x = (Time - AudioUpdate.Time + SettingsController.DelayValue / 1000f) * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed * Line.FlowSpeed * 5;
        if (x > LineController.MoveArea)
        {
            if (spriteRenderer.color.a != 0)
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
            return;
        }
        if (spriteRenderer.color.a == 0)
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        if (HitJudge.Result.Dead)
        {
            missed = true;
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
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f - d);
        }
    }

    private void Judge()
    {
        if (AudioUpdate.Time - Time > GameSettings.Bad && !missed)
        {
            if (HitJudge.Record)
                HitJudge.RecordLog.AppendLine("[AutoMiss-TooLate] " + Index + "(Tap) Missed <At " + Time + ">");
            HitJudge.JudgeMiss(transform.parent, this, 0);
            missed = true;
            Line.RemainingNote--;
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        if (!missed && !Hit && Mathf.Abs(Time - AudioUpdate.Time) <= GameSettings.Valid)
        {
            holdKey = HitJudge.IsPress(this);
            if (holdKey != 0)
            {
                HitJudge.Judge(transform.parent, this, AudioUpdate.Time - Time, Snd, ref missed, 0);
                HitJudge.BindNotes[holdKey] = null;
                if (!missed)
                    Hit = true;
                Line.RemainingNote--;
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }
        if (GamePlayLoops.AutoPlay && Mathf.Abs(Time - AudioUpdate.Time) <= GameSettings.Perfect2)
        {
            HitJudge.Judge(transform.parent, this, AudioUpdate.Time - Time, Snd, ref missed, 0);
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
                HitJudge.PlayPerfect(transform.parent, 0);
                Line.RemainingNote--;
                gameObject.SetActive(false);
            }
        }
        UpdateGraphics();
    }

}

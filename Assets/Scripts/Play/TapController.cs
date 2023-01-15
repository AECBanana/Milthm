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
    public KeyCode Key;
    public int Index;
    bool Missed = false;
    SpriteRenderer Renderer;
    Transform KeyTip;
    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Renderer.color = new Color(1f, 1f, 1f, 0f);
        KeyTip = transform.GetChild(0);
    }
    private void Update()
    {
        float x = (Time - AudioUpdate.Time) * Line.FlowSpeed * 5, y = 0;
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
        if (KeyTip.localEulerAngles.z != -1 * Line.transform.localEulerAngles.z)
            KeyTip.localEulerAngles = new Vector3(0, 0, -1 * Line.transform.localEulerAngles.z);
        if (AudioUpdate.Time - Time > GameSettings.Bad && !Missed)
        {
            HitJudge.JudgeMiss(transform.parent, this);
            Missed = true;
            Destroy(gameObject);
        }
        if (!Missed && Mathf.Abs(Time - AudioUpdate.Time) <= GameSettings.Valid)
        {
            KeyCode key = Key;
            if (key == KeyCode.None)
                key = Line.Key;
            if (key == KeyCode.Tab)
            {
                key = HitJudge.AnykeyPress(this);
                if (key != KeyCode.Tab)
                {
                    Line.KeyOverride = key;
                    HitJudge.ResortLineKeys();
                }
            }
            if (key != KeyCode.Tab && HitJudge.IsPress(key, this))
            {
                HitJudge.CaptureOnce.Add(key);
                HitJudge.Judge(transform.parent, this, AudioUpdate.Time - Time);
                Hit = true;
                Destroy(gameObject);
            }
        }
        if (GamePlayLoops.AutoPlay && Mathf.Abs(Time - AudioUpdate.Time) <= GameSettings.Perfect2)
        {
            HitJudge.Judge(transform.parent, this, AudioUpdate.Time - Time);
            Hit = true;
            Destroy(gameObject);
        }
        if (x < 0)
        {
            float d = -1 * x / 2f;
            if (d > 1f) d = 1f;
            Renderer.color = new Color(1f, 1f, 1f, 1f - d);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class HoldController : MonoBehaviour
{
    public LineController Line;
    public float From, To;
    public Sprite DoubleSprite;
    public bool HeadHit = false, EndHit = false;
    public Animator HitAni = null;
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
        float x = (From - AudioUpdate.Time) * Line.FlowSpeed * 5 - 1.66f,
              x2 = (To - AudioUpdate.Time) * Line.FlowSpeed * 5 + 1.66f,
              y = 0;
        if (x < -1.66f)
            x = -1.66f;
        if (x > LineController.MoveArea)
        {
            if (Renderer.color.a != 0)
                Renderer.color = new Color(1f, 1f, 1f, 0f);
            return;
        }
        if (Renderer.color.a == 0)
            Renderer.color = new Color(1f, 1f, 1f, 1f);
        float w = (x2 - x) * (Renderer.size.y / 1.66f / 2);
        if (w < Renderer.size.y) w = Renderer.size.y;
        Renderer.size = new Vector2(w, Renderer.size.y);
        if (HitJudge.Result.Dead)
        {
            Missed = true;
            float pass = (float)(DateTime.Now - HitJudge.Result.DeadTime).TotalSeconds;
            x -= pass * (10 + (From - AudioUpdate.Time) * 10f);
            y -= pass * (Index % 2 == 0 ? 1 : -1);
            transform.localEulerAngles = new Vector3(0, 0, pass * 30 * (Index % 2 == 0 ? 1 : -1));
        }
        transform.localPosition = new Vector3(x, y, 0);
        if (KeyTip.localEulerAngles.z != -1 * Line.transform.localEulerAngles.z)
            KeyTip.localEulerAngles = new Vector3(0, 0, -1 * Line.transform.localEulerAngles.z);
        KeyCode key = Key;
        if (key == KeyCode.None)
            key = Line.Key;
        if (!HeadHit)
        {
            if (AudioUpdate.Time - From > GameSettings.Bad && !Missed)
            {
                // Miss
                Missed = true;
                Renderer.color = new Color(0.8f, 0.7f, 0.7f, 0.3f);
                HitJudge.JudgeMiss(transform.parent, this);
            }
            if (!Missed && Mathf.Abs(From - AudioUpdate.Time) <= GameSettings.Valid)
            {
                if (HitJudge.IsPress(key, this, true))
                {
                    HitAni = HitJudge.Judge(transform.parent, this, AudioUpdate.Time - From);
                    if (HitAni != null)
                        HitAni.Play("HoldAni", 0, 0.0f);
                    HeadHit = true;
                }
            }
        }
        if (HeadHit && !Missed)
        {
            if (!Input.GetKey(key))
            {
                if (Mathf.Abs(To - AudioUpdate.Time) > GameSettings.HoldValid)
                {
                    // Miss
                    Missed = true;
                    Renderer.color = new Color(0.8f, 0.7f, 0.7f, 0.3f);
                    HitJudge.JudgeMiss(transform.parent, this);
                }
                EndHit = true;
                if (HitAni != null)
                    HitAni.SetFloat("Speed", 1.0f);
            }
        }
        if (w == Renderer.size.y)
        {
            float d = (AudioUpdate.Time - To) / 0.25f;
            if (d > 1f) d = 1f;
            if (!Missed)
                Renderer.color = new Color(1f, 1f, 1f, 1f - d);
            else
                Renderer.color = new Color(0.8f, 0.7f, 0.7f, 0.3f - d * 0.3f);
            if (d >= 1f)
            {
                Destroy(gameObject);
                if (HitAni != null)
                    HitAni.SetFloat("Speed", 1.0f);
            }
        }
    }
}

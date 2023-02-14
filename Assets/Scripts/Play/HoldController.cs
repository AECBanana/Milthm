using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

/// <summary>
/// ³¤Ìõ¿ØÖÆÆ÷
/// </summary>
public class HoldController : MonoBehaviour
{
    public LineController Line;
    public float From, To;
    public Sprite DoubleSprite;
    public bool HeadHit = false, EndHit = false;
    public string Snd;
    public int Index;
    int holdKey = 0;
    int failFrames = 0;
    bool Missed = false;
    SpriteRenderer Renderer;
    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Renderer.color = new Color(1f, 1f, 1f, 0f);
    }
    private void UpdateGraphics()
    {
        float x = (From - AudioUpdate.Time) * Line.FlowSpeed * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed * 5 - 1.66f,
              x2 = (To - AudioUpdate.Time) * Line.FlowSpeed * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed * 5 + 1.66f,
              y = 0;
        if (AudioUpdate.Instance.PreviewMode)
        {
            x = (From - AudioUpdate.Time + SettingsController.DelayValue / 1000f) * Line.FlowSpeed * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed * 5 - 1.66f;
            x2 = (To - AudioUpdate.Time + SettingsController.DelayValue / 1000f) * Line.FlowSpeed * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed * 5 + 1.66f;
        }
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
        if (Mods.Data[Mod.Invisible])
        {
            float p = Mathf.Clamp((x - LineController.MoveArea * 0.1f) / (LineController.MoveArea * 0.4f), 0f, 1f);
            Renderer.color = new Color(1f, 1f, 1f, p);
        }
        if (Mods.Data[Mod.Dance])
        {
            y = Mathf.Sin(Index + x / 10f) * (Mathf.Max(0f,x) / LineController.MoveArea * 20f);
        }
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
        /**if (holdKey != KeyCode.None)
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite =
                Resources.Load<Sprite>("KeySets\\" + (char)('A' + (holdKey - KeyCode.A)));**/
        transform.localPosition = new Vector3(x, y, 0);
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
                if (!HeadHit && !Missed && !AudioUpdate.Instance.PreviewMode)
                {
                    Missed = true;
                    Renderer.color = new Color(0.8f, 0.7f, 0.7f, 0.3f);
                    if (HitJudge.Record)
                        HitJudge.RecordLog.AppendLine("[AutoMiss-TooLate] " + Index + "(Hold) Missed <From " + From + " to " + To + ">");
                    HitJudge.JudgeMiss(transform.parent, this, 1);
                }
                if (holdKey != 0 && HitJudge.BindNotes[holdKey] == this)
                {
                    if (HitJudge.Record)
                        HitJudge.RecordLog.AppendLine("[Release] " + Index + "(Hold) released " + holdKey);
                    HitJudge.BindNotes[holdKey] = null;
                }
                Line.RemainingNote--;
                HeadHit = false;
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }
    }
    public void Judge()
    {
        if (!HeadHit)
        {
            if (AudioUpdate.Time - From > GameSettings.Bad && !Missed)
            {
                // Miss
                Missed = true;
                Renderer.color = new Color(0.8f, 0.7f, 0.7f, 0.3f);
                HitJudge.JudgeMiss(transform.parent, this, 1);
            }
            if (!Missed && Mathf.Abs(From - AudioUpdate.Time) <= GameSettings.Valid)
            {
                if ((holdKey = HitJudge.IsPress(this)) != 0)
                {
                    HitJudge.Judge(transform.parent, this, AudioUpdate.Time - From, Snd, ref Missed, 1);
                    if (Missed)
                    {
                        if (HitJudge.Record)
                            HitJudge.RecordLog.AppendLine("[Release] " + Index + "(Hold) released " + holdKey);
                        HitJudge.BindNotes[holdKey] = null;
                    }
                    if (!Missed)
                        HeadHit = true;
                }
            }
            if (Mods.Data[Mod.AutoPlay] && Mathf.Abs(From - AudioUpdate.Time) <= GameSettings.Perfect2)
            {
                HitJudge.Judge(transform.parent, this, AudioUpdate.Time - From, Snd, ref Missed, 1);
                HeadHit = true;
            }
        }
        if (HeadHit && !Missed && !EndHit)
        {
            if (AudioUpdate.Audio.isPlaying && !Mods.Data[Mod.AutoPlay])
            {
                if (holdKey == 0)
                {
                    if (HitJudge.Record)
                        HitJudge.RecordLog.AppendLine("[Rebind] " + Index + "(Hold) seeking proper inputs...");
                    holdKey = HitJudge.GetAvailableHoldingKey(this);
                    if (holdKey == 0)
                        failFrames++;
                    else
                        failFrames = 0;
                }
                if (failFrames > 5)
                {
                    if (Mathf.Abs(To - AudioUpdate.Time) > GameSettings.HoldValid)
                    {
                        // Miss
                        Missed = true;
                        Renderer.color = new Color(0.8f, 0.7f, 0.7f, 0.3f);
                        HitJudge.JudgeMiss(transform.parent, this, 1);
                        if (HitJudge.Record)
                            HitJudge.RecordLog.AppendLine("[AutoMiss-NotEnough] " + Index + "(Hold) released too early.");
                    }
                    EndHit = true;
                }
                if (holdKey != 0 && !HitJudge.IsHolding(holdKey))
                {
                    if (HitJudge.Record)
                        HitJudge.RecordLog.AppendLine("[Disconnect] " + Index + "(Hold) lost connection to " + holdKey);
                    if (HitJudge.Record)
                        HitJudge.RecordLog.AppendLine("[Release] " + Index + "(Hold) released " + holdKey);
                    HitJudge.BindNotes[holdKey] = null;
                    if (Mathf.Abs(To - AudioUpdate.Time) > GameSettings.HoldValid)
                    {
                        holdKey = HitJudge.GetAvailableHoldingKey(this);
                    }
                    else
                    {
                        EndHit = true;
                    }
                }
            }
        }
    }
    private void LateUpdate()
    {
        if (!AudioUpdate.Instance.PreviewMode)
        {
            Judge();
        }
        else
        {
            if (!HeadHit && Mathf.Abs(From - AudioUpdate.Time + SettingsController.DelayValue / 1000) <= GameSettings.Perfect2)
            {
                HitJudge.PlayPerfect(transform.parent, 1);
                HeadHit = true;
            }
        }
        UpdateGraphics();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TapController : HitObject
{
    public bool Hit = false;
    private int holdKey = -1;
    private bool missed = false;
    private void UpdateGraphics()
    {
        float x = (From - AudioUpdate.Time) * Line.FlowSpeed * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed * 5, y = 0;
        if (AudioUpdate.Instance.PreviewMode)
            x = (From - AudioUpdate.Time + SettingsController.DelayValue / 1000f) * GamePlayLoops.FlowSpeedFactor * BeatmapLoader.FlowSpeed * Line.FlowSpeed * 5;
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
            y = Mathf.Sin(Index + x / 10f) * (x / LineController.MoveArea * 20f);
        }
        if (HitJudge.Result.Dead)
        {
            missed = true;
            float pass = (float)(DateTime.Now - HitJudge.Result.DeadTime).TotalSeconds;
            x -= pass * (10 + (From - AudioUpdate.Time) * 10f);
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

    private void Judge()
    {
        if (AudioUpdate.Time - From > GameSettings.Bad && !missed)
        {
            if (HitJudge.Record)
                HitJudge.RecordLog.AppendLine("[AutoMiss-TooLate] " + Index + "(Tap) Missed <At " + From + ">");
            HitJudge.JudgeMiss(this);
            missed = true;
            Line.RemainingNote--;
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
        if (!missed && !Hit && Mathf.Abs(From - AudioUpdate.Time) <= GameSettings.Valid)
        {
            holdKey = HitJudge.IsPress(this);
            if (holdKey != 0)
            {
                HitJudge.Judge(transform.parent, this, AudioUpdate.Time - From, ref missed);
                HitJudge.BindNotes[holdKey] = null;
                if (!missed)
                    Hit = true;
                Line.RemainingNote--;
                gameObject.SetActive(false);
                //Destroy(gameObject);
            }
        }
        if (Mods.Data[Mod.AutoPlay] && Mathf.Abs(From - AudioUpdate.Time) <= GameSettings.Perfect2)
        {
            HitJudge.Judge(transform.parent, this, AudioUpdate.Time - From, ref missed);
            Hit = true;
            Line.RemainingNote--;
            gameObject.SetActive(false);
            //Destroy(gameObject);
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
            if (Mathf.Abs(From - AudioUpdate.Time + SettingsController.DelayValue / 1000) <= GameSettings.Perfect2)
            {
                HitJudge.PlayPerfect(transform.parent, 0);
                Line.RemainingNote--;
                gameObject.SetActive(false);
            }
        }
        UpdateGraphics();
    }

}

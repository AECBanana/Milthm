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
              x2 = (To - AudioUpdate.Time) * Line.FlowSpeed * 5 - 1.66f;
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
        transform.localPosition = new Vector3(x, 0, 0);
        if (KeyTip.localEulerAngles.z != -1 * Line.transform.localEulerAngles.z)
            KeyTip.localEulerAngles = new Vector3(0, 0, -1 * Line.transform.localEulerAngles.z);
        if (!HeadHit && x == -1.66f)
        {
            SndPlayer.Play("hit");
            HitAni = HitJudge.Judge(transform.parent);
            HitAni.Play("HoldAni", 0, 0.0f);
            HeadHit = true;
        }
        if (w == Renderer.size.y)
        {
            if (!EndHit)
            {
                EndHit = true;
                if (HitAni != null)
                    HitAni.SetFloat("Speed", 1.0f);
            }
            float d = (AudioUpdate.Time - To) / 0.25f;
            if (d > 1f) d = 1f;
            Renderer.color = new Color(1f, 1f, 1f, 1f - d);
            if (d >= 1f)
            {
                Destroy(gameObject);
                if (HitAni != null)
                    HitAni.SetFloat("Speed", 1.0f);
            }
        }
    }
}

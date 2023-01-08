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
        float x = (Time - AudioUpdate.Time) * Line.FlowSpeed * 5;
        if (x > LineController.MoveArea)
        {
            if (Renderer.color.a != 0)
                Renderer.color = new Color(1f, 1f, 1f, 0f);
            return;
        }
        if (Renderer.color.a == 0)
            Renderer.color = new Color(1f, 1f, 1f, 1f);
        transform.localPosition = new Vector3(x, 0, 0);
        if (KeyTip.localEulerAngles.z != -1 * Line.transform.localEulerAngles.z)
            KeyTip.localEulerAngles = new Vector3(0, 0, -1 * Line.transform.localEulerAngles.z);
        if (x < 0)
        {
            if (!Hit)
            {
                Hit = true;
                SndPlayer.Play(HitJudge.HitSnd);
                HitJudge.Judge(transform.parent);
                Destroy(gameObject);
            }
            float d = -1 * x / 1f;
            if (d > 1f) d = 1f;
            Renderer.color = new Color(1f, 1f, 1f, 1f - d);
            if (d >= 1f)
                Destroy(gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitType{
    Tap, Hold, Drag
}

public class HitObject : MonoBehaviour
{
    private static readonly string[] HitName = {"Tap", "Hold", "Drag"};
    public LineController Line;
    public float From, To;
    public Sprite DoubleSprite;
    public string Snd;
    public int Index;
    public HitType Type;
    public SpriteRenderer Renderer;

    public string HitTypeName => HitName[(int)Type];

    private void Awake()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Renderer.color = new Color(1f, 1f, 1f, 0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏设定
/// </summary>
public class GameSettings : MonoBehaviour
{
    // 击打音效
    public const string HitSnd = "hit";
    // 判定区间
    public const float Perfect2 = 0.03f, Perect = 0.06f, Good = 0.12f, Bad = 0.135f, Valid = 0.25f, HoldValid = 0.5f;
    [Tooltip("鼠标图标")]
    public Texture2D CursorTexture;
    private void Awake()
    {
        Cursor.SetCursor(CursorTexture, Vector2.zero, CursorMode.Auto);
        Application.targetFrameRate = 300;
    }
}

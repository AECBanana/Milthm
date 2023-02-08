using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏设定
/// </summary>
public class GameSettings : MonoBehaviour
{
    public const string FormatVersion = "alpha 0.7.8";
    // 击打音效
    public static string HitSnd = "milthm";
    // 判定区间
    public static float Perfect2 = 0.03f, Perect = 0.06f, Good = 0.12f, Bad = 0.135f, Valid = 0.15f, HoldValid = 0.3f;
    public static float ScreenW, ScreenH;
    public static float WFactor, HFactor;
    public static bool NoCustomSnd = false;
    public static bool NoPerfect = false;
    [Tooltip("鼠标图标")]
    public Texture2D CursorTexture;
    private void Awake()
    {
        Cursor.SetCursor(CursorTexture, new Vector2(16, 16), CursorMode.Auto);
        QualitySettings.vSyncCount = 1;
        if (Application.platform == RuntimePlatform.Android)
            Application.targetFrameRate = 120;
        else
            Application.targetFrameRate = 300;
    }
}

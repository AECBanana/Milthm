using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�趨
/// </summary>
public class GameSettings : MonoBehaviour
{
    public const string FormatVersion = "alpha 0.7.8";
    // ������Ч
    public static string HitSnd = "milthm";
    // �ж�����
    public static float Perfect2 = 0.03f, Perect = 0.06f, Good = 0.12f, Bad = 0.135f, Valid = 0.15f, HoldValid = 0.3f;
    public static float ScreenW, ScreenH;
    public static float WFactor, HFactor;
    public static bool NoCustomSnd = false;
    public static bool NoPerfect = false;
    [Tooltip("���ͼ��")]
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

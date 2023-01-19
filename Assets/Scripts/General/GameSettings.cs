using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�趨
/// </summary>
public class GameSettings : MonoBehaviour
{
    // ������Ч
    public const string HitSnd = "hit";
    // �ж�����
    public const float Perfect2 = 0.03f, Perect = 0.06f, Good = 0.12f, Bad = 0.135f, Valid = 0.25f, HoldValid = 0.5f;
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

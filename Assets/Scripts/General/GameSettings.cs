using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public const string HitSnd = "hit";
    public const float Perfect2 = 0.03f, Perect = 0.06f, Good = 0.12f, Bad = 0.135f, Valid = 0.25f, HoldValid = 0.5f;
    public Texture2D CursorTexture;
    private void Awake()
    {
        Cursor.SetCursor(CursorTexture, Vector2.zero, CursorMode.Auto);
        Application.targetFrameRate = 300;
    }
}

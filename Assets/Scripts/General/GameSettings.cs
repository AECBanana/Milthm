using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public Texture2D CursorTexture;
    private void Awake()
    {
        Cursor.SetCursor(CursorTexture, Vector2.zero, CursorMode.Auto);
        Application.targetFrameRate = -1;
    }
}

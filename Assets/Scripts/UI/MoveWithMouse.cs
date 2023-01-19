using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithMouse : MonoBehaviour
{
    public float XBounce, YBounce;
    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
            return;
        float x = (Input.mousePosition.x / Screen.width - 0.5f) * 2,
              y = (Input.mousePosition.y / Screen.height - 0.5f) * 2;
        transform.localPosition = new Vector3(-x * XBounce, -y * YBounce, 0);
    }
}

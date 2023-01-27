using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformShow : MonoBehaviour
{
    public enum ShowType
    {
        PCOnly, MobileOnly
    }
    public ShowType showType = ShowType.PCOnly;
    private void Awake()
    {
        if (showType == ShowType.MobileOnly && Application.platform != RuntimePlatform.Android)
            gameObject.SetActive(false);
        if (showType == ShowType.PCOnly && Application.platform == RuntimePlatform.Android)
            gameObject.SetActive(false);
    }
}

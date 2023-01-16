using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// µ¼º½À¸¿ØÖÆÆ÷
/// </summary>
public class HeadBarController : MonoBehaviour
{
    public int Index;
    public void Touch()
    {
        if (Index == 4)
        {
            // ÑÓ³Ùµ÷½ÚÆ÷
            Instantiate(Resources.Load<GameObject>("TestDelayCanvas")).SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBarController : MonoBehaviour
{
    public int Index;
    public void Touch()
    {
        if (Index == 4)
        {
            Instantiate(Resources.Load<GameObject>("TestDelayCanvas")).SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������������
/// </summary>
public class HeadBarController : MonoBehaviour
{
    public int Index;
    public void Touch()
    {
        if (Index == 4)
        {
            // �ӳٵ�����
            Instantiate(Resources.Load<GameObject>("TestDelayCanvas")).SetActive(true);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressStart : MonoBehaviour
{
    public GameObject Ani;
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
        {
            Ani.SetActive(true);
        }
    }
}

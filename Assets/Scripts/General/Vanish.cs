using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vanish : MonoBehaviour
{
    public void PauseAni()
    {
        GetComponent<Animator>().SetFloat("Speed", 0.0f);
    }
    public void AniCallback()
    {
        Destroy(gameObject);
    }
    public void Delete()
        => AniCallback();
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

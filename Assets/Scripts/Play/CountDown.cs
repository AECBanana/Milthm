using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    public void ContinueGame()
    {
        AudioUpdate.Audio.Play();
        gameObject.SetActive(false);
    }
}

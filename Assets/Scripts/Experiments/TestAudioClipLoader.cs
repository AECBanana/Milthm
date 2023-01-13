using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestAudioClipLoader : MonoBehaviour
{
    public AudioSource Audio;
    public Image Image;
    void Start()
    {
        string file = "file:///C://Users//Buger%20404//Music//whisper_.mp3";
        var handler = new DownloadHandlerAudioClip(file, AudioType.MPEG);
        var request = new UnityWebRequest(file, "GET", handler, null);
        request.SendWebRequest().completed += (obj) =>
        {
            Audio.clip = handler.audioClip;
            Audio.Play();
            Debug.Log(handler.audioClip.length);
        };
        Test2();
    }
    void Test2()
    {
        string file = "file:///C:/Users/Buger%20404/Pictures/Intallk.png";
        var handler = new DownloadHandlerTexture();
        var request = new UnityWebRequest(file, "GET", handler, null);
        request.SendWebRequest().completed += (obj) =>
        {
            Debug.Log(handler.texture.width + "," + handler.texture.height);
            Sprite sprite = Sprite.Create(handler.texture, new Rect(0, 0, handler.texture.width, handler.texture.height), Vector2.one * 0.5f);
            Image.sprite = sprite;
        };
    }
}

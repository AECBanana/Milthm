using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���渨����
/// </summary>
public class GamePlayAdapter : MonoBehaviour
{
    public static GamePlayAdapter Instance;
    /// <summary>
    /// �����ɵ��������
    /// </summary>
    List<GameObject> Rains = new List<GameObject>();
    public AudioSource BGM;
    private void Awake()
    {
        Instance = this;
    }
    /// <summary>
    /// ���Ž�����Ч
    /// </summary>
    public void PlaySummarySnd()
    {
        if (HitJudge.Result.Dead || HitJudge.Result.Score < 820000)
            SndPlayer.Play("Fail");
        else
            SndPlayer.Play("UI_Buttons_Pack2\\Button_11_Pack2");
    }
    /// <summary>
    /// �������й��
    /// </summary>
    public void HideLines()
    {
        HitJudge.Result.DeadTime = System.DateTime.MinValue;
    }
    /// <summary>
    /// ���Ž���BGM����������������
    /// </summary>
    public void PlaySummaryBGM()
    {
        Camera.main.transform.localEulerAngles = new Vector3(0, 0, 0);
        BGM.clip = BeatmapLoader.Instance.Audio.clip;
        BGM.Play();
        AudioUpdate.Audio.Pause();
        if (BeatmapLoader.Playing.PreviewTime != -1)
            BGM.time = BeatmapLoader.Playing.PreviewTime;
        GameObject rain = Instantiate(GamePlayLoops.Instance.Rain, GamePlayLoops.Instance.Rain.transform.parent);
        Rains.Add(rain);
        rain.SetActive(true);
    }
    /// <summary>
    /// ������Ϸ
    /// </summary>
    public void ContinuePlay()
    {
        // ��ʾ����ʱ
        GamePlayLoops.Instance.CountDown.SetActive(true);
        GamePlayLoops.Instance.PauseScreen.GetComponent<Animator>().Play("HidePausePanel", 0, 0.0f);
    }
    /// <summary>
    /// ���������ɵ��������
    /// </summary>
    public void DeleteRain()
    {
        foreach (GameObject go in Rains)
            Destroy(go);
        Rains.Clear();
    }

    public void Retry()
    {
        HitJudge.CaptureOnce.Clear();
        AudioUpdate.Audio.Pause();
        GamePlayLoops.Instance.BlackScreen.SetActive(true);
    }
}

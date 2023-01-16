using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitJudge : MonoBehaviour
{
    public class ResultData
    {
        public List<KeyCode> UsedKeys = new List<KeyCode>();
        private long mHP = 100;
        private bool danger_hiding = false;
        public int MissContinious = 0;
        public int FullCombo;
        public int Perfect2, Perfect, Good, Bad, Miss;
        public int MaxCombo, Combo, Hit;
        public int Early, Late;
        public long HP
        {
            get
            {
                return mHP;
            }
            set
            {
                mHP = value;
                if (mHP > 100)
                    mHP = 100;
                if (value < 50)
                {
                    if (!GamePlayLoops.Instance.DangerAni.gameObject.activeSelf)
                        GamePlayLoops.Instance.DangerAni.gameObject.SetActive(true);
                }
                else
                {
                    if (GamePlayLoops.Instance.DangerAni.gameObject.activeSelf)
                    {
                        if (!danger_hiding)
                            GamePlayLoops.Instance.DangerAni.Play("HideDanger", 0, 0.0f);
                        danger_hiding = true;
                    }
                    else
                    {
                        danger_hiding = false;
                    }  
                }
                if (value <= 0 && !Dead)
                {
                    Dead = true;
                    DeadTime = DateTime.Now;
                    if (!danger_hiding)
                        GamePlayLoops.Instance.DangerAni.Play("HideDanger", 0, 0.0f);
                    SndPlayer.Play("Fail");
                    GamePlayLoops.Instance.SummaryInfo.UpdateInfo();
                    BeatmapLoader.Instance.Audio.Pause();
                    GamePlayLoops.Instance.SummaryAni.Play("DeadShow", 0, 0.0f);
                    mHP = 0;
                }
            }
        }
        public bool Dead = false, Win = false;
        public DateTime DeadTime;
        public long Score
        {
            get
            {
                if (FullCombo == 0)
                    return 0;
                return (long)((MaxCombo * 1.0 / FullCombo) * 110000 + ((Perfect2 * 1.1 + Perfect * 1.0) / FullCombo + Good * 1.0 / FullCombo * 0.6 + Bad * 1.0 / FullCombo * 0.1) * 900000);
            }
        }
        public float Accuracy
        {
            get
            {
                if (Hit == 0)
                    return 0;
                return Mathf.Floor(((Perfect2 + Perfect) * 1.0f / Hit + Good * 1f / Hit * 0.75f + Bad * 1f / Hit * 0.5f) * 100f) / 100f;
            }
        }
    }
    static GameObject Perfect, Good, Miss, Perfect2;
    public static ResultData Result = new ResultData();
    public static List<List<MonoBehaviour>> HitList = new List<List<MonoBehaviour>>();
    public static List<int> CaptureOnce = new List<int>();
    public static Dictionary<int, MonoBehaviour> BindNotes = new Dictionary<int, MonoBehaviour>();

    static HitJudge()
    {
        Perfect = Resources.Load<GameObject>("Perfect");
        Perfect2 = Resources.Load<GameObject>("Perfect+");
        Good = Resources.Load<GameObject>("Good");
        Miss = Resources.Load<GameObject>("Miss");
    }
    public static int GetAvaliableHoldingKey(MonoBehaviour note)
    {
        for(int i = 0;i < CaptureOnce.Count; i++)
        {
            if (BindNotes[CaptureOnce[i]] == null)
            {
                BindNotes[CaptureOnce[i]] = note;
                return CaptureOnce[i];
            }
        }
        return 0;
    }
    public static int IsPress(MonoBehaviour note)
    {
        if (Application.platform == RuntimePlatform.Android)
            return IsPress_Android(note);
        else
            return IsPress_Windows(note);
    }
    public static bool IsHolding(int key)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            for (int i = 0; i < Input.touchCount; i++)
                if (Input.touches[i].fingerId == key - 1 && Input.touches[i].phase != TouchPhase.Ended && Input.touches[i].phase != TouchPhase.Canceled)
                    return true;
            return false;
        }
        else
            return Input.GetKey((KeyCode)key);
    }
    static int IsPress_Windows(MonoBehaviour note)
    {
        if (GamePlayLoops.AutoPlay)
            return 0;
        if (!AudioUpdate.Audio.isPlaying)
            return 0;
        if (HitList.Count == 0)
            return 0;
        if (!HitList[0].Contains(note))
            return 0;
        if (Input.anyKey)
        {
            for (int key = (int)KeyCode.A; key <= (int)KeyCode.Z; key++)
            {
                if (Input.GetKeyDown((KeyCode)key) && !CaptureOnce.Contains(key))
                {
                    /**if (note is TapController tap)
                        Debug.Log("tap " + tap.Index + " -> " + key);
                    else if (note is HoldController hold)
                        Debug.Log("hold " + hold.Index + " -> " + key);**/
                    if (!BindNotes.ContainsKey(key))
                        BindNotes.Add(key, null);
                    BindNotes[key] = note;
                    CaptureOnce.Add(key);
                    return key;
                }
            }
            return 0;
        }
        else
        {
            return 0;
        }
    }
    static int IsPress_Android(MonoBehaviour note)
    {
        if (GamePlayLoops.AutoPlay)
            return 0;
        if (!AudioUpdate.Audio.isPlaying)
            return 0;
        if (HitList.Count == 0)
            return 0;
        if (!HitList[0].Contains(note))
            return 0;
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began && !CaptureOnce.Contains(touch.fingerId + 1))
                {
                    if (!BindNotes.ContainsKey(touch.fingerId + 1))
                        BindNotes.Add(touch.fingerId + 1, null);
                    BindNotes[touch.fingerId + 1] = note;
                    CaptureOnce.Add(touch.fingerId + 1);
                    return touch.fingerId + 1;
                }
            }
            return 0;
        }
        else
        {
            return 0;
        }
    }
    public static Animator Judge(Transform AniParent, MonoBehaviour note, float deltaTime)
    {
        GameObject effect = null;
        float orTime = deltaTime;
        deltaTime = Mathf.Abs(deltaTime);
        bool miss = false;
        if (deltaTime <= GameSettings.Perfect2)
        {
            effect = Perfect2;
            Result.Perfect2++;
            Result.HP += 3;
        }
        else if (deltaTime <= GameSettings.Perect)
        {
            effect = Perfect;
            Result.Perfect++;
            Result.HP += 2;
        }
        else if (deltaTime <= GameSettings.Good)
        {
            effect = Good;
            Result.Good++;
            Result.HP += 1;
            if (orTime > 0)
            {
                Result.Late++;
                GamePlayLoops.Instance.Pitch.gameObject.SetActive(false);
                GamePlayLoops.Instance.Pitch.text = "Late";
                GamePlayLoops.Instance.Pitch.gameObject.SetActive(true);
            }
            else
            {
                Result.Early++;
                GamePlayLoops.Instance.Pitch.gameObject.SetActive(false);
                GamePlayLoops.Instance.Pitch.text = "Early";
                GamePlayLoops.Instance.Pitch.gameObject.SetActive(true);
            } 
        }
        else if (deltaTime <= GameSettings.Bad)
        {
            Result.Bad++;
            if (orTime > 0)
            {
                Result.Late++;
                GamePlayLoops.Instance.Pitch.gameObject.SetActive(false);
                GamePlayLoops.Instance.Pitch.text = "Late";
                GamePlayLoops.Instance.Pitch.gameObject.SetActive(true);
            }
            else
            {
                Result.Early++;
                GamePlayLoops.Instance.Pitch.gameObject.SetActive(false);
                GamePlayLoops.Instance.Pitch.text = "Early";
                GamePlayLoops.Instance.Pitch.gameObject.SetActive(true);
            }
        }
        else
        {
            /**if (note is TapController tap)
                Debug.Log("tap " + tap.Index + " hit, but too early.");
            else if (note is HoldController hold)
                Debug.Log("hold " + hold.Index + " hit, but too early.");**/
            effect = Miss;
            Result.Combo = 0;
            miss = true;
            Result.Miss++;
            Result.MissContinious++;
            Result.HP -= 5;
            if (orTime > 0)
                Result.Late++;
            else
                Result.Early++;
        }
        if (!miss)
        {
            SndPlayer.Play(GameSettings.HitSnd);
            Result.Combo++;
            if (Result.Combo > Result.MaxCombo) 
                Result.MaxCombo = Result.Combo;
            Result.MissContinious = 0;
        }
        Result.Hit++;
        MoveNext(note);
        if (effect != null)
        {
            GameObject go = Instantiate(effect, AniParent);
            go.transform.localPosition = new Vector3(4.1f, 0, 0);
            go.SetActive(true);
            return go.GetComponent<Animator>();
        }
        else
        {
            return null;
        }
    }
    public static void MoveNext(MonoBehaviour note)
    {
        if (!HitList[0].Contains(note))
            return;
        HitList[0].Remove(note);
        if (HitList[0].Count == 0)
            HitList.RemoveAt(0);
    }
    public static void JudgeMiss(Transform AniParent, MonoBehaviour note)
    {
        /**if (note is TapController tap)
            Debug.Log("tap " + tap.Index + " Missed!");
        else if (note is HoldController hold)
            Debug.Log("hold " + hold.Index + " Missed!");**/
        Result.MissContinious++;
        Result.Miss++;
        Result.Hit++;
        Result.Combo = 0;
        Result.Late++;
        MoveNext(note);
        Result.HP -= 10;
        GameObject go = Instantiate(Miss, AniParent);
        go.transform.localPosition = new Vector3(4.1f, 0, 0);
        go.SetActive(true);
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    public static List<KeyCode> CaptureOnce = new List<KeyCode>();
    public static Dictionary<KeyCode, MonoBehaviour> BindNotes = new Dictionary<KeyCode, MonoBehaviour>();

    static HitJudge()
    {
        Perfect = Resources.Load<GameObject>("Perfect");
        Perfect2 = Resources.Load<GameObject>("Perfect+");
        Good = Resources.Load<GameObject>("Good");
        Miss = Resources.Load<GameObject>("Miss");
    }
    public static KeyCode GetAvaliableHoldingKey(MonoBehaviour note)
    {
        //Debug.Log("Finding avaliable keys...");
        for(int i = 0;i < CaptureOnce.Count; i++)
        {
            if (BindNotes[CaptureOnce[i]] == null)
            {
                //Debug.Log(CaptureOnce[i] + " is free");
                BindNotes[CaptureOnce[i]] = note;
                return CaptureOnce[i];
            }
            else
            {
                //Debug.Log(CaptureOnce[i] + " is bind to " + BindNotes[CaptureOnce[i]].name);
            }
        }
        return KeyCode.None;
    }
    public static KeyCode IsPress(MonoBehaviour note)
    {
        if (GamePlayLoops.AutoPlay)
            return KeyCode.None;
        if (!AudioUpdate.Audio.isPlaying)
            return KeyCode.None;
        if (HitList.Count == 0)
            return KeyCode.None;
        if (!HitList[0].Contains(note))
            return KeyCode.None;
        if (Input.anyKey)
        {
            for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
            {
                if (Input.GetKeyDown(key) && !CaptureOnce.Contains(key))
                {
                    /**if (note is TapController tap)
                        Debug.Log("tap " + tap.Index + " -> " + key);
                    else if (note is HoldController hold)
                        Debug.Log("hold " + hold.Index + " -> " + key);**/
                    BindNotes[key] = note;
                    CaptureOnce.Add(key);
                    return key;
                }   
            }
            return KeyCode.None;
        }
        else
        {
            return KeyCode.None;
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

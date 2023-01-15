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
    public static List<KeyCode> KeyWeight = new List<KeyCode>()
    {
        KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P,
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.Z, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.B, KeyCode.N, KeyCode.M
    };

    static HitJudge()
    {
        Perfect = Resources.Load<GameObject>("Perfect");
        Perfect2 = Resources.Load<GameObject>("Perfect+");
        Good = Resources.Load<GameObject>("Good");
        Miss = Resources.Load<GameObject>("Miss");
    }
    public static bool IsPress(KeyCode key, MonoBehaviour note, bool capture = false)
    {
        if (key == KeyCode.Tab)
            return false;
        if (GamePlayLoops.AutoPlay)
            return false;
        if (!AudioUpdate.Audio.isPlaying)
            return false;
        if (HitList.Count == 0)
            return false;
        if (HitList[0].Contains(note))
        {
            if (CaptureOnce.Contains(key))
                return false;
            bool ret = Input.GetKey(key);
            if (capture && ret)
                CaptureOnce.Add(key);
            return ret;
        }
        else
        {
            return false;
        }
    }
    public static void ResortLineKeys()
    {
        List<LineController> lines = new List<LineController>();
        List<KeyCode> keys = new List<KeyCode>();
        for(int i = 0;i < LineController.Lines.Count; i++)
        {
            if (LineController.Lines[i].OriginKey == KeyCode.Tab && LineController.Lines[i].KeyOverride != KeyCode.Tab)
            {
                lines.Add(LineController.Lines[i]);
                keys.Add(LineController.Lines[i].KeyOverride);
            }
        }
        keys.Sort((x, y) => KeyWeight.FindIndex(a => a == x).CompareTo(KeyWeight.FindIndex(b => b == y)));
        for (int i = 0; i < lines.Count; i++)
        {
            lines[i].KeyOverride = keys[i];
            lines[i].UpdateKeyTip();
        }
    }
    public static KeyCode AnykeyPress(MonoBehaviour note)
    {
        if (GamePlayLoops.AutoPlay)
            return KeyCode.Tab;
        if (!AudioUpdate.Audio.isPlaying)
            return KeyCode.Tab;
        if (HitList.Count == 0)
            return KeyCode.Tab;
        if (!HitList[0].Contains(note))
            return KeyCode.Tab;
        if (Input.anyKey)
        {
            List<KeyCode> avaliable = new List<KeyCode>();
            for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
            {
                if (Input.GetKey(key) && !Result.UsedKeys.Contains(key))
                {
                    avaliable.Add(key);
                }
            }
            if (avaliable.Count > 0)
            {
                avaliable.Sort((x, y) => KeyWeight.FindIndex(a => a == x).CompareTo(KeyWeight.FindIndex(b => b == y)));
                int i = (HitList[0].FindIndex(x => x == note) + 1) / HitList[0].Count * avaliable.Count - 1;
                if (i < 0)
                    i = 0;
                if (i >= avaliable.Count)
                    i = avaliable.Count - 1;
                KeyCode key = avaliable[i];
                Result.UsedKeys.Add(key);
                return key;
            }
            return KeyCode.Tab;
        }
        else
        {
            return KeyCode.Tab;
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
        HitList[0].Remove(note);
        if (HitList[0].Count == 0)
            HitList.RemoveAt(0);
    }
    public static void JudgeMiss(Transform AniParent, MonoBehaviour note)
    {
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

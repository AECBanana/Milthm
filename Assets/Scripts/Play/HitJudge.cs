using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

/// <summary>
/// 击打判定器
/// </summary>
public class HitJudge : MonoBehaviour
{
    /// <summary>
    /// 结算数据
    /// </summary>
    public class ResultData
    {
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
                // 播放警告动画
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
                // 死亡处理
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
                return Mathf.Floor(((Perfect2 + Perfect) * 1.0f / Hit + Good * 1f / Hit * 0.75f + Bad * 1f / Hit * 0.5f) * 10000f) / 10000f;
            }
        }
    }
    // 指定判定的特效物体
    static GameObject Perfect, Good, Miss, Perfect2;
    public static ResultData Result = new ResultData();
    public static float JudgeArea = 0;
    public static bool Record = false;
    /// <summary>
    /// 判定模式，0=全屏判定，1=非全屏判定(但PC不支持)
    /// </summary>
    public static int JudgeMode = 0;
    public static StringBuilder RecordLog = new StringBuilder();
    /// <summary>
    /// 击打列表
    /// </summary>
    public static List<List<MonoBehaviour>> HitList = new List<List<MonoBehaviour>>();
    /// <summary>
    /// 已被捕获的输入
    /// </summary>
    public static List<int> CaptureOnce = new List<int>();
    /// <summary>
    /// 已捕获的输入对应的note
    /// </summary>
    public static Dictionary<int, MonoBehaviour> BindNotes = new Dictionary<int, MonoBehaviour>();

    static HitJudge()
    {
        Perfect = Resources.Load<GameObject>("Perfect");
        Perfect2 = Resources.Load<GameObject>("Perfect+");
        Good = Resources.Load<GameObject>("Good");
        Miss = Resources.Load<GameObject>("Miss");
    }
    /// <summary>
    /// 查找可用的输入
    /// </summary>
    /// <param name="note">note</param>
    /// <returns>0为无可用输入</returns>
    public static int GetAvaliableHoldingKey(MonoBehaviour note)
    {
        if (JudgeMode == 1 && Application.platform == RuntimePlatform.Android)
            return 0;
        if (Record)
            RecordLog.AppendLine("++Start Seeking");
        for(int i = 0;i < CaptureOnce.Count; i++)
        {
            if (BindNotes[CaptureOnce[i]] == null)
            {
                if (Record)
                    RecordLog.AppendLine(CaptureOnce[i] + " is AVALIABLE.\n++");
                BindNotes[CaptureOnce[i]] = note;
                return CaptureOnce[i];
            }
            else if (Record)
            {
                if (BindNotes[CaptureOnce[i]] is TapController tap)
                    RecordLog.AppendLine(CaptureOnce[i] + " is connected with " + tap.Index + "(Tap)");
                else if (BindNotes[CaptureOnce[i]] is HoldController hold)
                    RecordLog.AppendLine(CaptureOnce[i] + " is connected with " + hold.Index + "(Hold)");
            }
        }
        if (Record)
        {
            if (CaptureOnce.Count == 0)
                RecordLog.AppendLine("No inputs right now.");
            RecordLog.AppendLine("++");
        }
        return 0;
    }
    /// <summary>
    /// 是否输入
    /// </summary>
    /// <param name="note">note</param>
    /// <returns>非0表示存在有效输入</returns>
    public static int IsPress(MonoBehaviour note)
    {
        if (Application.platform == RuntimePlatform.Android)
            return IsPress_Android(note);
        else
            return IsPress_Windows(note);
    }
    /// <summary>
    /// 是否输入持续中
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Windows端输入判定
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
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
                if (Input.GetKey((KeyCode)key) && !CaptureOnce.Contains(key))
                {
                    if (Record)
                    {
                        if (note is TapController tap)
                            RecordLog.AppendLine("[Bind] " + tap.Index + "(Tap) -> " + key);
                        else if (note is HoldController hold)
                            RecordLog.AppendLine("[Bind] " + hold.Index + "(Hold) -> " + key);
                    }
                    if (!BindNotes.ContainsKey(key))
                        BindNotes.Add(key, null);
                    BindNotes[key] = note;
                    CaptureOnce.Add(key);
                    return key;
                }
                else if (Record)
                {
                    if (Input.GetKeyDown((KeyCode)key))
                        RecordLog.AppendLine("[Log] " + key + " avaliable, but being catched.");
                }
            }
            return 0;
        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// 移动端输入判定
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
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
        Transform judgePoint = null;
        if (JudgeMode == 1)
        {
            if (note is TapController tap)
                judgePoint = tap.Line.JudgePoint.transform;
            else if (note is HoldController hold)
                judgePoint = hold.Line.JudgePoint.transform;
        }
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled && !CaptureOnce.Contains(touch.fingerId + 1))
                {
                    if (JudgeMode == 1)
                    {
                        Vector2 delta = Camera.main.ScreenToWorldPoint(touch.position) - judgePoint.position;
                        if (!(Math.Abs(delta.x) <= JudgeArea && Math.Abs(delta.y) <= JudgeArea))
                            continue;
                    }
                    if (!BindNotes.ContainsKey(touch.fingerId + 1))
                        BindNotes.Add(touch.fingerId + 1, null);
                    if (Record)
                    {
                        if (note is TapController tap)
                            RecordLog.AppendLine("[Bind] " + tap.Index + "(Tap) -> Finger " + (touch.fingerId + 1));
                        else if (note is HoldController hold)
                            RecordLog.AppendLine("[Bind] " + hold.Index + "(Hold) -> Finger " + (touch.fingerId + 1));
                    }
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
    public static Animator PlayPerfect(Transform AniParent)
    {
        SndPlayer.Play(GameSettings.HitSnd);
        GameObject go = Instantiate(Perfect2, AniParent);
        go.transform.localPosition = new Vector3(4.1f, 0, 0);
        go.SetActive(true);
        return go.GetComponent<Animator>();
    }
    /// <summary>
    /// 判定
    /// </summary>
    /// <param name="AniParent">判定动画生成位置</param>
    /// <param name="note">note</param>
    /// <param name="deltaTime">误差时间</param>
    /// <returns>如有判定动画生成，则为非null</returns>
    public static Animator Judge(Transform AniParent, MonoBehaviour note, float deltaTime, ref bool missed)
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
            if (Record)
            {
                if (note is TapController tap)
                    RecordLog.AppendLine("[AutoMiss-TooEarly] " + tap.Index + "(Tap) Missed");
                else if (note is HoldController hold)
                    RecordLog.AppendLine("[AutoMiss-TooEarly] " + hold.Index + "(Hold) Missed");
            }
            effect = Miss;
            Result.Combo = 0;
            miss = true;
            Result.Miss++;
            missed = true;
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
            if (Result.Combo % 100 == 0 || Result.Combo == 50)
            {
                GamePlayLoops.Instance.ComboTip.text = Result.Combo + " COMBO";
                GamePlayLoops.Instance.ComboTip.gameObject.SetActive(true);
            }
        }
        if (Record)
        {
            if (note is TapController tap)
                RecordLog.AppendLine("[Judge] " + tap.Index + "(Tap): " + deltaTime * 1000 + "ms <At " + AudioUpdate.Time + ">");
            else if (note is HoldController hold)
                RecordLog.AppendLine("[Judge] " + hold.Index + "(Hold): " + deltaTime * 1000 + "ms <At " + AudioUpdate.Time + ">");
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
    /// <summary>
    /// 将判定移动到下一个note
    /// </summary>
    /// <param name="note"></param>
    public static void MoveNext(MonoBehaviour note)
    {
        if (!HitList[0].Contains(note))
            return;
        HitList[0].Remove(note);
        if (HitList[0].Count == 0)
            HitList.RemoveAt(0);
    }
    /// <summary>
    /// 强制判定Miss（过晚、hold过早松开等）
    /// </summary>
    /// <param name="AniParent"></param>
    /// <param name="note"></param>
    public static void JudgeMiss(Transform AniParent, MonoBehaviour note)
    {
        if (Record)
        {
            if (note is TapController tap)
                RecordLog.AppendLine("[AutoMiss] " + tap.Index + "(Tap): Missed");
            else if (note is HoldController hold)
                RecordLog.AppendLine("[AutoMiss] " + hold.Index + "(Hold): Missed");
        }
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

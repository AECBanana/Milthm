using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// �����ж���
/// </summary>
public class HitJudge : MonoBehaviour
{
    /// <summary>
    /// ��������
    /// </summary>
    public class ResultData
    {
        private long mHP = 100;
        private bool danger_hiding = false;
        public int MissContinuous = 0;
        public int FullCombo;
        public int Perfect2, Perfect, Good, Bad, Miss;
        public int MaxCombo, Combo, Hit;
        public int Early, Late;
        public long HP
        {
            get => mHP;
            set
            {
                mHP = value;
                if (mHP > 100)
                    mHP = 100;
                // ���ž��涯��
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
                // ��������
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
                double orScore = OriginScore;
                double judgeBuff = 1.0;
                if (JudgeRange == 0)
                    judgeBuff = 0.8;
                else if (JudgeRange == 1)
                    judgeBuff = 1;
                else if (JudgeRange == 2)
                    judgeBuff = 1.2;
                return (long)(orScore * judgeBuff);
            }
        }
        public double OriginScore => (MaxCombo * 1.0 / FullCombo) * 110000 + 
                                     ((Perfect2 * 1.1 + Perfect * 1.0) / FullCombo + 
                                      Good * 1.0 / FullCombo * 0.6 + 
                                      Bad * 1.0 / FullCombo * 0.1) * 900000;

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
    // ָ���ж�����Ч����
    private static GameObject[] Perfect, Good, Bad, Perfect2;
    public static ResultData Result = new ResultData();
    public static float JudgeArea = 0;
    public static int JudgeRange = 1;
    public static bool Record = false;
    public static bool NoDead = false;
    /// <summary>
    /// �ж�ģʽ��0=ȫ���ж���1=��ȫ���ж�(��PC��֧��)
    /// </summary>
    public static int JudgeMode = 0;
    public static readonly StringBuilder RecordLog = new StringBuilder();
    /// <summary>
    /// �����б�
    /// </summary>
    public static List<List<MonoBehaviour>> HitList = new List<List<MonoBehaviour>>();
    /// <summary>
    /// �ѱ����������
    /// </summary>
    public static List<int> CaptureOnce = new List<int>();
    /// <summary>
    /// �Ѳ���������Ӧ��note
    /// </summary>
    public static Dictionary<int, MonoBehaviour> BindNotes = new Dictionary<int, MonoBehaviour>();

    static HitJudge()
    {
        Perfect2 =  new GameObject[] {
            Resources.Load<GameObject>("Perfect+"), Resources.Load<GameObject>("Perfect+_Hold")
        };
        Perfect =  new GameObject[] {
            Resources.Load<GameObject>("Perfect"), Resources.Load<GameObject>("Perfect_Hold")
        };
        Good =  new GameObject[] {
            Resources.Load<GameObject>("Good"), Resources.Load<GameObject>("Good_Hold")
        };
        Bad =  new GameObject[] {
            Resources.Load<GameObject>("Bad"), Resources.Load<GameObject>("Bad_Hold")
        };
    }
    /// <summary>
    /// ���ҿ��õ�����
    /// </summary>
    /// <param name="note">note</param>
    /// <returns>0Ϊ�޿�������</returns>
    public static int GetAvailableHoldingKey(MonoBehaviour note)
    {
        if (JudgeMode == 1 && Application.platform == RuntimePlatform.Android)
            return 0;
        if (Record)
            RecordLog.AppendLine("++Start Seeking");
        foreach (var capture in CaptureOnce)
        {
            if (!BindNotes[capture])
            {
                if (Record)
                    RecordLog.AppendLine(capture + " is AVAILABLE.\n++");
                BindNotes[capture] = note;
                return capture;
            }
            else if (Record)
            {
                switch (BindNotes[capture])
                {
                    case TapController tap:
                        RecordLog.AppendLine(capture + " is connected with " + tap.Index + "(Tap)");
                        break;
                    case HoldController hold:
                        RecordLog.AppendLine(capture + " is connected with " + hold.Index + "(Hold)");
                        break;
                }
            }
        }

        if (!Record) return 0;
        if (CaptureOnce.Count == 0)
            RecordLog.AppendLine("No inputs right now.");
        RecordLog.AppendLine("++");
        return 0;
    }
    /// <summary>
    /// �Ƿ�����
    /// </summary>
    /// <param name="note">note</param>
    /// <returns>��0��ʾ������Ч����</returns>
    public static int IsPress(MonoBehaviour note)
    {
        return Application.platform == RuntimePlatform.Android ? IsPress_Android(note) : IsPress_Windows(note);
    }
    /// <summary>
    /// �Ƿ����������
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static bool IsHolding(int key)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (JudgeMode == 1)
            {
                return LineController.Lines[key - 1].Holding;
            }
            else
            {
                for (var i = 0; i < Input.touchCount; i++)
                    if (Input.touches[i].fingerId == key - 1 && Input.touches[i].phase != TouchPhase.Ended && Input.touches[i].phase != TouchPhase.Canceled)
                        return true;
                return false;
            }
        }
        else
            return Input.GetKey((KeyCode)key);
    }
    /// <summary>
    /// Windows�������ж�
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
                if (Record)
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
    /// �ƶ��������ж�
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
        if (JudgeMode == 1)
        {
            // ��ȫ���ж�
            var line = note switch
            {
                TapController x => x.Line,
                HoldController x => x.Line,
                _ => null
            };
            if (line.FirstHold && !CaptureOnce.Contains(line.Index + 1))
            {
                if (!BindNotes.ContainsKey(line.Index + 1))
                    BindNotes.Add(line.Index + 1, null);
                BindNotes[line.Index + 1] = note;
                CaptureOnce.Add(line.Index + 1);
                return line.Index + 1;
            }
            return 0;
        }
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase is TouchPhase.Ended or TouchPhase.Canceled ||
                    CaptureOnce.Contains(touch.fingerId + 1)) continue;
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
            return 0;
        }
        else
        {
            return 0;
        }
    }
    public static void PlayPerfect(Transform aniParent, int type)
    {
        SndPlayer.Play(GameSettings.HitSnd);
        var go = Instantiate(Perfect2[type], aniParent);
        go.transform.localPosition = new Vector3(-1.97f, 0, 0);
        go.SetActive(true);
    }
    /// <summary>
    /// �ж�
    /// </summary>
    /// <param name="aniParent">�ж���������λ��</param>
    /// <param name="note">note</param>
    /// <param name="deltaTime">���ʱ��</param>
    /// <returns>�����ж��������ɣ���Ϊ��null</returns>
    public static void Judge(Transform aniParent, MonoBehaviour note, float deltaTime, string snd, ref bool missed, int type)
    {
        GameObject effect = null;
        float orTime = deltaTime;
        deltaTime = Mathf.Abs(deltaTime);
        bool miss = false;
        if (deltaTime <= GameSettings.Perfect2)
        {
            effect = Perfect2[type];
            Result.Perfect2++;
            Result.HP += 3;
        }
        else if (deltaTime <= GameSettings.Perfect)
        {
            effect = Perfect[type];
            Result.Perfect++;
            Result.HP += 2;
        }
        else if (deltaTime <= GameSettings.Good)
        {
            effect = Good[type];
            Result.Good++;
            Result.HP += 1;
            if (orTime > 0)
            {
                Result.Late++;
                GamePlayLoops.Instance.Late.SetActive(false);
                GamePlayLoops.Instance.Late.SetActive(true);
            }
            else
            {
                Result.Early++;
                GamePlayLoops.Instance.Early.SetActive(false);
                GamePlayLoops.Instance.Early.SetActive(true);
            } 
        }
        else if (deltaTime <= GameSettings.Bad)
        {
            Result.Bad++;
            Result.Combo = -1;
            if (orTime > 0)
            {
                Result.Late++;
                GamePlayLoops.Instance.Late.SetActive(false);
                GamePlayLoops.Instance.Late.SetActive(true);
            }
            else
            {
                Result.Early++;
                GamePlayLoops.Instance.Early.SetActive(false);
                GamePlayLoops.Instance.Early.SetActive(true);
            }
            effect = Bad[type];
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
            Result.Combo = 0;
            miss = true;
            Result.Miss++;
            missed = true;
            Result.MissContinuous++;
            if (!NoDead)
                Result.HP -= 7;
            if (orTime > 0)
                Result.Late++;
            else
                Result.Early++;
        }
        if (!miss)
        {
            if (string.IsNullOrEmpty(snd))
                SndPlayer.Play(GameSettings.HitSnd);
            else if (SongResources.HitSnd[snd] == null)
                SndPlayer.Play(GameSettings.HitSnd);
            else
                SndPlayer.Play(SongResources.HitSnd[snd]);
            Result.Combo++;
            if (Result.Combo > Result.MaxCombo) 
                Result.MaxCombo = Result.Combo;
            Result.MissContinuous = 0;
            if (Result.Combo % 100 == 0 || Result.Combo == 50)
            {
                GamePlayLoops.Instance.ComboTip.text = Result.Combo + " COMBO";
                GamePlayLoops.Instance.ComboTip.gameObject.SetActive(true);
            }
        }
        if (Record)
        {
            switch (note)
            {
                case TapController tap:
                    RecordLog.AppendLine("[Judge] " + tap.Index + "(Tap): " + deltaTime * 1000 + "ms <At " + AudioUpdate.Time + ">");
                    break;
                case HoldController hold:
                    RecordLog.AppendLine("[Judge] " + hold.Index + "(Hold): " + deltaTime * 1000 + "ms <At " + AudioUpdate.Time + ">");
                    break;
            }
        }
        Result.Hit++;
        MoveNext(note);
        if (effect)
        {
            if (effect == Perfect[type] && GameSettings.NoPerfect)
                effect = Perfect2[type];
            var go = Instantiate(effect, aniParent);
            go.transform.localPosition = new Vector3(-1.97f, 0, 0); //4.1f
            go.SetActive(true);
        }
    }
    /// <summary>
    /// ���ж��ƶ�����һ��note
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
    /// ǿ���ж�Miss������hold�����ɿ��ȣ�
    /// </summary>
    /// <param name="aniParent"></param>
    /// <param name="note"></param>
    /// <param name="aniType"></param>
    public static void JudgeMiss(Transform aniParent, MonoBehaviour note, int aniType)
    {
        if (Record)
        {
            if (note is TapController tap)
                RecordLog.AppendLine("[AutoMiss] " + tap.Index + "(Tap): Missed");
            else if (note is HoldController hold)
                RecordLog.AppendLine("[AutoMiss] " + hold.Index + "(Hold): Missed");
        }
        Result.MissContinuous++;
        Result.Miss++;
        Result.Hit++;
        Result.Combo = 0;
        Result.Late++;
        MoveNext(note);
        if (!NoDead)
            Result.HP -= 7;
    }

}

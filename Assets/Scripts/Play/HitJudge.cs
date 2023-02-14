using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
    // 指定判定的特效物体
    private static GameObject[] Perfect, Good, Bad, Perfect2;
    public static ResultData Result = new ResultData();
    public static int JudgeRange = 1;
    public static bool Record = false;

    /// <summary>
    /// 判定模式，0=全屏判定，1=非全屏判定(但PC不支持)
    /// </summary>
    public static int JudgeMode = 0;
    public static readonly StringBuilder RecordLog = new ();
    /// <summary>
    /// 击打列表
    /// </summary>
    public static List<List<HitObject>> HitList = new ();
    /// <summary>
    /// 已被捕获的输入
    /// </summary>
    public static List<int> CaptureOnce = new ();
    /// <summary>
    /// 已捕获的输入对应的note
    /// </summary>
    public static Dictionary<int, HitObject> BindNotes = new ();

    static HitJudge()
    {
        Perfect2 =  new GameObject[] {
            Resources.Load<GameObject>("Perfect+"), 
            Resources.Load<GameObject>("Perfect+_Hold"),
            Resources.Load<GameObject>("Perfect+")
        };
        Perfect =  new GameObject[] {
            Resources.Load<GameObject>("Perfect"), 
            Resources.Load<GameObject>("Perfect_Hold"),
            Resources.Load<GameObject>("Perfect")
        };
        Good =  new GameObject[] {
            Resources.Load<GameObject>("Good"), 
            Resources.Load<GameObject>("Good_Hold"),
            Resources.Load<GameObject>("Good")
        };
        Bad =  new GameObject[] {
            Resources.Load<GameObject>("Bad"), 
            Resources.Load<GameObject>("Bad_Hold"),
            Resources.Load<GameObject>("Bad")
        };
    }
    /// <summary>
    /// 查找可用的输入
    /// </summary>
    /// <param name="note">note</param>
    /// <returns>0为无可用输入</returns>
    public static int GetAvailableHoldingKey(HitObject note)
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

            if (Record)
            {
                RecordLog.AppendLine(capture + " is connected with " + BindNotes[capture].Index + "(" + BindNotes[capture].HitTypeName + ")");
            }
        }

        if (!Record) return 0;
        if (CaptureOnce.Count == 0)
            RecordLog.AppendLine("No inputs right now.");
        RecordLog.AppendLine("++");
        return 0;
    }
    /// <summary>
    /// 是否输入
    /// </summary>
    /// <param name="note">note</param>
    /// <returns>非0表示存在有效输入</returns>
    public static int IsPress(HitObject note)
    {
        return Application.platform == RuntimePlatform.Android ? IsPress_Android(note) : IsPress_Windows(note);
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
    /// Windows端输入判定
    /// </summary>
    /// <param name="note"></param>
    /// <returns></returns>
    private static int IsPress_Windows(HitObject note)
    {
        if (Mods.Data[Mod.AutoPlay])
            return 0;
        if (!AudioUpdate.Audio.isPlaying)
            return 0;
        if (HitList.Count == 0)
            return 0;
        if (!HitList[0].Contains(note))
            return 0;
        if (Input.anyKey)
        {
            for (var key = (int)KeyCode.A; key <= (int)KeyCode.Z; key++)
            {
                if (Input.GetKey((KeyCode)key) && !CaptureOnce.Contains(key))
                {
                    if (Record)
                    {
                        RecordLog.AppendLine("[Bind] " + note.Index + "(" + note.HitTypeName + ") -> " + key);
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
                        RecordLog.AppendLine("[Log] " + key + " avaliable, but being caught.");
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
    static int IsPress_Android(HitObject note)
    {
        if (Mods.Data[Mod.AutoPlay])
            return 0;
        if (!AudioUpdate.Audio.isPlaying)
            return 0;
        if (HitList.Count == 0)
            return 0;
        if (!HitList[0].Contains(note))
            return 0;
        if (JudgeMode == 1)
        {
            // 非全屏判定
            var line = note.Line;
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
                    RecordLog.AppendLine("[Bind] " + note.Index + "(" + note.HitTypeName + ") -> Finger " + (touch.fingerId + 1));
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
    /// 判定
    /// </summary>
    /// <param name="aniParent">判定动画生成位置</param>
    /// <param name="note">note</param>
    /// <param name="deltaTime">误差时间</param>
    /// <returns>如有判定动画生成，则为非null</returns>
    public static void Judge(Transform aniParent, HitObject note, float deltaTime, ref bool missed)
    {
        var type = (int)note.Type;
        var orTime = deltaTime;
        var miss = false;
        GameObject effect = null;
        deltaTime = Mathf.Abs(deltaTime);
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
            if (Mods.Data[Mod.PerfectionismIII])
                GamePlayAdapter.Instance.Retry();
        }
        else if (deltaTime <= GameSettings.Good)
        {
            if (Mods.Data[Mod.PerfectionismII] || Mods.Data[Mod.PerfectionismIII])
                GamePlayAdapter.Instance.Retry();
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
            if (Mods.Data[Mod.PerfectionismI] || Mods.Data[Mod.PerfectionismII] || Mods.Data[Mod.PerfectionismIII])
                GamePlayAdapter.Instance.Retry();
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
                RecordLog.AppendLine("[AutoMiss-TooEarly] " + note.Index + "(" + note.HitTypeName + ") Missed");
            }
            Result.Combo = 0;
            miss = true;
            Result.Miss++;
            missed = true;
            Result.MissContinuous++;
            if (!Mods.Data[Mod.NoDead])
                Result.HP -= 7;
            if (orTime > 0)
                Result.Late++;
            else
                Result.Early++;
            if (Mods.Data[Mod.PerfectionismI] || Mods.Data[Mod.PerfectionismII] || Mods.Data[Mod.PerfectionismIII])
                GamePlayAdapter.Instance.Retry();
        }
        if (!miss)
        {
            var snd = note.Snd;
            if (string.IsNullOrEmpty(snd))
                SndPlayer.Play(note.Type == HitType.Drag ? "drag" : GameSettings.HitSnd);
            else if (!SongResources.HitSnd[snd])
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
            RecordLog.AppendLine("[Judge] " + note.Index + "(" + note.HitTypeName + "): " + deltaTime * 1000 + "ms <At " + AudioUpdate.Time + ">");
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
    /// 将判定移动到下一个note
    /// </summary>
    /// <param name="note"></param>
    private static void MoveNext(HitObject note)
    {
        if (HitList.Count == 0)
            return;
        if (!HitList[0].Contains(note))
            return;
        HitList[0].Remove(note);
        if (HitList[0].Count == 0)
            HitList.RemoveAt(0);
    }
    /// <summary>
    /// 强制判定Miss（过晚、hold过早松开等）
    /// </summary>
    /// <param name="aniParent"></param>
    /// <param name="note"></param>
    /// <param name="aniType"></param>
    public static void JudgeMiss(HitObject note)
    {
        if (Record)
        {
            RecordLog.AppendLine("[AutoMiss] " + note.Index + "(" + note.HitTypeName + "): Missed");
        }
        Result.MissContinuous++;
        Result.Miss++;
        Result.Hit++;
        Result.Combo = 0;
        Result.Late++;
        MoveNext(note);
        if (!Mods.Data[Mod.NoDead])
            Result.HP -= 7;
        if (Mods.Data[Mod.PerfectionismI] || Mods.Data[Mod.PerfectionismII] || Mods.Data[Mod.PerfectionismIII])
            GamePlayAdapter.Instance.Retry();
    }

}

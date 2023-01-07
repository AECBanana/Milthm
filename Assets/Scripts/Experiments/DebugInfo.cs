using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfo : MonoBehaviour
{
    public static DebugInfo Instance { get; private set; }
    static Dictionary<string, string> infos = new Dictionary<string, string>();
    static Dictionary<string, int> ticks = new Dictionary<string, int>();
    static Dictionary<string, int> lticks = new Dictionary<string, int>();
    static float accumulate_time = 0f;
    Text text;
    private void Awake()
    {
        Instance = this;
        text = GetComponent<Text>();
    }
    private void Update()
    {
        StringBuilder sb = new();
        foreach (string key in infos.Keys)
        {
            sb.AppendLine($"<b>{key}</b>   {infos[key]}");
        }
        text.text = sb.ToString();

        accumulate_time += Time.deltaTime;
        Tick("FPS");
        if (accumulate_time >= 1.0f)
        {
            accumulate_time = 0f;
            foreach (string key in ticks.Keys)
            {
                if (!lticks.ContainsKey(key))
                    lticks.Add(key, ticks[key]);

                if (!infos.ContainsKey(key))
                {
                    infos.Add(key, ticks[key].ToString());
                }
                else
                {
                    string v = infos[key];
                    float nv = ticks[key], lv = lticks[key];
                    if (nv == lv)
                        v = "<color=black>" + nv + "</color> " + v;
                    else if (nv > lv)
                        v = "<color=green>" + nv + "</color> " + v;
                    else if (nv < lv)
                        v = "<color=red>" + nv + "</color> " + v;
                    if (v.Length > 400)
                    {
                        for (int i = v.Length - 1;i >= 0; i--)
                        {
                            if (v[i] == ' ')
                            {
                                v = v.Substring(0, i);
                                break;
                            }
                        }
                    }
                    infos[key] = v;
                }
                lticks[key] = ticks[key];
            }
            ticks.Clear();
        }
    }
    public static void Output(string key, string value)
    {
        if (!infos.ContainsKey(key))
            infos.Add(key, value);
        else
            infos[key] = value;
    }
    public static void Tick(string key)
    {
        if (!ticks.ContainsKey(key))
            ticks.Add(key, 0);
        ticks[key]++;
    }
}

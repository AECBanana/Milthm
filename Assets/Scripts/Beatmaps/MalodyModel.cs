using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class MalodySongMeta
{
    public string title;
    public string artist;
    public int id;
    public string titleorg;
    public string artistorg;
}
[Serializable]
public class MalodyModeExtMeta
{
    public int column;
    public int bar_begin;
}
[Serializable]
public class MalodyMeta
{
    public string creator;
    public string background;
    public string version;
    public int id;
    public int mode;
    public long time;
    public long preview;
    public MalodySongMeta song;
    public MalodyModeExtMeta mode_ext;
}
[Serializable]
public class MalodyTime
{
    public int[] beat;
    public float bpm;
}
[Serializable]
public class MalodyNote
{
    public int[] beat;
    public int[] endbeat;
    public int column;
    public string sound;
    public float vol;
    public float offset;
    public int type;
}
[Serializable]
public class MalodyTest
{
    public int divide;
    public int speed;
    public int save;
    public int edit_mode;
}
[Serializable]
public class MalodyExtra
{
    public int[] beat;
    public int column;
}
[Serializable]
public class MalodyModel
{
    public MalodyMeta meta;
    public List<MalodyTime> time;
    public List<string> effect;
    public List<MalodyNote> note;
}
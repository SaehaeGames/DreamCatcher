using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RepeatQuestDreamCatcher
{
    public string DCid;
    public int DCcolor;
    public int DCfeather1, DCfeather2, DCfeather3;

    public RepeatQuestDreamCatcher()
    {
        DCid = "JS_0000";
        DCcolor = 0;
        DCfeather1 = 0;
        DCfeather2 = 0;
        DCfeather3 = 0;
    }
    public RepeatQuestDreamCatcher(string _DCid, int _DCcolor, int _DCfeather1, int _DCfeather2, int _DCfeather3)
    {
        DCid = _DCid;
        DCcolor = _DCcolor;
        DCfeather1 = _DCfeather1;
        DCfeather2 = _DCfeather2;
        DCfeather3 = _DCfeather3;
    }
}

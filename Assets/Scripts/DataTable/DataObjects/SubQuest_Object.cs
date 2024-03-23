using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SubQuest_Object
{
    public int thread;
    public int feather1;
    public int feather2;
    public int feather3;
    public int reward;
    public int read;

    public SubQuest_Object()
    {
        thread = feather1 = feather2 = feather3 = reward = read = 0;
    }

    public SubQuest_Object(int _thread, int _feather1, int _feather2, int _feather3, int _reward, int _read)
    {
        thread = _thread;
        feather1 = _feather1;
        feather2 = _feather2;
        feather3 = _feather3;
        reward = _reward;
        read = _read;
    }
}

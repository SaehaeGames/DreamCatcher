using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class DreamInfo_Object
{
    public string id;
    public string name;
    public string kind;
    public string line;
    public string feather1;
    public string feather2;
    public string feather3;
    public int bead;

    public DreamInfo_Object()
    {
        id = "SO_0000";
        bead = 0;
        name = kind = line = feather1 = feather2 = feather3 = "";
    }

    public DreamInfo_Object(string _id, string _name, string _kind, string _line, string _feather1, string _feather2, string _feather3, int _bead)
    {
        id = _id;
        name = _name;
        kind = _kind;
        line = _line;
        feather1 = _feather1;
        feather2 = _feather2;
        feather3 = _feather3;
        bead = _bead;
    }
}

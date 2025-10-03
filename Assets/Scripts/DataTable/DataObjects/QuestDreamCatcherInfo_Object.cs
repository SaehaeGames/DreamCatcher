using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestDreamCatcherInfo_Object
{
    public int id;
    [HideInInspector] [SerializeField] private string linesRaw;
    [HideInInspector] [SerializeField] private string beadsRaw;
    public int[] lines;
    public bool[] beads;
    public int color;
    public int feather1;
    public int feather2;
    public int feather3;

    public QuestDreamCatcherInfo_Object()
    {
        this.id = 0;
        this.lines = new int[64];
        this.beads = new bool[48];
        this.color = 0;
        this.feather1 = 0;
        this.feather2 = 0;
        this.feather3 = 0;
    }

    public QuestDreamCatcherInfo_Object(int _id, int[] _lines, bool[] _beads, int _color, int _feather1, int _feather2, int _feather3)
    {
        this.id = _id;
        this.lines = _lines;
        this.beads = _beads;
        this.color = _color;
        this.feather1 = _feather1;
        this.feather2 = _feather2;
        this.feather3 = _feather3;
    }

    public void Parse()
    {
        if(!string.IsNullOrEmpty(linesRaw))
        {
            lines = Array.ConvertAll(linesRaw.Trim('[', ']').Split(','), s => int.Parse(s.Trim()));
        }

        if (!string.IsNullOrEmpty(beadsRaw))
        {
            beads = Array.ConvertAll(beadsRaw.Trim('[', ']').Split(','), s => bool.Parse(s.Trim()));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharonInfo_Object
{
    public string id;
    public string title;
    public string contents;
    public string from;
    public string item;
    public string note;

    public CharonInfo_Object()
    {
        this.id = "SO_0000";
        this.title = this.note = this.item = this.contents = this.from = "";
    }

    public CharonInfo_Object(string _id, string _title, string _contents, string _from, string _item, string _note)
    {
        this.id = _id;
        this.title = _title;
        this.contents = _contents;
        this.from = _from;
        this.item = _item;
        this.note = _note;
    }

    public string GetId()
    {
        return id;
    }

    public string GetTitle()
    {
        return title;
    }

    public string GetContents()
    {
        return contents;
    }

    public string GetFrom()
    {
        return from;
    }

    public string GetItem()
    {
        return item;
    }

    public string GetNote()
    {
        return note;
    }
}

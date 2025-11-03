using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryScriptInfo_Object
{
    public string id;
    public int sceneNum;
    public int questNum;
    public int charImage;
    public int faceImage;
    public int effectImage;
    public int screenEffect;
    public string speaker;
    public string line;

    public StoryScriptInfo_Object()
    {
        id = "SO_0000";
        sceneNum = questNum = charImage = faceImage = effectImage = screenEffect = 0;
        speaker = line = "";
    }

    public StoryScriptInfo_Object(string _id, int _sceneNum, int _questNum, int _charImage, int _faceImage, int _effectImage, int _screenEffect, string _speaker, string _line)
    {
        this.id = _id;
        this.sceneNum = _sceneNum;
        this.questNum = _questNum;
        this.charImage = _charImage;
        this.faceImage = _faceImage;
        this.effectImage = _effectImage;
        this.screenEffect = _screenEffect;
        this.speaker = _speaker;
        this.line = _line;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StorySceneInfo_Object
{
    public int id;
    public int sceneNum;
    public int startId;
    public int endId;

    public StorySceneInfo_Object()
    {
        id = sceneNum = startId = endId = 0;
    }

    public StorySceneInfo_Object(int _id, int _sceneNum, int _startId, int _endId)
    {
        this.id = _id;
        this.sceneNum = _sceneNum;
        this.startId = _startId;
        this.endId = _endId;
    }
}

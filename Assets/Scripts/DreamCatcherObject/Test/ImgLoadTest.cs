using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImgLoadTest : MonoBehaviour
{
    public Image testSpriteIndex;
    public Image testSpriteId;
    public Text testText;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            testSpriteIndex.sprite = DreamCatcherInfoLoad.Instance.ImageLoad(1000);
            testSpriteId.sprite = DreamCatcherInfoLoad.Instance.ImageLoad(0);
            testText.text = "드림캐쳐 존재";
        }
        catch(Exception e)
        {
            testText.text += e;
        }
    }

    public void deleteDreamCatcherFile(int id)
    {
        DreamCatcherInfoLoad.Instance.ImageDelete(id);
        DreamCatcherInfoLoad.Instance.DataDelete(id);
    }
}

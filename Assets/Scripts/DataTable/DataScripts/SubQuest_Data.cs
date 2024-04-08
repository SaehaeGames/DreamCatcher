using UnityEngine;
using System;
using System.IO;
using UnityEngine.Events;
using UnityEditor;

[CreateAssetMenu(fileName = "DataTable", menuName = "Scriptable Object Asset/SubQuestInfo")]
public class SubQuest_Data : ScriptableObject
{
    public SubQuest_Object curSubQuest;

    public void test(int num)
    {
        curSubQuest.thread = num;
    }

}
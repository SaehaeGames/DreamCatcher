using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdCSV : MonoBehaviour
{
    private List<Dictionary<string, object>> data;

    private void OnEnable()
    {
        data = CSVParser.ReadFromFile("BirdInfo");
    }

    public List<Dictionary<string, object>> GetBirdData()
    {
        return data;
    }

    public void SaveBirdInfo()
    {
        CSVParser.WriteFromFile("BirdInfo", data);
    }

}

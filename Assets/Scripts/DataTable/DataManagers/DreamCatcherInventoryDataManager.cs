using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DreamCatcherInventoryDataManager
{
    private JsonManager jsonManager = new JsonManager();
    private const int IdBase = 1000; // 아이디 시작 지점
    private DreamCatcherInventoryDataModel dreamCatcherInventoryDataModel = new DreamCatcherInventoryDataModel();

    public void AddDreamCatcherInventoryData(DreamCatcher dreamCatcher)
    {
        if (dreamCatcher == null)
            return;

        var data = dreamCatcherInventoryDataModel.dataList.FirstOrDefault(
            x => x.TemplateHash == dreamCatcher.TemplateHash
        );

        if (data != null)
        {
            data.Number += 1;
        }
        else
        {
            dreamCatcherInventoryDataModel.dataList.Add(new DreamCatcherInventoryData(dreamCatcher));
        }
    }

    public void ResetData()
    {
        if (dreamCatcherInventoryDataModel.dataList != null)
            dreamCatcherInventoryDataModel.dataList.Clear();
    }

    public DreamCatcherInventoryData GetDreamCatcherInventoryData(string _templateHash)
    {
        DreamCatcherInventoryData getData = dreamCatcherInventoryDataModel.dataList.FirstOrDefault(x => x.TemplateHash == _templateHash);
        if (getData != null)
            return getData;
        else
            return null;
    }
}

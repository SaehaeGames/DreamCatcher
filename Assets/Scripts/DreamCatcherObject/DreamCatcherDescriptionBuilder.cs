using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Text;
using UnityEngine;

public static class DreamCatcherDescriptionBuilder
{
    public static string Build(DreamCatcher dreamCatcher)
    {
        var sb = new StringBuilder();

        sb.AppendLine(GetColorText(dreamCatcher.DCcolor));
        sb.AppendLine(GetFeatherText(dreamCatcher.DCfeather1, dreamCatcher.DCfeather2, dreamCatcher.DCfeather3));
        return sb.ToString();
    }

    public static string GetColorText(int colorNum)
    {
        string content = "";
        switch (colorNum)
        {
            case 0:
                content += "흰색";
                break;
            case 1:
                content += "노란색";
                break;
            case 2:
                content += "파란색";
                break;
            case 3:
                content += "빨간색";
                break;
            case 4:
                content += "검정색";
                break;
            default:
                content += "";
                break;
        }

        return content;
    }

    public static string GetFeatherText(int _feather1, int _feather2, int _feather3)
    {
        BirdInfo_Data _birdinfo_data = GameManager.instance.birdinfo_data;
        string featherTxt = "";

        if (_feather1 == _feather2 && _feather1 == _feather3)
        {
            featherTxt = _birdinfo_data.dataList[_feather1].name + " 깃털 X3";
        }
        else if (_feather1 == _feather2 && _feather1 != _feather3)
        {
            featherTxt = _birdinfo_data.dataList[_feather1].name + " 깃털 X2" + "\n" + _birdinfo_data.dataList[_feather3].name + " 깃털";
        }
        else if (_feather1 == _feather3 && _feather1 != _feather2)
        {
            featherTxt = _birdinfo_data.dataList[_feather1].name + " 깃털 X2" + "\n" + _birdinfo_data.dataList[_feather2].name + " 깃털";
        }
        else if (_feather2 == _feather3 && _feather2 != _feather1)
        {
            featherTxt = _birdinfo_data.dataList[_feather2].name + " 깃털 X2" + "\n" + _birdinfo_data.dataList[_feather1].name + " 깃털";
        }
        else
        {
            featherTxt = _birdinfo_data.dataList[_feather1].name + " 깃털" + "\n" + _birdinfo_data.dataList[_feather2].name + " 깃털" + "\n" + _birdinfo_data.dataList[_feather3].name + " 깃털";
        }
        return featherTxt;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DreamCatcher
{
    public int DCid;
    public int[] DCline = new int[64];
    public bool[] DCbead = new bool[48];
    public int DCcolor;
    public int DCfeather1, DCfeather2, DCfeather3;

    public DreamCatcher(int _DCid, int[] _DCline, bool[] _DCbead, int _DCcolor, int _DCfeather1, int _DCfeather2, int _DCfeather3)
    {
        DCid = _DCid;
        DCline = _DCline;
        DCbead = _DCbead;
        DCcolor = _DCcolor;
        DCfeather1 = _DCfeather1;
        DCfeather2 = _DCfeather2;
        DCfeather3 = _DCfeather3;
    }

    

    // 드림캐쳐 배열변형(1D->2D) 함수
    // : 드림캐쳐의 실모양을 나타내는 DCLine1d를 1차원배열에서 2차원배열로 변환하는 함수
    public int[,] ConvertLineArrayTo2D(int[] line1d)
    {
        int index = 0;
        int[,] line2d = new int[8, 8];
        // 매개변수인 dcline2d를 1차원 배열로 변환
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                line2d[i, j] = line1d[index];
                index++;
            }
        }
        return line2d;
    }


    // **드림캐쳐 GET 함수

    // 드림캐쳐 id GET 함수
    // : 드림캐쳐 id를 가져오는 함수
    public int GetId()
    {
        return DCid;
    }

    // 드림캐쳐 색 GET 함수
    // : 드림캐쳐 색을 가져오는 함수
    public int GetColor()
    {
        return DCcolor;
    }

    // 드림캐쳐 깃털 GET 함수
    // : 드림캐쳐 깃털을 가져오는 함수
    public int GetFeather(int index)
    {
        switch(index)
        {
            case 0:
                return DCfeather1;
            case 1:
                return DCfeather2;
            case 2:
                return DCfeather3;
        }
        return 20; 
    }

    // 드림캐쳐 라인 GET 함수
    // : 드림캐쳐 실모양을 가져오는 함수
    public int[] GetLine()
    {
        return DCline;
    }

    // 드림캐쳐 구슬 GET 함수
    // : 드림캐쳐 구슬배치를 가져오는 함수
    public bool[] GetBead()
    {
        return DCbead;
    }
}

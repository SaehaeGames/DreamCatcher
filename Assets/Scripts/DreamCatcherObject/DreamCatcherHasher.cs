using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class DreamCatcherHasher
{
    public static string CreateTemplateHash(int[] _DCline, bool[] _DCbead, int _DCcolor, int _DCfeather1, int _DCfeather2, int _DCfeather3)
    {
        // 1. 해시에 포함할 데이터만 추출
        var sb = new StringBuilder();

        sb.Append(_DCcolor).Append("|");

        int[] feathers = { _DCfeather1, _DCfeather2, _DCfeather3 };

        sb.Append(string.Join(",", feathers)).Append("|");

        sb.Append(string.Join(",", _DCline)).Append("|");

        sb.Append(string.Join(",", _DCbead.Select(b => b ? 1 : 0)));

        // 2. 문자열 → 해시
        return ComputeSHA256(sb.ToString());
    }

    private static string ComputeSHA256(string raw)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        return BitConverter.ToString(bytes).Replace("-", "");
    }
}

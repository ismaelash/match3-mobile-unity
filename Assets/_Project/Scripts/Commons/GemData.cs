using System;
using UnityEngine;
using static IsmaelNascimento.Commons.GameData;

[Serializable]
public class GemData
{
    public GemType type;
    public Sprite sprite;
    public int minMatch = 3;
}

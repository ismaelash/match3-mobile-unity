using System;
using UnityEngine;
using static Match3.Commons.GameData;

namespace Match3.Commons
{
    [Serializable]
    public class GemData
    {
        public GemType type;
        public Sprite sprite;
        public int minMatch = 3;
        [HideInInspector] public Vector2Int position;
    }
}
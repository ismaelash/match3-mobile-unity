using System;
using System.Collections.Generic;

namespace Match3.Commons
{
    [Serializable]
    public class CrushInfo
    {
        public BaseGem gem;
        public BaseGem currentSwap;
        public List<BaseGem> swaps = new List<BaseGem>();

        public CrushInfo(BaseGem gem)
        {
            this.gem = gem;
        }
    }
}
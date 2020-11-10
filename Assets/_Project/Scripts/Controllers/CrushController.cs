using System.Collections.Generic;
using Utils;

namespace IsmaelNascimento.Controllers
{
    public class CrushController : Singleton<CrushController>
    {
        #region VARIABLES

        private List<CrushInfo> crushes = new List<CrushInfo>();

        public bool HasCrushes
        {
            get => crushes.Count > 0;
        }

        public bool Paused { get; set; }

        #endregion

        #region PUBLIC_METHODS

        public void FindCrushes()
        {
            crushes.Clear();

            for (int indexY = 0; indexY < BoardController.Instance.Height; ++indexY)
            {
                for (int indexX = 0; indexX < BoardController.Instance.Width; ++indexX)
                {
                    BaseGem gem = BoardController.Instance.GetGem(indexX, indexY);

                    // Swap Right
                    BaseGem otherGem = BoardController.Instance.GetGem(indexX + 1, indexY);
                    if (otherGem && otherGem.type != gem.type)
                    {
                        CrushInfo crushInfo = GetCrush(gem, otherGem);

                        if (crushInfo != null && !crushes.Contains(crushInfo))
                        {
                            crushes.Add(crushInfo);
                        }
                    }

                    // Swap Up
                    otherGem = BoardController.Instance.GetGem(indexX, indexY + 1);
                    if (otherGem && otherGem.type != gem.type)
                    {
                        CrushInfo crushInfo = GetCrush(gem, otherGem);

                        if (crushInfo != null && !crushes.Contains(crushInfo))
                        {
                            crushes.Add(crushInfo);
                        }
                    }
                }
            }
        }

        #endregion

        #region PRIVATE_METHODS

        private CrushInfo GetCrush(BaseGem gem, BaseGem otherGem)
        {
            if (!(gem && otherGem))
            {
                return null;
            }

            CrushInfo crushInfo = null;
            CrushInfo crushA = crushes.Find(h => h.gem == gem);
            CrushInfo crushB = crushes.Find(h => h.gem == otherGem);

            BoardController.Instance.SwapGems(gem, otherGem);

            MatchInfo matchInfoA = gem.GetMatch();
            MatchInfo matchInfoB = otherGem.GetMatch();

            if (matchInfoA.isValid)
            {
                crushInfo = crushA ?? new CrushInfo(gem);
                crushInfo.swaps.Add(otherGem);
            }
            else if (matchInfoB.isValid)
            {
                crushInfo = crushB ?? new CrushInfo(otherGem);
                crushInfo.swaps.Add(gem);
            }

            BoardController.Instance.SwapGems(gem, otherGem);

            return crushInfo;
        }

        #endregion
    }
}
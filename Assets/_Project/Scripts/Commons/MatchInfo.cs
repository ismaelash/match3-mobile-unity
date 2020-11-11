using System.Collections.Generic;
using UnityEngine;

namespace Match3.Commons
{
    public class MatchInfo
    {
        #region ENUMS

        public enum MatchType
        {
            Invalid,
            Horizontal,
            Vertical,
            Cross
        }

        #endregion

        #region VARIABLES

        private Vector2Int minPosition;
        private Vector2Int maxPosition;
        private MatchType type;
        private List<BaseGem> matches = new List<BaseGem>();
        private Vector2Int pivot;

        public bool IsValid
        {
            get => type != MatchType.Invalid;
        }

        public List<BaseGem> Matches
        {
            get => new List<BaseGem>(matches);
        }

        public BaseGem Pivot
        {
            get
            {
                if (type == MatchType.Cross)
                {
                    return matches.Find(gem => gem.gemData.position == pivot);
                }
                else
                {
                    return matches[0];
                }
            }
        }

        private int HorizontalLenght
        {
            get => (maxPosition.x - minPosition.x) + 1;
        }

        private int VerticalLenght
        {
            get => (maxPosition.y - minPosition.y) + 1;
        }

        #endregion

        #region PUBLIC_METHODS

        // Constructor
        public MatchInfo(List<BaseGem> matches = null)
        {
            if (matches != null)
            {
                pivot = matches[0].gemData.position;
                AddMatches(matches);
            }
        }

        public int GetScore()
        {
            if (!IsValid)
            {
                return 0;
            }

            return matches.Count;
        }

        // Join Crossed Matches from same type
        public static MatchInfo JoinCrossedMatches(MatchInfo matchInfoA, MatchInfo matchInfoB)
        {

            if (!(matchInfoA.IsValid && matchInfoB.IsValid) || matchInfoA.Pivot.gemData.type != matchInfoB.Pivot.gemData.type)
            {
                return new MatchInfo();
            }

            foreach (BaseGem match in matchInfoA.matches)
            {
                if (matchInfoB.matches.Contains(match))
                {
                    matchInfoA.pivot = match.gemData.position;
                    matchInfoB.matches.Remove(match);
                    matchInfoA.AddMatches(matchInfoB.matches);

                    return matchInfoA;
                }
            }

            return new MatchInfo();
        }

        public List<Vector2Int> GetFallPositions()
        {
            List<Vector2Int> fallPositions = new List<Vector2Int>();

            matches.ForEach(match =>
            {
                int id = fallPositions.FindIndex(fallPosition => fallPosition.x == match.gemData.position.x);

                if (id > -1 && match.gemData.position.y < fallPositions[id].y)
                {
                    fallPositions[id] = match.gemData.position;
                }
                else
                {
                    fallPositions.Add(match.gemData.position);
                }
            });

            return fallPositions;
        }

        public static List<Vector2Int> JoinFallPositions(List<Vector2Int> matchA, List<Vector2Int> matchB)
        {
            List<Vector2Int> fallPositions = new List<Vector2Int>();

            if (matchA.Count == 0)
            {
                return matchB;
            }
            else if (matchB.Count == 0)
            {
                return matchA;
            }

            fallPositions.AddRange(matchA);

            matchB.ForEach(currentFall =>
            {
                int id = fallPositions.FindIndex(fallPosition => fallPosition.x == currentFall.x);

                if (id > -1 && currentFall.y < fallPositions[id].y)
                {
                    fallPositions[id] = currentFall;
                }
                else
                {
                    fallPositions.Add(currentFall);
                }
            });

            return fallPositions;
        }

        #endregion

        #region PRIVATE_METHODS

        private void AddMatches(List<BaseGem> matches)
        {
            this.matches.AddRange(matches);
            ValidateMatch();
        }

        private void ValidateMatch()
        {
            type = MatchType.Invalid;
            minPosition = maxPosition = Pivot.gemData.position;

            foreach (BaseGem match in matches)
            {
                int x = minPosition.x;
                int y = minPosition.y;

                if (match.gemData.position.x < minPosition.x)
                {
                    x = match.gemData.position.x;
                }

                if (match.gemData.position.y < minPosition.y)
                {
                    y = match.gemData.position.y;
                }

                minPosition = new Vector2Int(x, y);

                x = maxPosition.x;
                y = maxPosition.y;

                if (match.gemData.position.x > maxPosition.x)
                {
                    x = match.gemData.position.x;
                }

                if (match.gemData.position.y > maxPosition.y)
                {
                    y = match.gemData.position.y;
                }

                maxPosition = new Vector2Int(x, y);

                if (HorizontalLenght >= Pivot.gemData.minMatch)
                {
                    type |= MatchType.Horizontal;
                }

                if (VerticalLenght >= Pivot.gemData.minMatch)
                {
                    type |= MatchType.Vertical;
                }
            }
        }

        #endregion
    }
}
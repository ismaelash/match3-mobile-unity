using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Commons
{
    public class MatchInfo
    {
        #region ENUMS

        [Flags]
        public enum MatchType
        {
            Invalid,    //00000000
            Horizontal, //00000001
            Vertical,   //00000010
            Cross       //00000011
        }

        #endregion

        #region VARIABLES

        Vector2Int minPosition;
        Vector2Int maxPosition;

        #endregion



        int horizontalLenght
        {
            get { return (maxPosition.x - minPosition.x) + 1; }
        }

        int verticalLenght
        {
            get { return (maxPosition.y - minPosition.y) + 1; }
        }

        public MatchType type;

        public bool isValid
        {
            get { return type != MatchType.Invalid; }
        }

        List<BaseGem> _matches = new List<BaseGem>();
        public List<BaseGem> matches
        {
            get { return new List<BaseGem>(_matches); }
        }

        public List<MatchInfo> specialMatches = new List<MatchInfo>();

        Vector2Int _pivot;
        public BaseGem pivot
        {
            get
            {
                if (type == MatchType.Cross)
                    return _matches.Find(gem => gem.gemData.position == _pivot);
                else
                    return _matches[0];
            }
        }

        public MatchInfo(List<BaseGem> matches = null)
        {
            if (matches != null)
            {
                _pivot = matches[0].gemData.position;
                AddMatches(matches);
            }
        }

        void AddMatches(List<BaseGem> matches)
        {
            _matches.AddRange(matches);
            ValidateMatch();
        }

        public void RemoveMatches(List<BaseGem> matches)
        {
            _matches.RemoveAll(g => matches.Contains(g));
        }

        void ValidateMatch()
        {
            type = MatchType.Invalid;
            minPosition = maxPosition = pivot.gemData.position;

            foreach (BaseGem match in _matches)
            {
                int x = minPosition.x;
                int y = minPosition.y;

                if (match.gemData.position.x < minPosition.x)
                    x = match.gemData.position.x;
                if (match.gemData.position.y < minPosition.y)
                    y = match.gemData.position.y;

                minPosition = new Vector2Int(x, y);

                x = maxPosition.x;
                y = maxPosition.y;

                if (match.gemData.position.x > maxPosition.x)
                    x = match.gemData.position.x;
                if (match.gemData.position.y > maxPosition.y)
                    y = match.gemData.position.y;

                maxPosition = new Vector2Int(x, y);

                if (horizontalLenght >= pivot.gemData.minMatch)
                    type |= MatchType.Horizontal;

                if (verticalLenght >= pivot.gemData.minMatch)
                    type |= MatchType.Vertical;
            }
        }

        public int GetScore()
        {
            if (!isValid)
                return 0;

            return _matches.Count;
        }

        // Join Crossed Matches from same type
        public static MatchInfo JoinCrossedMatches(MatchInfo a, MatchInfo b)
        {

            if (!(a.isValid && b.isValid) || a.pivot.gemData.type != b.pivot.gemData.type)
            {
                return new MatchInfo();
            }

            foreach (BaseGem match in a._matches)
            {
                if (b._matches.Contains(match))
                {
                    a._pivot = match.gemData.position;
                    b._matches.Remove(match);
                    a.AddMatches(b._matches);

                    return a;
                }
            }

            return new MatchInfo();
        }

        public List<Vector2Int> GetFallPositions()
        {
            List<Vector2Int> fallPositions = new List<Vector2Int>();

            _matches.ForEach(match =>
            {
                int id = fallPositions.FindIndex(f => f.x == match.gemData.position.x);
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

        public static List<Vector2Int> JoinFallPositions(
            List<Vector2Int> matchA, List<Vector2Int> matchB
        )
        {
            List<Vector2Int> fallPositions = new List<Vector2Int>();

            if (matchA.Count == 0)
                return matchB;
            else if (matchB.Count == 0)
                return matchA;

            fallPositions.AddRange(matchA);

            matchB.ForEach(currentFall =>
            {
                int id = fallPositions.FindIndex(f => f.x == currentFall.x);
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
    }
}
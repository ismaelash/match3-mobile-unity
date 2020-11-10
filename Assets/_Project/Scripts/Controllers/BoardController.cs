using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace IsmaelNascimento.Controllers
{
    public class BoardController : SingletonMonoBehaviour<BoardController>
    {
        #region VARIABLES

        [Header("Config Board")]
        [SerializeField] private BaseGem gemPrefab;
        [SerializeField] private int width = 6;
        [SerializeField] private int height = 6;

        // Publics
        public bool IsUpdatingBoard { get; set; }
        public Action OnEndUpdatingBoard;

        // Privates
        private Coroutine updateBoard;
        private int matchCounter;

        // Properties
        public int Width { get => width; set => width = value; }

        public int Height { get => height; set => height = value; }

        public BaseGem[,] GemBoard { get; set; }
        public int MatchCounter
        {
            get => matchCounter;
            set => matchCounter = Mathf.Min(value, GameData.maxCombo);
        }

        #endregion

        #region PUBLIC_METHODS

        // Calculate Board Position into World
        public Vector3 GetWorldPosition(Vector2Int position)
        {
            float newX = position.x - ((Width / 2f) - 0.5f);
            float newY = position.y - ((Height / 2f) - 0.5f);
            
            return new Vector2(newX, newY);
        }

        public float CreateBoard()
        {
            GemBoard = new BaseGem[Width, Height];
            float maxDuration = 0;
            float delayLine = 0;

            for (int indexY = Height - 1; indexY >= 0; --indexY)
            {
                for (int indexX = 0; indexX < Width; ++indexX)
                {
                    BaseGem gem = CreateRandomGem(indexX, indexY, GetWorldPosition(new Vector2Int(indexX, indexY + 1)), delayLine);

                    if (GameController.Instance.PreventInitialMatches)
                    {
                        while (gem.GetMatch().isValid)
                        {
                            gem.SetType(GameData.RandomGem());
                        }
                    }

                    float duration = gem.MoveTo(GetWorldPosition(gem.position), GameController.Instance.FallSpeed, delayLine);

                    if (duration > maxDuration)
                    {
                        maxDuration = duration;
                    }
                }

                delayLine = maxDuration;
            }

            return maxDuration;
        }

        // Check if position is valid, then returns a Gem
        public BaseGem GetGem(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return null;

            return GemBoard[x, y];
        }

        public void SwapGems(BaseGem from, BaseGem to)
        {
            Vector2Int fromPosition = from.position;
            from.SetPosition(to.position);
            to.SetPosition(fromPosition);
        }

        public void TryMatch(BaseGem from, BaseGem to)
        {
            StartCoroutine(TryMatch_Coroutine(from, to));
        }

        public void UpdateBoard()
        {
            if (updateBoard != null)
            {
                StopCoroutine(updateBoard);
            }

            updateBoard = StartCoroutine(UpdateBoard_Coroutine());
        }

        public MatchInfo GetHorizontalMatch(BaseGem gem, Func<BaseGem, bool> validateGem)
        {
            List<BaseGem> matches = new List<BaseGem>
            {
                gem
            };

            BaseGem gemToCheck = GetGem(gem.position.x - 1, gem.position.y);

            while (gemToCheck && validateGem(gemToCheck))
            {
                matches.Add(gemToCheck);
                gemToCheck = GetGem(gemToCheck.position.x - 1, gemToCheck.position.y);
            }

            gemToCheck = GetGem(gem.position.x + 1, gem.position.y);

            while (gemToCheck && validateGem(gemToCheck))
            {
                matches.Add(gemToCheck);
                gemToCheck = GetGem(gemToCheck.position.x + 1, gemToCheck.position.y);
            }

            return new MatchInfo(matches);
        }

        public MatchInfo GetVerticalMatch(BaseGem gem, Func<BaseGem, bool> validateGem)
        {
            List<BaseGem> matches = new List<BaseGem>
            {
                gem
            };

            BaseGem gemToCheck = GetGem(gem.position.x, gem.position.y - 1);

            while (gemToCheck && validateGem(gemToCheck))
            {
                matches.Add(gemToCheck);
                gemToCheck = GetGem(gemToCheck.position.x, gemToCheck.position.y - 1);
            }

            gemToCheck = GetGem(gem.position.x, gem.position.y + 1);

            while (gemToCheck && validateGem(gemToCheck))
            {
                matches.Add(gemToCheck);
                gemToCheck = GetGem(gemToCheck.position.x, gemToCheck.position.y + 1);
            }

            return new MatchInfo(matches);
        }

        public MatchInfo GetCrossMatch(BaseGem gem, Func<BaseGem, bool> validateGem)
        {
            MatchInfo mathcInfohorizontal = GetHorizontalMatch(gem, validateGem);
            MatchInfo matchInfoVertical = GetVerticalMatch(gem, validateGem);
            int crossCheck = 0;

            while (!mathcInfohorizontal.isValid && crossCheck < matchInfoVertical.matches.Count)
            {
                if (matchInfoVertical.isValid)
                {
                    mathcInfohorizontal = GetHorizontalMatch(matchInfoVertical.matches[crossCheck], validateGem);
                }
                else
                {
                    break;
                }

                crossCheck++;
            }

            crossCheck = 0;
            while (!matchInfoVertical.isValid && crossCheck < mathcInfohorizontal.matches.Count)
            {
                if (mathcInfohorizontal.isValid)
                {
                    matchInfoVertical = GetVerticalMatch(mathcInfohorizontal.matches[crossCheck], validateGem);
                }
                else
                {
                    break;
                }

                crossCheck++;
            }

            MatchInfo mathcInfo = MatchInfo.JoinCrossedMatches(mathcInfohorizontal, matchInfoVertical);

            if (!mathcInfo.isValid)
            {
                if (mathcInfohorizontal.isValid)
                {
                    return mathcInfohorizontal;
                }
                else
                {
                    return matchInfoVertical;
                }
            }

            return mathcInfo;
        }

        public float DestroyGems(List<BaseGem> matches = null, bool moveToPivot = false)
        {
            Vector3 pivotPosition = Vector3.zero;
            float maxDuration = 0;

            if (matches == null)
            {
                matches = GemBoard.GetList();
                moveToPivot = false;
            }
            else if (moveToPivot && matches.Count > 0)
            {
                pivotPosition = GetWorldPosition(matches[0].position);
            }


            foreach (BaseGem gem in matches)
            {
                GemBoard[gem.position.x, gem.position.y] = null;
                float duration = gem.Matched();

                if (moveToPivot)
                {
                    duration = Mathf.Max(duration, gem.MoveTo(pivotPosition, GameController.Instance.FallSpeed));
                }

                if (duration > maxDuration)
                {
                    maxDuration = duration;
                }

                Destroy(gem.gameObject, maxDuration);
            }

            return maxDuration;
        }

        public void ShuffleBoard()
        {
            StartCoroutine(ShuffleBoard_Coroutine());
        }

        #endregion

        #region PRIVATE_METHODS

        private void EnableUpdateBoard(bool enable)
        {
            IsUpdatingBoard = enable;
            HintController.paused = enable;
            TouchController.Instance.IsDisabled = enable;
        }

        private BaseGem CreateGem(int x, int y, GemData type, Vector3 worldPosition, float delay, out float creatingDuration)
        {
            BaseGem gem = Instantiate(gemPrefab, worldPosition, Quaternion.identity, transform).GetComponent<BaseGem>();
            gem.SetPosition(new Vector2Int(x, y));
            gem.SetType(type);
            creatingDuration = gem.Creating(delay);

            return gem;
        }

        private BaseGem CreateRandomGem(int x, int y, Vector3 worldPosition, float delay, out float creatingDuration)
        {
            return CreateGem(x, y, GameData.RandomGem(), worldPosition, delay, out creatingDuration);
        }

        private BaseGem CreateRandomGem(int x, int y, Vector3 worldPosition,float delay)
        {
            return CreateRandomGem(x, y, worldPosition, delay, out float _);
        }

        #endregion

        #region COROUTINES

        private IEnumerator ShuffleBoard_Coroutine()
        {
            yield return new WaitForSeconds(.25f);
            GemBoard = Miscellaneous.ShuffleMatrix(GemBoard);
            float maxDuration = 0;

            for (int indexY = 0; indexY < Height; ++indexY)
            {
                for (int indexX = 0; indexX < Width; ++indexX)
                {
                    GemBoard[indexX, indexY].SetPosition(new Vector2Int(indexX, indexY));
                    Vector3 target = GetWorldPosition(GemBoard[indexX, indexY].position);
                    float speed = GameController.Instance.FallSpeed * (GemBoard[indexX, indexY].transform.position - GetWorldPosition(GemBoard[indexX, indexY].position)).magnitude / 4;
                    float duration = GemBoard[indexX, indexY].MoveTo(target, speed);

                    if (duration > maxDuration)
                    {
                        maxDuration = duration;
                    }
                }
            }

            yield return new WaitForSeconds(maxDuration);
        }

        private IEnumerator SwapGems_Coroutine(BaseGem from, BaseGem to)
        {
            float durationFrom = from.MoveTo(GetWorldPosition(to.position), GameController.Instance.SwapSpeed);
            float durationTo = to.MoveTo(GetWorldPosition(from.position), GameController.Instance.SwapSpeed);
            yield return new WaitForSeconds(Mathf.Max(durationFrom, durationTo));
            SwapGems(from, to);
        }

        private IEnumerator TryMatch_Coroutine(BaseGem from, BaseGem to)
        {
            EnableUpdateBoard(true);
            yield return StartCoroutine(SwapGems_Coroutine(from, to));

            MatchInfo matchFrom = from.GetMatch();
            MatchInfo matchTo = to.GetMatch();

            if (!(matchFrom.isValid || matchTo.isValid))
            {
                yield return StartCoroutine(SwapGems_Coroutine(from, to));
                EnableUpdateBoard(false);
            }
            else
            {
                List<Vector2Int> fallPositions = new List<Vector2Int>();
                List<MatchInfo> matches = new List<MatchInfo>
                {
                    matchFrom,
                    matchTo
                };

                if (from.type == GemType.Special)
                {
                    foreach (MatchInfo specialMatch in matchFrom.specialMatches)
                    {
                        matches.Add(specialMatch);
                    }
                }

                if (to.type == GemType.Special)
                {
                    foreach (MatchInfo specialMatch in matchTo.specialMatches)
                    {
                        matches.Add(specialMatch);
                    }
                }

                foreach (var matchInfo in new List<MatchInfo>(matches))
                {
                    if (matchInfo.isValid)
                    {
                        fallPositions = MatchInfo.JoinFallPositions(fallPositions, matchInfo.GetFallPositions());
                    }
                    else
                    {
                        matches.Remove(matchInfo);
                    }
                }

                yield return StartCoroutine(DestroyMatchedGems(matches));
                yield return StartCoroutine(FallGems(fallPositions));

                UpdateBoard();
            }
        }

        private IEnumerator UpdateBoard_Coroutine()
        {
            EnableUpdateBoard(true);

            yield return StartCoroutine(FindChainMatches_Coroutine());

            if (GameController.Instance.TimeLeft <= 0)
            {
                EnableUpdateBoard(false);
                yield break;
            }

            HintController.FindHints();
            if (!HintController.hasHints)
            {
                yield return StartCoroutine(ShuffleBoard_Coroutine());
                UpdateBoard();
            }
            else
            {
                EnableUpdateBoard(false);
                MatchCounter = 0;
                OnEndUpdatingBoard?.Invoke();
            }
        }

        // Check for matches in all Board
        private IEnumerator FindChainMatches_Coroutine()
        {
            List<BaseGem> gems = GemBoard.GetList();
            List<MatchInfo> matchInfos = new List<MatchInfo>();

            while (gems.Count > 0)
            {
                BaseGem current = gems[0];
                gems.Remove(current);

                if (current.type == GemType.Special)
                {
                    continue;
                }

                MatchInfo matchInfo = current.GetMatch();
                if (matchInfo.isValid)
                {
                    matchInfo.matches.ForEach(gem => gems.Remove(gem));

                    MatchInfo matchInfoSameType = matchInfos.Find(matchInfoData => matchInfoData.pivot.type == matchInfo.pivot.type);

                    if (matchInfoSameType != null)
                    {
                        matchInfoSameType = MatchInfo.JoinCrossedMatches(matchInfoSameType, matchInfo);

                        if (matchInfoSameType.isValid)
                        {
                            matchInfos.Add(matchInfoSameType);
                            continue;
                        }
                    }

                    matchInfos.Add(matchInfo);
                }
            }

            if (matchInfos.Count > 0)
            {
                List<Vector2Int> fallPositions = new List<Vector2Int>();
                List<MatchInfo> matchesToDestroy = new List<MatchInfo>();

                foreach (MatchInfo matchInfo in matchInfos)
                {
                    matchesToDestroy.Add(matchInfo);
                    fallPositions = MatchInfo.JoinFallPositions(fallPositions, matchInfo.GetFallPositions());
                }

                yield return StartCoroutine(DestroyMatchedGems(matchesToDestroy));
                yield return StartCoroutine(FallGems(fallPositions));
                yield return StartCoroutine(FindChainMatches_Coroutine());
            }
        }

        // Update position of Gems and create new ones
        private IEnumerator FallGems(List<Vector2Int> fallPositions)
        {
            float maxDuration = 0;
            float delay = 0;

            foreach (Vector3Int fall in fallPositions)
            {
                int fallY = 0;
                for (int indexY = fall.y; indexY < Height; ++indexY)
                {
                    BaseGem gem = GetGem(fall.x, indexY);

                    if (gem)
                    {
                        float duration = gem.MoveTo(GetWorldPosition(new Vector2Int(fall.x, indexY - fallY)), GameController.Instance.FallSpeed);
                        gem.SetPosition(new Vector2Int(fall.x, indexY - fallY));

                        if (duration > maxDuration)
                        {
                            maxDuration = duration;
                        }
                    }
                    else
                    {
                        fallY++;
                    }
                }

                for (int indexY = Height - 1; indexY >= Height - fallY; --indexY)
                {
                    Vector3 worldPosition = GetWorldPosition(new Vector2Int(fall.x, indexY + 1));
                    BaseGem newGem = CreateRandomGem(fall.x, indexY, worldPosition, delay);

                    float duration = newGem.MoveTo(GetWorldPosition(newGem.position), GameController.Instance.FallSpeed, delay);
                    delay = duration;

                    if (duration > maxDuration)
                    {
                        maxDuration = duration;
                    }
                }
            }

            yield return new WaitForSeconds(maxDuration);
        }

        private IEnumerator DestroyMatchedGems(List<MatchInfo> matches)
        {
            float maxDuration = 0;
            int score = 0;

            foreach (MatchInfo matchInfo in matches)
            {

                float duration = DestroyGems(matchInfo.matches, matchInfo.pivot);

                if (matchInfo.type == MatchType.Cross && !(matchInfo.pivot is BlenderGem))
                {
                    float newGemDuration;
                    BaseGem newGem = CreateGem(
                        matchInfo.pivot.position.x,
                        matchInfo.pivot.position.y,
                        GameData.GemOfType(GemType.Special),
                        GetWorldPosition(matchInfo.pivot.position + Vector2Int.up),
                        0, out newGemDuration
                    );

                    newGem.MoveTo(GetWorldPosition(newGem.position),GameController.Instance.FallSpeed);
                    duration += newGemDuration;
                }

                if (duration > maxDuration)
                {
                    maxDuration = duration;
                }

                MatchCounter++;
                if (matchInfo.pivot is BlenderGem)
                {
                    MatchCounter = 5;
                }

                score += matchInfo.GetScore();
            }

            GameController.Instance.Score += score * MatchCounter;
            UIController.ShowMessage($"{ GameData.GetComboMessage(MatchCounter - 1) }");
            SoundController.PlaySfx(GameData.GetAudioClip("match"));

            yield return new WaitForSeconds(maxDuration / 2);
        }

        #endregion
    }
}
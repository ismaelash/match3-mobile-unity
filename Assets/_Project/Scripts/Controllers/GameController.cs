using Match3.Commons;
using Match3.Screens;
using System.Collections;
using UnityEngine;

namespace Match3.Controllers
{
    public class GameController : Singleton<GameController>
    {
        #region ENUMS

        public enum GameState
        {
            None,
            Gameplay
        }

        #endregion

        #region VARIABLES

        [Header("Game Settings")]
        [SerializeField] private GameData gameData;
        [SerializeField] [Tooltip("Attempts matches moves")] private int attempMatchesLimit = 10;
        [SerializeField] [Tooltip("Seconds")] private int timeTotalGameplay = 120;
        [SerializeField] private float swapSpeed;
        [SerializeField] private float fallSpeed;
        [SerializeField] private bool preventInitialMatches;
        [SerializeField] private GameplayScreen gameplayScreen;

        // Privates
        private float cameraWidth = 7;
        private Coroutine changeGem;
        private Coroutine gameOver;
        private int currentGoalScore;
        private int score;
        private float timeLeft;
        private int attempMatchesCount;
        private GameState state = GameState.None;

        // Properties
        public int ScoreCurrent
        {
            get => score;
            set
            {
                score = value;
                gameplayScreen.UpdateScore();
            }
        }

        public int GoalScoreCurrent
        {
            get => currentGoalScore;
            set
            {
                currentGoalScore = value;
                gameplayScreen.UpdateScore();
            }
        }

        public float TimeLeft
        {
            get => timeLeft;
            set
            {
                timeLeft = Mathf.Max(value, 0);
                gameplayScreen.UpdateTimeLeft(timeLeft);
            }
        }

        public int AttempMatchesCount
        {
            get => attempMatchesCount;
            set
            {
                attempMatchesCount = value;
                gameplayScreen.UpdateMovesCount();
            }
        }

        public float SwapSpeed { get => swapSpeed; set => swapSpeed = value; }
        public float FallSpeed { get => fallSpeed; set => fallSpeed = value; }
        public bool PreventInitialMatches { get => preventInitialMatches; set => preventInitialMatches = value; }
        public GameData GameData { get => gameData; set => gameData = value; }
        public int AttempMatchesLimit { get => attempMatchesLimit; }

        #endregion

        #region MONOBEHAVIOUR_METHODS

        private void Start()
        {
            cameraWidth = BoardController.Instance.Width + (Camera.main.aspect * 2);
            Miscellaneous.SetCameraOrthographicSizeByWidth(Camera.main, cameraWidth);
            UIController.Instance.ShowMainScreen();
        }

        private void Update()
        {
            if (state == GameState.Gameplay)
            {
                TimeLeft -= Time.deltaTime;
                if (ScoreCurrent >= GoalScoreCurrent)
                {
                    GoalScoreCurrent += GoalScoreCurrent + GoalScoreCurrent / 2;
                }

                if (TimeLeft <= 0)
                {
                    GameOver();
                }
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                BoardController.Instance.ShuffleBoard();
            }
#endif
        }

        #endregion

        #region PUBLIC_METHODS

        public void StartGame()
        {
            StartCoroutine(StartGame_Coroutine());
        }

        public void ShowGemMenu(bool show = true)
        {
            if (!show)
            {
                if (changeGem != null)
                {
                    StopCoroutine(changeGem);
                    changeGem = null;
                }
            }
        }

        #endregion

        #region PRIVATE_METHODS

        private void GameOver()
        {
            if (gameOver == null)
            {
                gameOver = StartCoroutine(GameOver_Coroutine());
            }
        }

        #endregion

        #region COROUTINES

        private IEnumerator StartGame_Coroutine()
        {
            AttempMatchesCount = 0;
            ScoreCurrent = 0;
            GoalScoreCurrent = 50;
            TimeLeft = timeTotalGameplay;
            BoardController.Instance.MatchCounter = 0;
            gameplayScreen.Show();
            yield return new WaitForSeconds(1f);
            TouchController.Instance.IsDisabled = true;
            yield return new WaitForSeconds(BoardController.Instance.CreateBoard());
            state = GameState.Gameplay;
            BoardController.Instance.UpdateBoard();
        }

        private IEnumerator GameOver_Coroutine()
        {
            yield return new WaitUntil(() => !BoardController.Instance.IsUpdatingBoard);

            if (TimeLeft > 0)
            {
                gameOver = null;
                yield break;
            }

            TouchController.Instance.IsDisabled = true;
            state = GameState.None;
            yield return new WaitForSeconds(BoardController.Instance.DestroyGems() + .5f);
            UIController.Instance.ShowMainScreen();
            gameOver = null;
        }

        #endregion
    }
}
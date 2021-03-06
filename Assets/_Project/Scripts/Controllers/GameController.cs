﻿using Match3.Commons;
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

        [Header("Screens")]
        [SerializeField] private MainScreen mainScreen;
        [SerializeField] private GameplayScreen gameplayScreen;

        [Header("Game Settings")]
        [SerializeField] private GameData gameData;
        [SerializeField] [Tooltip("Attempts matches moves")] private int attempMatchesLimit = 100;
        [SerializeField] [Tooltip("Seconds")] private int timeTotalGameplay = 120;
        [SerializeField] private float swapSpeed;
        [SerializeField] private float fallSpeed;
        [SerializeField] private bool preventInitialMatches;


        private float cameraWidth;
        private Coroutine gameOver;
        private int currentGoalScore;
        private int score;
        private float timeLeft;
        private int attempMatchesCount;
        private GameState state;

        public int ScoreCurrent
        {
            get => score;
            set
            {
                score = value;
                EventController.Instance.OnNewScoreAction?.Invoke(value);
            }
        }

        public int GoalScoreCurrent
        {
            get => currentGoalScore;
            set
            {
                currentGoalScore = value;
                EventController.Instance.OnNewGoalScore?.Invoke(value);
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
                EventController.Instance.OnAttempMatch?.Invoke(value);

                if(attempMatchesCount == attempMatchesLimit)
                {
                    EventController.Instance.OnAttempMatchLimit?.Invoke();
                }
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
            EventController.Instance.OnEndGameplayAction += GameOver;
            EventController.Instance.OnAttempMatchLimit += GameOver;
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
                    EventController.Instance.OnEndGameplayAction?.Invoke();
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
            EventController.Instance.OnStartGameAction?.Invoke();
        }

        private IEnumerator GameOver_Coroutine()
        {
            yield return new WaitForSeconds(.5f);
            yield return new WaitUntil(() => !BoardController.Instance.IsUpdatingBoard);
            TouchController.Instance.IsDisabled = true;
            state = GameState.None;
            yield return new WaitForSeconds(BoardController.Instance.DestroyGems() + .5f);
            mainScreen.Show();
            gameOver = null;
        }

        #endregion
    }
}
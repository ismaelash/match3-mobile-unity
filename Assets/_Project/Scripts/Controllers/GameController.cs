using System.Collections;
using UnityEngine;
using Utils;

namespace IsmaelNascimento.Controllers
{
    public class GameController : SingletonMonoBehaviour<GameController>
    {
        #region ENUMS

        public enum GameState
        {
            None,
            Gameplay
        }

        #endregion

        #region VARIABLES

        [Header("Camera Settings")]
        [SerializeField] private float cameraWidth = 7;
        [SerializeField] private bool autoCameraWidth;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private GameObject gemMenu;


        [Header("Game Settings")]
        [SerializeField] private GameData gameData;
        [SerializeField] [Tooltip("Seconds")] private int timeTotalGameplay = 120;
        [SerializeField] private float swapSpeed;
        [SerializeField] private float fallSpeed;
        [SerializeField] private bool preventInitialMatches;

        // Privates
        private Coroutine changeGem;
        private Coroutine gameOver;
        private int currentGoalScore;
        private int score;
        private float timeLeft;
        private GameState state = GameState.None;

        // Properties
        public int Score
        {
            get { return score; }
            set
            {
                UIController.UpdateComboScore(value - score, BoardController.matchCounter);
                score = value;
                UIController.UpdateScore(score);
            }
        }

        public int CurrentGoalScore
        {
            get { return currentGoalScore; }
            set
            {
                currentGoalScore = value;
                UIController.UpdateGoalScore(currentGoalScore);
            }
        }

        public float TimeLeft
        {
            get { return timeLeft; }
            set
            {
                timeLeft = Mathf.Max(value, 0);
                UIController.UpdateTimeLeft(timeLeft);
            }
        }

        public float SwapSpeed { get => swapSpeed; set => swapSpeed = value; }
        public float FallSpeed { get => fallSpeed; set => fallSpeed = value; }
        public bool PreventInitialMatches { get => preventInitialMatches; set => preventInitialMatches = value; }

        #endregion

        #region MONOBEHAVIOUR_METHODS

        private void Start()
        {
            if (autoCameraWidth)
            {
                cameraWidth = BoardController.width + (Camera.main.aspect * 2);
            }

            Miscellaneous.SetCameraOrthographicSizeByWidth(Camera.main, cameraWidth);
            float backgroundWidth = background.sprite.bounds.size.x;
            float backgroundHeight = background.sprite.bounds.size.y;

            background.transform.localScale = Vector3.one *
                Mathf.Max(
                    cameraWidth / backgroundWidth,
                    Camera.main.orthographicSize * 2 / backgroundHeight
                );

            gemMenu.transform.localScale = Vector3.one * 2 * (cameraWidth / 7f);
            UIController.ShowMainScreen();
            SoundController.PlayMusic(GameData.GetAudioClip("bgm"), 1);
        }

        private void Update()
        {
            if (state == GameState.Gameplay)
            {
                TimeLeft -= Time.deltaTime;
                if (Score >= CurrentGoalScore)
                {
                    CurrentGoalScore += CurrentGoalScore + CurrentGoalScore / 2;
                    TimeLeft = timeTotalGameplay;
                }

                if (TimeLeft <= 0)
                {
                    GameOver();
                }
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(BoardController.Instance.ShuffleBoard());
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
            Score = 0;
            CurrentGoalScore = 50;
            TimeLeft = timeTotalGameplay;
            BoardController.matchCounter = 0;
            UIController.ShowGameScreen();
            yield return new WaitForSeconds(1f);
            TouchController.Instance.IsDisabled = true;
            yield return new WaitForSeconds(BoardController.CreateBoard());
            state = GameState.Gameplay;
            BoardController.UpdateBoard();
        }

        private IEnumerator GameOver_Coroutine()
        {
            yield return new WaitUntil(() => !BoardController.updatingBoard);

            if (TimeLeft > 0)
            {
                gameOver = null;
                yield break;
            }

            TouchController.Instance.IsDisabled = true;
            state = GameState.None;
            UIController.ShowMessage("Game Over");
            yield return new WaitForSeconds(BoardController.DestroyGems() + .5f);
            UIController.ShowMainScreen();
            gameOver = null;
        }

        #endregion
    }
}
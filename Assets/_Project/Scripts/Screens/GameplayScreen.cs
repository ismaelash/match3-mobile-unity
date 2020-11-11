using Match3.Commons;
using Match3.Controllers;
using Match3.Interfaces;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Match3.Screens
{
    public class GameplayScreen : MonoBehaviour, IScreen
    {
        #region VARIABLES

        [SerializeField] private GameObject containerElementsUI;
        [SerializeField] private Button menuButton;
        [SerializeField] private TextMeshProUGUI timeLeftText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI movesCountText;
        [SerializeField] private MainScreen mainScreen;

        #endregion

        #region MONOBEHAVIOUR_METHODS

        private void Start()
        {
            menuButton.onClick.AddListener(OnButtonMenuClicked);
            EventController.Instance.OnStartGameAction += OnStartGame_Action;
            EventController.Instance.OnNewScoreAction += OnUpdateScores_Action;
            EventController.Instance.OnNewGoalScore += OnUpdateScores_Action;
            EventController.Instance.OnAttempMatch += OnAttempMatch_Action;
            EventController.Instance.OnGemMatch += OnGemMatch_Action;
        }

        #endregion

        #region PUBLIC_METHODS

        public void UpdateTimeLeft(float timeLeft)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeLeft);
            string minutes = timeSpan.Minutes.ToString("D2");
            string seconds = timeSpan.Seconds.ToString("D2");
            timeLeftText.text = $"{ minutes }:{ seconds }";
        }

        public void Show()
        {
            mainScreen.gameObject.SetActive(false);
            gameObject.SetActive(true);
            SoundController.Instance.StopMusic();
        }

        #endregion

        #region PRIVATE_METHODS

        private void OnStartGame_Action()
        {
            UpdateTimeLeft(GameController.Instance.TimeLeft);
            OnUpdateScores_Action(0);
            OnAttempMatch_Action(0);
            containerElementsUI.SetActive(true);
        }

        private void OnUpdateScores_Action(int score)
        {
            scoreText.text = $"{ GameController.Instance.ScoreCurrent } / {GameController.Instance.GoalScoreCurrent}";
        }

        private void OnAttempMatch_Action(int attemptCount)
        {
            movesCountText.text = $"{ GameController.Instance.AttempMatchesCount } / {GameController.Instance.AttempMatchesLimit}";
        }

        private void OnGemMatch_Action(GemData gemData)
        {
            Debug.Log($"{gemData.type} matched!");
        }

        private void OnButtonMenuClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        #endregion
    }
}
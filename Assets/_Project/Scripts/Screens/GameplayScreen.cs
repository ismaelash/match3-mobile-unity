using IsmaelNascimento.Controllers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IsmaelNascimento.Screens
{
    public class GameplayScreen : MonoBehaviour
    {
        #region VARIABLES

        [Header("UI Elements")]
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

        public void UpdateScore()
        {
            scoreText.text = $"{ GameController.Instance.ScoreCurrent } / {GameController.Instance.GoalScoreCurrent}";
        }

        public void UpdateMovesCount()
        {
            movesCountText.text = $"{ GameController.Instance.AttempMatchesCount } / {GameController.Instance.AttempMatchesLimit}";
        }

        public void Show()
        {
            UpdateTimeLeft(GameController.Instance.TimeLeft);
            UpdateMovesCount();
            UpdateScore();

            mainScreen.gameObject.SetActive(false);
            gameObject.SetActive(true);
            SoundController.Instance.StopMusic();

        }

        #endregion

        #region PRIVATE_METHODS

        private void OnButtonMenuClicked()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        #endregion
    }
}
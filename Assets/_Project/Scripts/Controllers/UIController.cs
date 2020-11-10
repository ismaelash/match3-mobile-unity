using Utils;
using TMPro;
using UnityEngine;
using System;

namespace IsmaelNascimento.Controllers
{
    public class UIController : SingletonMonoBehaviour<UIController>
    {
        #region VARIABLES

        [Header("Screens")]
        [SerializeField] private GameObject mainScreen;
        [SerializeField] private GameObject gameScreen;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeLeftText;

        #endregion

        #region PUBLIC_METHODS

        public void ShowMainScreen()
        {
            gameScreen.SetActive(false);
            mainScreen.SetActive(true);
        }

        public void ShowGameScreen()
        {
            UpdateTimeLeft(GameController.Instance.TimeLeft);
            mainScreen.SetActive(false);
            gameScreen.SetActive(true);
        }

        public void UpdateScore()
        {
            scoreText.text = $"{ GameController.Instance.ScoreCurrent } / {GameController.Instance.GoalScoreCurrent}";
        }

        public void UpdateTimeLeft(float timeLeft)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeLeft);
            string minutes = timeSpan.Minutes.ToString("D2");
            string seconds = timeSpan.Seconds.ToString("D2");
            timeLeftText.text = $"{ minutes }:{ seconds }";
        }

        #endregion
    }
}
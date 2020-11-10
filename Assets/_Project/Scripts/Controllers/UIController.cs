using Utils;
using TMPro;
using UnityEngine;
using System;
using IsmaelNascimento.Commons;

namespace IsmaelNascimento.Controllers
{
    public class UIController : Singleton<UIController>
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
            SoundController.Instance.PlayMusic(GameController.Instance.GameData.GetAudioClip(Constants.backgroundMusicAudioclipName));
        }

        public void ShowGameScreen()
        {
            UpdateTimeLeft(GameController.Instance.TimeLeft);
            mainScreen.SetActive(false);
            gameScreen.SetActive(true);
            SoundController.Instance.StopMusic();

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
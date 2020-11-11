using Match3.Commons;
using Match3.Controllers;
using Match3.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.Screens
{
    public class MainScreen : MonoBehaviour, IScreen
    {
        #region VARIABLES

        [SerializeField] private GameplayScreen gameplayScreen;
        [SerializeField] private Button playButton;

        #endregion

        #region MONOBEHAVIOUR_METHODS

        private void Start()
        {
            playButton.onClick.AddListener(OnButtonPlayClicked);
            Show();
        }

        #endregion

        #region PUBLIC_METHODS

        public void Show()
        {
            gameplayScreen.gameObject.SetActive(false);
            gameObject.SetActive(true);
            SoundController.Instance.PlayMusic(GameController.Instance.GameData.GetAudioClip(Constants.backgroundMusicAudioclipName));
        }

        #endregion

        #region PRIVATE_METHODS

        private void OnButtonPlayClicked()
        {
            GameController.Instance.StartGame();
        }

        #endregion
    }
}
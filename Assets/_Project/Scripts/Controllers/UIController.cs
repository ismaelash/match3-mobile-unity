using Utils;
using UnityEngine;
using IsmaelNascimento.Commons;

namespace IsmaelNascimento.Controllers
{
    public class UIController : Singleton<UIController>
    {
        #region VARIABLES

        [Header("Screens")]
        [SerializeField] private GameObject mainScreen;
        [SerializeField] private GameObject gameScreen;

        #endregion

        #region PUBLIC_METHODS

        public void ShowMainScreen()
        {
            gameScreen.SetActive(false);
            mainScreen.SetActive(true);
            SoundController.Instance.PlayMusic(GameController.Instance.GameData.GetAudioClip(Constants.backgroundMusicAudioclipName));
        }

        #endregion
    }
}
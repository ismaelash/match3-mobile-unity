using IsmaelNascimento.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace IsmaelNascimento.Screens
{
    public class MainScreen : MonoBehaviour
    {
        #region VARIABLES

        [SerializeField] private Button playButton;

        #endregion

        #region MONOBEHAVIOUR_METHODS

        private void Start()
        {
            playButton.onClick.AddListener(OnButtonPlayClicked);
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
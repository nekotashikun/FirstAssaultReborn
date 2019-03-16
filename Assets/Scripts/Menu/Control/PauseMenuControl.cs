using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu.Control
{
    public class PauseMenuControl : MonoBehaviour
    {
        [Header("Buttons"), SerializeField]
        private Button _backButton;
        [SerializeField]
        private Button _quitButton;

        private InGameMenuController _menuController;

        void Start()
        {
            _menuController = GetComponentInParent<InGameMenuController>();
            _backButton.onClick.AddListener(GoToInGame);
            _quitButton.onClick.AddListener(GoToMainMenu);
        }

        public void GoToInGame()
        {
            _menuController.SendMessage(MenuType.HUD);
            DestroyPauseMenu();
        }

        public void GoToMainMenu()
        {
            Debug.Log("Going Back to Main Menu...");
            _menuController.SendMessage(MenuType.EXIT);
            DestroyPauseMenu();
        }

        public void DestroyPauseMenu()
        {
            Destroy(this.gameObject);
        }
    }
}

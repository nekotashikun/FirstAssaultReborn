using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu.Controls
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
            _menuController = InGameMenuController.Instance;
            _backButton.onClick.AddListener(HidePauseMenu);
            _quitButton.onClick.AddListener(ExitLevel);
        }

        public void HidePauseMenu()
        {
            _menuController.SendMessage(MenuType.GAME);
            Destroy(gameObject);
        }

        public void ExitLevel()
        {
            Debug.Log("Going Back to Main Menu...");
            _menuController.Exit();
        }
    }
}

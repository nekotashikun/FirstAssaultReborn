using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Controls
{
    public class MainMenuControl : MonoBehaviour
    {
        [Header("Buttons"), SerializeField]
        private Button _optionsButton;

        private MenuController _menuController;

        void Start()
        {
            _menuController = MenuController.Instance;
            _optionsButton.onClick.AddListener(GoToOptions);
        }

        public void GoToOptions()
        {
            _menuController.EnterOptionsMenu();
            DestroyMainMenu();
        }

        public void DestroyMainMenu()
        {
            Destroy(gameObject);
        }

    }
}

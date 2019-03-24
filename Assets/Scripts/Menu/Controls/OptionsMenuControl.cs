using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Controls
{
    public class OptionsMenuControl : MonoBehaviour
    {
        [Header("Buttons"), SerializeField]
        private Button _backButton;

        private MenuController _menuController;

        void Start()
        {
            _menuController = MenuController.Instance;
            _backButton.onClick.AddListener(SendMainMenuMessage);
        }

        public void SendMainMenuMessage()
        {
            Debug.Log("Going Back to Main Menu...");
            _menuController.EnterMainMenu();
            DestroyOptionsMenu();
        }

        public void DestroyOptionsMenu()
        {
            Destroy(gameObject);
        }
    }
}

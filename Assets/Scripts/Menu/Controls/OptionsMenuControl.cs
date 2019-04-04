using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu.Controls
{
    public class OptionsMenuControl : MonoBehaviour
    {
        [Header("Controller"), SerializeField]
        private MenuController _menuController;

        private string _currentScene;

        void Start()
        {
            _currentScene = _menuController.GetCurrentSceneName();
        }

        public void EnterPreviousMenu()
        {
            _currentScene = _menuController.GetCurrentSceneName();

            if (_currentScene != "MainMenu")
                _menuController.SendMenuMessage(MenuType.PAUSE);
            else
                _menuController.SendMenuMessage(MenuType.MAIN);
        }

    }
}

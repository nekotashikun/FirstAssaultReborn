using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.MessageBroker;

namespace Menu.Controls
{
    public class OptionsMenuControl : MonoBehaviour
    {

        [Header("Parent Controller"), SerializeField]
        private MenuController _controller;

        public void EnterPreviousMenu()
        {

            if (SceneManager.GetActiveScene().name.Equals(_controller.GetMainMenuScene()))
            {
                GameMessenger.Instance.SendMessageOfType(new MenuMessage(MenuType.Main));
            }
            else
            {
                GameMessenger.Instance.SendMessageOfType(new MenuMessage(MenuType.Pause));
            }
        }

    }
}

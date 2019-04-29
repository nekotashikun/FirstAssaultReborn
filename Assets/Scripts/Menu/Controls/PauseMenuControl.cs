using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.MessageBroker;

namespace Menu.Controls
{
    public class PauseMenuControl : MonoBehaviour
    {

        public void HidePauseMenu()
        {
            GameMessenger.Instance.SendMessageOfType(new MenuMessage(MenuType.Game));
        }

    }
}

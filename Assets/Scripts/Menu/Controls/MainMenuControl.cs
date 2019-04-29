using Menu;
using UnityEngine;
using Utilities.MessageBroker;

namespace Menu.Controls
{
    public class MainMenuControl : MonoBehaviour
    {
        
        public void EnterConnectionMenu()
        {
            GameMessenger.Instance.SendMessageOfType(new MenuMessage(MenuType.Connect));
        }

    }
}
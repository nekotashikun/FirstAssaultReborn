using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.MessageBroker;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {
        [Header("UI Prefabs"), SerializeField]
        private GameObject _optionsPanel;
        [SerializeField]
        private GameObject _mainMenuPanel;

        private GameMessenger _messenger;
        private MenuMessage _message;
        private GameObject _mainCanvas;
      
        void Start()
        {
            _messenger = GameMessenger.Instance;
            _messenger.RegisterSubscriberToMessageTypeOf<MenuMessage>(HandleMessage);
            _message.MenuState = MenuType.MAIN;
            _mainCanvas = GameObject.Find("Canvas");

            EnterMainMenu();
        }

        private void HandleMessage(MenuMessage incomingMessage)
        {
            switch (incomingMessage.MenuState)
            {
                case MenuType.MAIN:
                    {
                        Instantiate(_mainMenuPanel, _mainCanvas.transform, false);
                        _mainMenuPanel.SetActive(true);
                        break;
                    }
                case MenuType.OPTIONS:
                    {
                        Instantiate(_optionsPanel, _mainCanvas.transform, false);
                        _optionsPanel.SetActive(true);
                        break;
                    }
                default:
                    {
                        Debug.Log("Invalid Menu Request");
                        break;
                    }
            }
        }

        public void SendMessage(MenuType nextMenu)
        {
            Debug.Log("Sending Menu Message: " + nextMenu.ToString());
            _message.MenuState = nextMenu;
            _messenger.SendMessageOfType<MenuMessage>(_message);
        }

        public void EnterOptionsMenu()
        {
            SendMessage(MenuType.OPTIONS);
        }

        public void EnterMainMenu()
        {
            SendMessage(MenuType.MAIN);
        }

        public void OnDestroy()
        {
            _messenger.UnRegisterAllMessagesForObject(this);
        }
    }
}
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
            _message.MessageNumber = MenuType.NONE;
            _mainCanvas = GameObject.Find("Canvas");

            if (SceneManager.GetActiveScene().buildIndex == 1)
                EnterMainMenu();
                
        }

        private void HandleMessage(MenuMessage incomingMessage)
        {
            switch (incomingMessage.MessageNumber)
            {
                case MenuType.NONE:
                    {
                        if(_mainMenuPanel.activeSelf)
                            _mainMenuPanel.SetActive(false);
                        if (_optionsPanel.activeSelf)
                            _optionsPanel.SetActive(false);
                        break;
                    }
                case MenuType.MAIN:
                    {
                        if(GameObject.Find("MainMenuPanel") || GameObject.Find("MainMenuPanel\\(Clone\\)"))
                        Instantiate(_mainMenuPanel, _mainCanvas.transform, false);

                        _mainMenuPanel.SetActive(true);
                        break;
                    }
                case MenuType.OPTIONS:
                    {
                        Instantiate(_optionsPanel, _mainCanvas.transform, false);

                        _mainMenuPanel.SetActive(false);
                        _optionsPanel.SetActive(true);
                        break;
                    }
                case MenuType.PAUSE:
                    {
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
            _message.MessageNumber = nextMenu;
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

        public void HideMainMenu()
        {

        }

        public void OnDestroy()
        {
            _messenger.UnRegisterAllMessagesForObject(this);
        }
    }
}
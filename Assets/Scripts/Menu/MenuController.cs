using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.MessageBroker;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {
        private static MenuController _instance;

        public static MenuController Instance { get { return _instance; } }
        
        [Header("Required Game Objects"), SerializeField]
        private GameObject _optionsPanelPrefab;
        [SerializeField]
        private GameObject _mainMenuPanelPrefab;
        [SerializeField]
        private GameObject _mainCanvas;

        private GameMessenger _messenger;
      
        void Start()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            _messenger = GameMessenger.Instance;
            _messenger.RegisterSubscriberToMessageTypeOf<MenuMessage>(HandleMessage);

            EnterMainMenu();
        }

        private void HandleMessage(MenuMessage incomingMessage)
        {
            switch (incomingMessage.MenuState)
            {
                case MenuType.MAIN:
                    {
                        Instantiate(_mainMenuPanelPrefab, _mainCanvas.transform, false);
                        _mainMenuPanelPrefab.SetActive(true);
                        break;
                    }
                case MenuType.OPTIONS:
                    {
                        Instantiate(_optionsPanelPrefab, _mainCanvas.transform, false);
                        _optionsPanelPrefab.SetActive(true);
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
            var _message = new MenuMessage(nextMenu);
            _messenger.SendMessageOfType(_message);
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
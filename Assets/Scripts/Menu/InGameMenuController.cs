using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using Utilities.MessageBroker;

namespace Menu
{
    public class InGameMenuController : MonoBehaviour
   {
        private static InGameMenuController _instance;

        public static InGameMenuController Instance { get { return _instance; } }

        [Header("Required Prefabs"), SerializeField]
        private GameObject _pausePanelPrefab;
        [Header("Canvas Assignment"),SerializeField]
        private GameObject _mainCanvas;

        private GameMessenger _messenger;
        private MenuType _state;

        void Start()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            _messenger = GameMessenger.Instance;
            _messenger.RegisterSubscriberToMessageTypeOf<MenuMessage>(HandleMessage);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && _state != MenuType.PAUSE)
            {
                SendMessage(MenuType.PAUSE);
            }
        }

        private void HandleMessage(MenuMessage incomingMessage)
        {
            switch (incomingMessage.MenuState)
            {
                case MenuType.PAUSE:
                    {
                        Instantiate(_pausePanelPrefab, _mainCanvas.transform, false);
                        _pausePanelPrefab.SetActive(true);
                        _state = MenuType.PAUSE;
                        break;
                    }
                default:
                    {
                        _state = MenuType.GAME;
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

        public void Exit()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void OnDestroy()
        {
            _messenger.UnRegisterAllMessagesForObject(this);
        }
    }
}

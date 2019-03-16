using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.MessageBroker;

namespace Menu
{
    public class InGameMenuController : MonoBehaviour
    {
        [Header("UI Prefabs"), SerializeField]
        private GameObject _pausePanel;
        [SerializeField]
        private GameObject _mainCanvas;

        private GameMessenger _messenger;
        private MenuMessage _message;
        //private GameObject _mainCanvas;

        void Start()
        {
            _messenger = GameMessenger.Instance;
            _messenger.RegisterSubscriberToMessageTypeOf<MenuMessage>(HandleMessage);
            _message.MenuState = MenuType.HUD;
            Debug.Log(_mainCanvas.name);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (_message.MenuState != MenuType.PAUSE)
                {
                    OpenPauseMenu();
                }
            }
        }

        private void HandleMessage(MenuMessage incomingMessage)
        {
            switch (incomingMessage.MenuState)
            {
                case MenuType.HUD:
                    {
                        break;
                    }
                case MenuType.PAUSE:
                    {
                        Instantiate(_pausePanel, _mainCanvas.transform, false);
                        _pausePanel.SetActive(true);
                        break;
                    }
                case MenuType.EXIT:
                    {
                        SceneManager.LoadScene(0);
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

        public void OpenPauseMenu()
        {
            SendMessage(MenuType.PAUSE);
        }

        public void OnDestroy()
        {
            _messenger.UnRegisterAllMessagesForObject(this);
        }
    }
}

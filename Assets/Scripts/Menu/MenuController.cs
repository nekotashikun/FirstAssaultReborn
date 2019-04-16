using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities.MessageBroker;

namespace Menu
{
    public class MenuController : MonoBehaviour
    {
        private static MenuController _instance;

        [Header("Required Game Objects"), SerializeField]
        private GameObject _mainCanvas;
        [SerializeField]
        private GameObject _mainMenuPanelPrefab;
        [SerializeField]
        private GameObject _optionsPanelPrefab;
        [SerializeField]
        private GameObject _pausePanelPrefab;

        private GameMessenger _messenger;
        private string _sceneName;

        private GameObject[] _menuPrefabs;
      
        void Start()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);

            _messenger = GameMessenger.Instance;
            _messenger.RegisterSubscriberToMessageTypeOf<MenuMessage>(HandleMessage);

            _sceneName = SceneManager.GetActiveScene().name;
            if (_sceneName != "MainMenu") return;
            else
                DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += ChangedScene;
            EditorSceneManager.activeSceneChanged += ChangedScene;

            _menuPrefabs = new GameObject[] { _mainMenuPanelPrefab, _optionsPanelPrefab, _pausePanelPrefab };

        }

        void Update()
        {
            if (_sceneName == "MainMenu" || !Input.GetKeyUp(KeyCode.Escape)) return;
            else
            {
                if (_sceneName == "MultiplayerMenu")
                    ReturnToMainMenu();
                else
                    SendMenuMessage(MenuType.PAUSE);

            }
        }

        private void HandleMessage(MenuMessage incomingMessage)
        {
            switch (incomingMessage.MenuState)
            {
                case MenuType.MAIN:
                    {
                        ResetPrefabs(_menuPrefabs[0]);
                        break;
                    }
                case MenuType.CONNECT:
                    {
                        ResetPrefabs();
                        SceneManager.LoadScene("MultiplayerMenu");
                        break;
                    }
                case MenuType.OPTIONS:
                    {
                        ResetPrefabs(_menuPrefabs[1]);
                        break;
                    }
                case MenuType.PAUSE:
                    {
                        ResetPrefabs(_menuPrefabs[2]);
                        break;
                    }
                default:
                    {
                        ResetPrefabs();
                        break;
                    }
            }
        }


        private void ResetPrefabs(GameObject keepAlive)
        {
            foreach (GameObject g in _menuPrefabs)
            {
                if (g != keepAlive)
                    g.SetActive(false);
                else
                    g.SetActive(true);
            }
        }

        private void ResetPrefabs()
        {
            foreach (GameObject g in _menuPrefabs)
                g.SetActive(false);
        }


        private void ChangedScene(Scene current, Scene next)
        {
            _sceneName = next.name;

            if (_sceneName == "MainMenu")
                EnterMainMenu();
        }

        public string GetCurrentSceneName()
        {
            return _sceneName;
        }

        public void SendMenuMessage(MenuType nextMenu)
        {
            Debug.Log("Sending Menu Message: " + nextMenu.ToString());
            var _message = new MenuMessage(nextMenu);
            _messenger.SendMessageOfType(_message);
        }

        public void EnterOptionsMenu()
        {
            SendMenuMessage(MenuType.OPTIONS);
        }

        public void EnterMainMenu()
        {
            SendMenuMessage(MenuType.MAIN);
        }

        public void Exit()
        {
            if (UnityEditor.EditorApplication.isPlaying)
                UnityEditor.EditorApplication.isPlaying = false;
            else
                Application.Quit();
        }

        public void ReturnToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void OnDestroy()
        {
            _messenger.UnRegisterAllMessagesForObject(this);
        }
    }
}
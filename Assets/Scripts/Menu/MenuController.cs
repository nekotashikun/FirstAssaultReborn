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

        [Header("Input Assignment"), SerializeField]
        private KeyCode _backKey;

        [Header("Scenes"), SerializeField]
        private string _mainMenuScene;
        [SerializeField]
        private string _lobbyScene;
        [SerializeField]
        private string _inGameScene;

        [Header("Shared Canvas"), SerializeField]
        private GameObject _mainCanvas;

        [Header("Menu Prefabs"),SerializeField]
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
            
            _messenger = GameMessenger.Instance;
            _messenger.RegisterSubscriberToMessageTypeOf<MenuMessage>(HandleMessage);

            DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += OnSceneChange;
            EditorSceneManager.activeSceneChanged += OnSceneChange;

            _menuPrefabs = new GameObject[] { _mainMenuPanelPrefab, _optionsPanelPrefab, _pausePanelPrefab };

            SceneManager.LoadScene(_mainMenuScene);
        }

        void Update()
        {
            var currentScene = SceneManager.GetActiveScene();

            if (!Input.GetKeyUp(_backKey) || _mainMenuPanelPrefab.activeSelf)
            {
                return;
            }
            else if (currentScene.name.Equals(_lobbyScene) && Input.GetKeyUp(_backKey))
            {
                ReturnToMainMenu();
            }
            else if (Input.GetKeyUp(_backKey)) 
            {
                if (!currentScene.name.Equals(_mainMenuScene))
                {
                    _messenger.SendMessageOfType(new MenuMessage(MenuType.Pause));
                }
                else
                {
                    _messenger.SendMessageOfType(new MenuMessage(MenuType.Main));
                }
            }
        }

        private void HandleMessage(MenuMessage incomingMessage)
        {
            switch (incomingMessage.MenuState)
            {
                case MenuType.Main:
                    {
                        ResetPrefabs(_menuPrefabs[0]);
                        break;
                    }
                case MenuType.Connect:
                    {
                        ResetPrefabs();
                        SceneManager.LoadScene(_lobbyScene);
                        break;
                    }
                case MenuType.Options:
                    {
                        ResetPrefabs(_menuPrefabs[1]);
                        break;
                    }
                case MenuType.Pause:
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
                {
                    g.SetActive(false);
                }
                else
                {
                    g.SetActive(true);
                }
            }
        }

        private void ResetPrefabs()
        {
            foreach (GameObject g in _menuPrefabs)
            { 
                g.SetActive(false);
            }
        }

        private void OnSceneChange(Scene current, Scene next)
        {
            if (next.name.Equals(_mainMenuScene))
            {
                EnterMainMenu();
            }
        }

        public void EnterOptionsMenu() => _messenger.SendMessageOfType(new MenuMessage(MenuType.Options));

        public void EnterMainMenu() => _messenger.SendMessageOfType(new MenuMessage(MenuType.Main));

        public void Exit() => Application.Quit();

        public string GetSceneName() => _sceneName;

        public string GetMainMenuScene() => _mainMenuScene;
        
        public void ReturnToMainMenu() => SceneManager.LoadScene(_mainMenuScene);
        
        public void OnDestroy() => _messenger.UnRegisterAllMessagesForObject(this);
        
    }
}
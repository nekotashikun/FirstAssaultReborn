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

        private List<GameObject> _menuPrefabs;
      
        void Start()
        {
            if (_instance == null)
                _instance = this;
            else
                Destroy(gameObject);

            _messenger = GameMessenger.Instance;
            _messenger.RegisterSubscriberToMessageTypeOf<MenuMessage>(HandleMessage);

            _sceneName = SceneManager.GetActiveScene().name;
            if (_sceneName == "MainMenu")
                DontDestroyOnLoad(gameObject);

            SceneManager.activeSceneChanged += ChangedScene;
            EditorSceneManager.activeSceneChanged += ChangedScene;

            _menuPrefabs = new List<GameObject>();
            if (_mainMenuPanelPrefab != null) _menuPrefabs.Add(_mainMenuPanelPrefab);
            if (_pausePanelPrefab != null) _menuPrefabs.Add(_pausePanelPrefab);
            if (_optionsPanelPrefab != null) _menuPrefabs.Add(_optionsPanelPrefab);
        }

        void Update()
        {
            if (_sceneName != "MainMenu" && Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log(_sceneName.ToString());
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
                        DeactivatePrefabs();
                        _mainMenuPanelPrefab.SetActive(true);
                        break;
                    }
                case MenuType.CONNECT:
                    {
                        DeactivatePrefabs();
                        SceneManager.LoadScene("MultiplayerMenu");
                        break;
                    }
                case MenuType.OPTIONS:
                    {
                        DeactivatePrefabs();
                        _optionsPanelPrefab.SetActive(true);
                        break;
                    }
                case MenuType.PAUSE:
                    {
                        DeactivatePrefabs();
                        _pausePanelPrefab.SetActive(true);
                        break;
                    }
                default:
                    {
                        DeactivatePrefabs();
                        break;
                    }
            }
        }

        private void DeactivatePrefabs()
        {
            foreach (GameObject prefab in _menuPrefabs)
            {
                if (prefab.activeSelf) prefab.SetActive(false);
            }
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
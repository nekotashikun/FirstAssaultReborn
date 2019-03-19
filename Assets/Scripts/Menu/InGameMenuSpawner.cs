using UnityEngine;

namespace Menu
{
    public class InGameMenuSpawner : MonoBehaviour
    {
        [Header("Required Objects"), SerializeField]
        private GameObject _menuGameObject;
        [SerializeField]
        private GameObject _menuControllerPrefab;
        [SerializeField]
        private GameObject _eventSystemPrefab;

    void Start()
        {
            if (_menuGameObject.GetComponents<InGameMenuController>().Length == 0)
            {
                Instantiate(_menuControllerPrefab);
                Instantiate(_eventSystemPrefab);
            }
        }
    }
}

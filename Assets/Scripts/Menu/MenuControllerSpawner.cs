using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Menu
{
    public class MenuControllerSpawner : MonoBehaviour
    {
        [Header("Required Objects"), SerializeField]
        private GameObject _menuGameObject;
        [SerializeField]
        private GameObject _menuControllerPrefab;
        [SerializeField]
        private GameObject _eventSystemPrefab;

    // Start is called before the first frame update
    void Start()
        {
            if (_menuGameObject.GetComponents<MenuController>().Length == 0)
            {
                Instantiate(_menuControllerPrefab);
                Instantiate(_eventSystemPrefab);
            }
                
        }

    }
}

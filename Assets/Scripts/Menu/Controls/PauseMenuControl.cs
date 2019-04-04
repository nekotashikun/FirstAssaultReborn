using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu.Controls
{
    public class PauseMenuControl : MonoBehaviour
    {
        [Header("Controller"), SerializeField]
        private MenuController _menuController;
        
        public void HidePauseMenu()
        {
            _menuController.SendMenuMessage(MenuType.GAME);
        }


    }
}

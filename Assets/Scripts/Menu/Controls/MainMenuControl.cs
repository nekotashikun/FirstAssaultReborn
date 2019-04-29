using Menu;
using UnityEngine;


namespace Menu.Controls
{
    public class MainMenuControl : MonoBehaviour
    {

        [Header("Controller"), SerializeField]
        private MenuController _menuController;

        public void EnterConnectionMenu()
        {
            _menuController.SendMenuMessage(MenuType.CONNECT);
        }

    }
}
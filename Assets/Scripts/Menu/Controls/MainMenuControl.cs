using Menu;
using UnityEngine;

public class MainMenuControl : MonoBehaviour
{

    [Header("Controller"), SerializeField]
    private MenuController _menuController;

    public void EnterConnectionMenu()
    {
        _menuController.SendMenuMessage(MenuType.CONNECT);
    }

}

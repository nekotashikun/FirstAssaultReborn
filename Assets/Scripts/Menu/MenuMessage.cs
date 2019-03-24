using System;

namespace Menu
{
    public struct MenuMessage
    {
        private readonly MenuType _menuState;

        public MenuMessage(MenuType desiredState)
        {
            _menuState = desiredState;
        }

        public MenuType MenuState { get { return _menuState; } }
    }
}

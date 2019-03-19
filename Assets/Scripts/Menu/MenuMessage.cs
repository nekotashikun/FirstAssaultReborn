using System;

namespace Menu
{
    public struct MenuMessage
    {
        public MenuType MenuState { get; set; }
    }

    public enum MenuType
    {
        MAIN,
        OPTIONS,
        PAUSE,
        HUD,
        EXIT
    }
}

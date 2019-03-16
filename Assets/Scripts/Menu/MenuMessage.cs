using System;

namespace Menu
{
    public struct MenuMessage
    {
        public MenuType MenuState { get; set; }
    }

    public enum MenuType
    {
        NONE,
        MAIN,
        OPTIONS,
        PAUSE,
        HUD,
        EXIT
    }
}

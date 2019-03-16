using System;

namespace Menu
{
    public struct MenuMessage
    {
        public MenuType MessageNumber { get; set; }
    }

    public enum MenuType
    {
        NONE,
        MAIN,
        OPTIONS,
        PAUSE
    }
}

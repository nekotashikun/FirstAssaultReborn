using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scripts.Player.Balin.Input;
using UnityEngine;

namespace Scripts.Player.Balin.Character
{
    public interface IControllable
    {
        void HandleInput(InputState tickState);
    } 
}

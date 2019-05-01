using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Scripts.Network
{
    public interface IControllable
    {
        void SetTickInput(Vector2 movementInput, bool crouchInput, bool sprintInput, bool walkInput, bool jumpInput, Vector2 lookInput);
    } 
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Player.Balin.Character
{
    public interface IMovementState
    {
        MovementStateEnum MovementState { get; }
    }
}

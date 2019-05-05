using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Scripts.Player.Balin.Input;

namespace Scripts.Player.Balin.Character
{
    public class StandingState : CharacterState
    {
        public override void OnFixedUpdate(TestMovement character, InputState inputState)
        {
            if (inputState.IsMoving)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Walking);
            }

            base.OnFixedUpdate(character, inputState);
        }
    }
}

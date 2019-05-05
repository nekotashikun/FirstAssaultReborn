using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Scripts.Player.Balin.Input;

namespace Scripts.Player.Balin.Character
{
    public class StandingState : CharacterBaseState
    {
        public override void OnEnterState(TestMovement character)
        {
            return;
        }

        public override void OnFixedUpdate(TestMovement character, InputState inputState)
        {
            base.OnFixedUpdate(character, inputState);

            if (inputState.Crouch)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Crouching);
                return;
            }

            if (!inputState.Walk && inputState.IsMoving)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Running);
                return;
            }

            if(inputState.Walk && inputState.IsMoving)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Walking);
                return;
            }

            if(inputState.Jump)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Jumping);
                return;
            }

            character.speed = Vector3.zero;

            Vector3 movementDelta = Physics.gravity;
            character.characterController.Move(movementDelta * Time.fixedDeltaTime);
        }

        public override void OnExitState(TestMovement character)
        {
            character.speed.y = 0;
        }
    }
}

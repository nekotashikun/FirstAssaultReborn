using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Scripts.Player.Balin.Input;

namespace Scripts.Player.Balin.Character
{
    class FallingState : CharacterBaseState
    {
        public override void OnEnterState(TestMovement character)
        {
            character.verticalSpeed = 0;
        }

        public override void OnFixedUpdate(TestMovement character, InputState inputState)
        {
            base.OnFixedUpdate(character, inputState);

            if (character.characterController.isGrounded)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Standing);
                return;
            }

            character.speed.y += Physics.gravity.y * Time.fixedDeltaTime;

            float movementHorizontalAxis = (inputState.MoveRight ? 1 : 0) + (inputState.MoveLeft ? -1 : 0);
            float movementVerticalAxis = (inputState.MoveForward ? 1 : 0) + (inputState.MoveBackward ? -1 : 0);

            Vector2 movementDirection = new Vector2(movementHorizontalAxis, movementVerticalAxis);
            if (movementDirection.sqrMagnitude > 1)
            {
                movementDirection = movementDirection.normalized;
            }

            character.characterController.Move(character.speed * Time.fixedDeltaTime); 
        }

        public override void OnExitState(TestMovement character)
        {
            character.verticalSpeed = 0;
        }
    }
}

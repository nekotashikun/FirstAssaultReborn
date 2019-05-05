using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Scripts.Player.Balin.Input;

namespace Scripts.Player.Balin.Character
{
    class WalkingState : CharacterState
    {
        public override void OnFixedUpdate(TestMovement character, InputState inputState)
        {
            if(!character.characterController.isGrounded)
            {
                //TODO: transition  to "Falling" state
            }

            if(!inputState.IsMoving && character.characterController.isGrounded)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Standing);
            }

            float movementHorizontalAxis = (inputState.MoveRight ? 1 : 0) + (inputState.MoveLeft ? -1 : 0);
            float movementVerticalAxis = (inputState.MoveForward ? 1 : 0) + (inputState.MoveBackward ? -1 : 0);

            Vector2 movementDirection = new Vector2(movementHorizontalAxis, movementVerticalAxis);
            if(movementDirection.sqrMagnitude > 1)
            {
                movementDirection = movementDirection.normalized;
            }

            Vector3 movementDelta = 
            character.transform.forward * character.moveSpeed * movementDirection.y +
            character.transform.right * character.moveSpeed * movementDirection.x +
            Physics.gravity
            ;

            character.characterController.Move(movementDelta * Time.fixedDeltaTime);

            base.OnFixedUpdate(character, inputState);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Scripts.Player.Balin.Input;

namespace Scripts.Player.Balin.Character
{
    class WalkingState : CharacterBaseState
    {
        public override void OnEnterState(TestMovement character)
        {
            return;
        }

        public override void OnFixedUpdate(TestMovement character, InputState inputState)
        {
            base.OnFixedUpdate(character, inputState);

            if (!inputState.IsMoving && character.characterController.isGrounded)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Standing);
                return;
            }

            if(inputState.Crouch && inputState.IsMoving)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.CrouchWalking);
                return;
            }

            if (!inputState.Walk && inputState.IsMoving)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Running);
                return;
            }

            if (inputState.Jump)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Jumping);
                return;
            }

            float movementHorizontalAxis = (inputState.MoveRight ? 1 : 0) + (inputState.MoveLeft ? -1 : 0);
            float movementVerticalAxis = (inputState.MoveForward ? 1 : 0) + (inputState.MoveBackward ? -1 : 0);

            Vector2 movementDirection = new Vector2(movementHorizontalAxis, movementVerticalAxis);
            if(movementDirection.sqrMagnitude > 1)
            {
                movementDirection = movementDirection.normalized;
            }

            if(movementDirection == Vector2.zero)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Standing);
            }

            character.speed = 
            character.transform.forward * character.moveSpeed * character.walkSpeedModifier * movementDirection.y +
            character.transform.right * character.moveSpeed * character.walkSpeedModifier * movementDirection.x
            ;

            if (Physics.Raycast(character.transform.position, -character.transform.up, TestMovement.standingSize * 0.75f))
            {
                character.speed.y = Physics.gravity.y;
            }

            character.characterController.Move(character.speed * Time.fixedDeltaTime);  
        }

        public override void OnExitState(TestMovement character)
        {
            character.speed.y = 0;
        }
    }
}

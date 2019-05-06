using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Scripts.Player.Balin.Input;

namespace Scripts.Player.Balin.Character
{
    class CrouchingState : CharacterBaseState
    {
        public override void OnEnterState(TestMovement character)
        {
            character.characterController.height = TestMovement.crouchingSize;
            character.characterController.Move(new Vector3(0, -(TestMovement.standingSize - TestMovement.crouchingSize)/2, 0));
        }

        public override void OnFixedUpdate(TestMovement character, InputState inputState)
        {
            base.OnFixedUpdate(character, inputState);

            if (
            !inputState.Crouch &&
            !Physics.SphereCast(new Ray(character.transform.position, Vector3.up), character.characterController.radius, 2f)
            )
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Standing);
                return;
            }

            if (inputState.IsMoving)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.CrouchWalking);
                return;
            }

            character.speed = Vector3.zero;

            Vector3 movementDelta = Physics.gravity;
            character.characterController.Move(movementDelta * Time.fixedDeltaTime);
        }

        public override void OnExitState(TestMovement character)
        {
            character.characterController.height = TestMovement.standingSize;
            character.characterController.Move(new Vector3(0, +(TestMovement.standingSize - TestMovement.crouchingSize) / 2, 0));
        }
    }
}

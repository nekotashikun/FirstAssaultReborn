using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Scripts.Player.Balin.Input;

namespace Scripts.Player.Balin.Character
{
    public abstract class CharacterBaseState
    {
        public abstract void OnEnterState(TestMovement character);

        public virtual void OnFixedUpdate(TestMovement character, InputState inputState)
        {
            if
            (
            !character.characterController.isGrounded &&
            character.characterState != CharacterStateMachine.Falling &&
            character.characterState != CharacterStateMachine.Jumping
            )
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Falling);
                return;
            }

            character.transform.Rotate(Vector3.up, character.rotateSpeed * inputState.MouseHorizontal * Time.fixedDeltaTime, Space.Self);

            character.lookAngle += character.rotateSpeed * inputState.MouseVertical * Time.fixedDeltaTime;
            character.lookAngle = Mathf.Clamp(character.lookAngle, TestMovement.lookVerticalUpperLimit, TestMovement.lookVerticalLowerLimit);
        }

        public abstract void OnExitState(TestMovement character);
    }
}

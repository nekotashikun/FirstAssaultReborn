using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Scripts.Player.Balin.Input;

namespace Scripts.Player.Balin.Character
{
    class JumpingState : CharacterBaseState
    {
        public override void OnEnterState(TestMovement character)
        {
            character.speed.y = character.jumpSpeed;

            character.characterController.Move(new Vector3(0, character.verticalSpeed * Time.fixedDeltaTime, 0));
        }

        public override void OnFixedUpdate(TestMovement character, InputState inputState)
        {
            base.OnFixedUpdate(character, inputState);

            if(character.speed.y <= 0)
            {
                CharacterStateMachine.ChangeState(character, inputState, CharacterStateMachine.Falling);
                return;
            }

            character.speed.y += Physics.gravity.y * Time.fixedDeltaTime;

            CollisionFlags flags = character.characterController.Move(character.speed * Time.fixedDeltaTime);

            if((flags & CollisionFlags.Above) != 0)
            {
                character.speed.y = 0;
            }
        }

        public override void OnExitState(TestMovement character)
        {
            character.speed.y = 0;
        }
    }
}

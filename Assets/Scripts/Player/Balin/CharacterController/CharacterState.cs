using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using Scripts.Player.Balin.Input;

namespace Scripts.Player.Balin.Character
{
    public abstract class CharacterState
    {
        public virtual void OnFixedUpdate(TestMovement character, InputState inputState)
        {
            character.transform.Rotate(Vector3.up, character.rotateSpeed * inputState.MouseHorizontal * Time.fixedDeltaTime, Space.Self);

            character.lookAngle += character.rotateSpeed * inputState.MouseVertical * Time.fixedDeltaTime;
            character.lookAngle = Mathf.Clamp(character.lookAngle, TestMovement.lookVerticalUpperLimit, TestMovement.lookVerticalLowerLimit);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Scripts.Player.Balin.Input;

namespace Scripts.Player.Balin.Character
{
    static class CharacterStateMachine
    {
        private static StandingState _standing = new StandingState();
        private static WalkingState _walking = new WalkingState();

        public static StandingState Standing
        {
            get
            {
                return _standing;
            }
        }

        public static WalkingState Walking
        {
            get
            {
                return _walking;
            }
        }

        public static void ChangeState(TestMovement character, InputState inputState, CharacterState state)
        {
            character.characterState = state;
            state.OnFixedUpdate(character, inputState);
        }
    }
}

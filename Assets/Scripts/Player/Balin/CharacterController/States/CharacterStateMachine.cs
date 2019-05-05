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
        public static StandingState Standing { get; } = new StandingState();
        public static CrouchingState Crouching { get; } = new CrouchingState();

        public static RunningState Running { get; } = new RunningState();
        public static CrouchWalkingState CrouchWalking { get; } = new CrouchWalkingState();
        public static WalkingState Walking { get; } = new WalkingState();
        public static SprintingState Sprinting { get; } = new SprintingState();

        public static FallingState Falling { get; } = new FallingState();
        public static JumpingState Jumping { get; } = new JumpingState();

        public static void ChangeState(TestMovement character, InputState inputState, CharacterBaseState state)
        {
            character.characterState.OnExitState(character);

            character.characterState = state;
            state.OnEnterState(character);

            state.OnFixedUpdate(character, inputState);
        }
    }
}

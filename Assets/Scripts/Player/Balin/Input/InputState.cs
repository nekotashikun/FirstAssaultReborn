using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scripts.Player.Balin.Input
{
    public struct InputState
    {
        public uint Tick { get; private set; }

        public float MouseHorizontal { get; private set; }
        public float MouseVertical { get; private set; }

        public bool MoveRight { get; private set; }
        public bool MoveLeft { get; private set; }
        public bool MoveForward { get; private set; }
        public bool MoveBackward { get; private set; }

        public bool Crouch { get; private set; }
        public bool Sprint { get; private set; }
        public bool Walk { get; private set; }

        public bool Jump { get; private set; }

        public bool IsMoving
        {
            get
            {
                return
                (MoveForward && !MoveBackward) ||
                (!MoveForward && MoveBackward) ||
                (MoveRight && !MoveLeft) ||
                (!MoveRight && MoveLeft)
                ;
            }
        }

        public InputState
        (
            uint tick,
            float mouseHorizontal,
            float mouseVertical,
            bool moveRight,
            bool moveLeft,
            bool moveForward,
            bool moveBackward,
            bool crouch,
            bool sprint,
            bool walk,
            bool jump
        )
        {
            Tick = tick;

            MouseHorizontal = mouseHorizontal;
            MouseVertical = mouseVertical;

            MoveRight = moveRight;
            MoveLeft = moveLeft;
            MoveForward = moveForward;
            MoveBackward = moveBackward;

            Crouch = crouch;
            Sprint = sprint;
            Walk = walk;

            Jump = jump;
        }
    }
}

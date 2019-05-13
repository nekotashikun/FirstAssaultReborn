using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Player.Balin.Character;

namespace Scripts.Player.Balin.FirstPerson
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _character;

        [SerializeField]
        private IMovementState _characterMovementState;

        [SerializeField]
        private Animator _movementAnimator;

        // Start is called before the first frame update
        void Start()
        {
            _characterMovementState = _character.GetComponent<IMovementState>();
            _movementAnimator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void LateUpdate()
        {
            switch(_characterMovementState.MovementState)
            {
                case MovementStateEnum.Standing:
                    _movementAnimator.SetBool("Iswalking", false);
                    _movementAnimator.SetBool("Isrunning", false);
                    _movementAnimator.SetBool("isidle", true);
                    break;
                case MovementStateEnum.Crouching:
                    _movementAnimator.SetBool("Iswalking", false);
                    _movementAnimator.SetBool("Isrunning", false);
                    _movementAnimator.SetBool("isidle", true);
                    break;
                case MovementStateEnum.Walking:
                    _movementAnimator.SetBool("Iswalking", true);
                    _movementAnimator.SetBool("Isrunning", false);
                    _movementAnimator.SetBool("isidle", false);
                    break;
                case MovementStateEnum.Running:
                    _movementAnimator.SetBool("Iswalking", true);
                    _movementAnimator.SetBool("Isrunning", false);
                    _movementAnimator.SetBool("isidle", false);
                    break;
                case MovementStateEnum.CrouchWalking:
                    _movementAnimator.SetBool("Iswalking", true);
                    _movementAnimator.SetBool("Isrunning", false);
                    _movementAnimator.SetBool("isidle", false);
                    break;
                case MovementStateEnum.Sprinting:
                    _movementAnimator.SetBool("Iswalking", false);
                    _movementAnimator.SetBool("Isrunning", true);
                    _movementAnimator.SetBool("isidle", false);
                    break;
            }
        }
    }
}

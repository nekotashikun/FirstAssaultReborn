using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Scripts.Network;

[RequireComponent(typeof(CharacterController))]
public class TestMovement : MonoBehaviour, IControllable
{
    public const float standingSize = 1.8f;
    public const float crouchingSize = 0.8f;

    public const float lookVerticalUpperLimit = -89;
    public const float lookVerticalLowerLimit = 89;

    [SerializeField]
    private CharacterController characterController;

    public float moveSpeed = 5;
    public float sprintSpeedModifier = 2f;
    public float walkSpeedModifier = 0.75f;
    public float crouchSpeedModifier = 0.5f;
    public float rotateSpeed = 90;
    public float jumpSpeed = 5;
    public float airControlModifier = 0.2f;
    public Vector2 moveTime = new Vector2();
    public AnimationCurve inertiaCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.1f, 0.9f, Mathf.Tan(Mathf.PI/4), Mathf.Tan(Mathf.PI / 4)), new Keyframe(1f, 1f));

    public float currentSpeed = 0;

    public float lookAngle = 0;
    public float verticalSpeed = 0;
    public bool isPreviouslyGrounded = true;
    public bool isCrouching = false;
    public Vector2 jumpStartSpeed = new Vector2();

    private Vector2 currentMovementInput = new Vector2();
    private bool currentCrouchInput = false;
    private bool currentSprintInput = false;
    private bool currentWalkInput = false;
    private bool currentJumpInput = false;
    private Vector2 currentLookInput = new Vector2();

    private int terrainLayerMask;

    // Start is called before the first frame update
    void Start()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        terrainLayerMask = LayerMask.GetMask("Terrain");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveCharacter(currentMovementInput, currentCrouchInput, currentSprintInput, currentWalkInput, currentJumpInput, currentLookInput);
    }

    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + characterController.velocity, Color.black);
    }

    public static Vector2 GetPositionDeltaComponents(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }

    public void MoveCharacter(Vector2 movementInput, bool crouchInput, bool sprintInput, bool walkInput, bool jumpInput, Vector2 lookInput)
    {
        float updateSpeed = moveSpeed * (sprintInput? sprintSpeedModifier:1) * Time.fixedDeltaTime;
        Vector2 verticalLegs = GetPositionDeltaComponents(transform.eulerAngles.y);
        Vector2 horizontalLegs = GetPositionDeltaComponents(transform.eulerAngles.y + 90);

        bool forceCrouch = false;

        bool crouchCheck = Physics.SphereCast(new Ray(transform.position, Vector3.up), characterController.radius, 2f, terrainLayerMask);

        if (isCrouching && crouchCheck)
        {
            forceCrouch = true;
        }

        if(crouchInput || forceCrouch)
        {
            updateSpeed = moveSpeed * crouchSpeedModifier * Time.fixedDeltaTime;
            characterController.height = crouchingSize;
            isCrouching = true;
        }
        else if(sprintInput)
        {
            updateSpeed = moveSpeed * sprintSpeedModifier * Time.fixedDeltaTime;
            characterController.height = standingSize;
            isCrouching = false;
        }
        else if (walkInput)
        {
            updateSpeed = moveSpeed * walkSpeedModifier * Time.fixedDeltaTime;
            characterController.height = standingSize;
            isCrouching = false;
        }
        else
        {
            updateSpeed = moveSpeed * Time.fixedDeltaTime;
            characterController.height = standingSize;
            isCrouching = false;
        }

        Vector2 updateDirection = movementInput;
        if (updateDirection.sqrMagnitude > 1)
        {
            updateDirection = updateDirection.normalized;
        }

        if (updateDirection.x != 0)
        {
            moveTime.x = Mathf.MoveTowards(moveTime.x, 1, Time.fixedDeltaTime);
        }
        else
        {
            moveTime.x = Mathf.MoveTowards(moveTime.x, 0, Time.fixedDeltaTime);
        }

        if (updateDirection.y != 0)
        {
            moveTime.y = Mathf.MoveTowards(moveTime.y, 1, Time.fixedDeltaTime);
        }
        else
        {
            moveTime.y = Mathf.MoveTowards(moveTime.y, 0, Time.fixedDeltaTime);
        }

        if (characterController.isGrounded)
        {
            jumpStartSpeed.x = characterController.velocity.x;
            jumpStartSpeed.y = characterController.velocity.z;
            if (jumpInput)
            {
                verticalSpeed = jumpSpeed;
                isPreviouslyGrounded = false;
            }
            else
            {
                verticalSpeed = Physics.gravity.y;
                isPreviouslyGrounded = true;
            }
        }
        else
        {
            if (isPreviouslyGrounded)
            {
                verticalSpeed = 0;
                isPreviouslyGrounded = false;
            }
            verticalSpeed += Physics.gravity.y * Time.fixedDeltaTime;
        }

        Vector2 movementDelta = new Vector2(
            updateSpeed * updateDirection.x * horizontalLegs.y * inertiaCurve.Evaluate(moveTime.x) +
            updateSpeed * updateDirection.y * verticalLegs.y * inertiaCurve.Evaluate(moveTime.y),
            updateSpeed * updateDirection.x * horizontalLegs.x * inertiaCurve.Evaluate(moveTime.x) +
            updateSpeed * updateDirection.y * verticalLegs.x * inertiaCurve.Evaluate(moveTime.y)
        );

        Vector3 positionDelta = new Vector3(movementDelta.x, verticalSpeed * Time.fixedDeltaTime, movementDelta.y);

        if(!characterController.isGrounded)
        {
            positionDelta.x = jumpStartSpeed.x * Time.fixedDeltaTime + movementDelta.x * airControlModifier;
            positionDelta.z = jumpStartSpeed.y * Time.fixedDeltaTime + movementDelta.y * airControlModifier;
        }

        characterController.Move(positionDelta);

        currentSpeed = characterController.velocity.magnitude;

        transform.Rotate(Vector3.up, rotateSpeed * lookInput.x * Time.fixedDeltaTime, Space.Self);

        lookAngle += rotateSpeed * lookInput.y * Time.fixedDeltaTime;
        lookAngle = Mathf.Clamp(lookAngle, lookVerticalUpperLimit, lookVerticalLowerLimit);
    }

    public void SetTickInput(Vector2 movementInput, bool crouchInput, bool sprintInput, bool walkInput, bool jumpInput, Vector2 lookInput)
    {
        currentMovementInput = movementInput;
        currentCrouchInput = crouchInput;
        currentSprintInput = sprintInput;
        currentWalkInput = walkInput;
        currentJumpInput = jumpInput;
        currentLookInput = lookInput;
    }
}

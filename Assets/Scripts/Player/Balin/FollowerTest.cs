using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Network;

[RequireComponent(typeof(IControllable))]
public class FollowerTest : MonoBehaviour
{
    public LocalInput localInput;
    public TestMovement testMovement;
    public Vector2 moveTime = new Vector2();

    public uint updateNumber = 0;
    public int currentIndex = 0;
    public int latency = 0;

    public IControllable controllable;

    // Start is called before the first frame update
    void Start()
    {
        controllable = GetComponent<IControllable>();
    }

    public void OnGUI()
    {
        GUI.Label(new Rect(new Vector2(50, 50), new Vector2(200, 50)), Vector3.Distance(testMovement.transform.position, transform.position).ToString());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FrameInput horizontalInput = null;
        FrameInput verticalInput = null;
        FrameInput mouseHorizontalInput = null;
        FrameInput mouseVerticalinput = null;
        FrameInput crouchFrameInput = null;
        FrameInput sprintFrameInput = null;
        FrameInput walkFrameInput = null;
        FrameInput jumpFrameInput = null;

        Vector2 movementInput = new Vector2();
        Vector2 lookInput = new Vector2();
        bool crouchInput = false;
        bool sprintInput = false;
        bool walkInput = false;
        bool jumpInput = false;

        if (localInput.inputHistory.Count > currentIndex)
        {
            if (localInput.inputHistory[currentIndex][0].updateNumber + latency < updateNumber)
            {
                horizontalInput = localInput.inputHistory[currentIndex].Find(FrameInput => FrameInput.Axis == InputUtils.Axes.Horizontal);
                verticalInput = localInput.inputHistory[currentIndex].Find(FrameInput => FrameInput.Axis == InputUtils.Axes.Vertical);

                mouseHorizontalInput = localInput.inputHistory[currentIndex].Find(FrameInput => FrameInput.Axis == InputUtils.Axes.MouseX);
                mouseVerticalinput = localInput.inputHistory[currentIndex].Find(FrameInput => FrameInput.Axis == InputUtils.Axes.MouseY);

                crouchFrameInput = localInput.inputHistory[currentIndex].Find(FrameInput => FrameInput.Axis == InputUtils.Axes.Crouch);
                sprintFrameInput = localInput.inputHistory[currentIndex].Find(FrameInput => FrameInput.Axis == InputUtils.Axes.Sprint);
                walkFrameInput = localInput.inputHistory[currentIndex].Find(FrameInput => FrameInput.Axis == InputUtils.Axes.Walk);
                jumpFrameInput = localInput.inputHistory[currentIndex].Find(FrameInput => FrameInput.Axis == InputUtils.Axes.Jump);

                currentIndex++;
            }
        }

        if (horizontalInput != null)
        {
            movementInput.x = horizontalInput.Value;
        }

        if (verticalInput != null)
        {
            movementInput.y = verticalInput.Value;
        }

        if (mouseHorizontalInput != null)
        {
            lookInput.x = mouseHorizontalInput.Value;
        }

        if (mouseVerticalinput != null)
        {
            lookInput.y = mouseVerticalinput.Value;
        }

        if (crouchFrameInput != null)
        {
            crouchInput = crouchFrameInput.IsPressed;
        }

        if (sprintFrameInput != null)
        {
            sprintInput = sprintFrameInput.IsPressed;
        }

        if (walkFrameInput != null)
        {
            walkInput = walkFrameInput.IsPressed;
        }

        if (jumpFrameInput != null)
        {
            jumpInput = jumpFrameInput.IsPressed;
        }

        controllable.SetTickInput(movementInput, crouchInput, sprintInput, walkInput, jumpInput, lookInput);

        updateNumber++;
    }
}

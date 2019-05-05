using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Player.Balin.Input;
using Scripts.Player.Balin.Character;

[RequireComponent(typeof(IControllable))]
public class LocalInput : MonoBehaviour
{
    public List<InputState> inputHistory = new List<InputState>();
    public uint currentTickNumber = 0;

    public IControllable controllable;

    // Start is called before the first frame update
    void Start()
    {
        controllable = GetComponent<IControllable>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputState currentTickInput = new InputState(
            currentTickNumber,
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y"),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.LeftControl),
            Input.GetKey(KeyCode.LeftShift),
            Input.GetKey(KeyCode.V),
            Input.GetKey(KeyCode.Space)
        );

        inputHistory.Add(currentTickInput);

        currentTickNumber++;

        controllable.HandleInput(currentTickInput);
    }

    void OnDestroy()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputValue
{
    public string Name { get; set; }

    public float Axis
    {
        get
        {
            return Input.GetAxis(Name);
        }
    }

    public float RawAxis
    {
        get
        {
            return Input.GetAxisRaw(Name);
        }
    }

    public bool GetButton
    {
        get
        {
            return Input.GetButton(Name);
        }
    }

    public bool GetButtonDown
    {
        get
        {
            return Input.GetButtonDown(Name);
        }
    }

    public InputValue(string name)
    {
        Name = name;
    }
}
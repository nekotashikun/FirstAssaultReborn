using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Player.Balin.Character;

public class LocalCamera : MonoBehaviour
{

    public TestMovement localPlayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x = localPlayer.lookAngle * -1;
        transform.localEulerAngles = newRotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Player.Balin.Character;

public class LocalCamera : MonoBehaviour
{

    public TestMovement localPlayer;
    public float maxVerticalSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = localPlayer.transform.position.x;
        newPosition.z = localPlayer.transform.position.z;
        newPosition.y = Mathf.MoveTowards(newPosition.y, localPlayer.transform.position.y + 0.525f, maxVerticalSpeed * Time.deltaTime);
        transform.position = newPosition;

        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x = localPlayer.lookAngle * -1;
        newRotation.y = localPlayer.transform.eulerAngles.y;
        transform.localEulerAngles = newRotation;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scripts.Player.Balin;
using Scripts.Player.Balin.Character;

public class LocalCamera : MonoBehaviour
{

    public ILocalCharacterView localPlayer;
    public GameObject playerGameObject;
    public float maxVerticalSpeed;

    // Start is called before the first frame update
    void Start()
    {
        localPlayer = playerGameObject.GetComponent<ILocalCharacterView>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position;
        newPosition.x = localPlayer.Position.x;
        newPosition.z = localPlayer.Position.z;
        newPosition.y = Mathf.MoveTowards(newPosition.y, localPlayer.Position.y + 0.525f, maxVerticalSpeed * Time.deltaTime);
        transform.position = newPosition;

        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x = localPlayer.LookRotation.x * -1;
        newRotation.y = localPlayer.LookRotation.y;
        transform.localEulerAngles = newRotation;
    }
}

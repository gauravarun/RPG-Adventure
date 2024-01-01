using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform target;

 
    void Update()
    {
        if (!target)
        {
            return;
        }

       float currentRotationAngle = transform.eulerAngles.y;
       float wantedRotationAngle = target.eulerAngles.y;

       currentRotationAngle = Mathf.LerpAngle
       (
        currentRotationAngle,
        wantedRotationAngle,
        0.5f
       );

       transform.position = new Vector3 
       (
        target.position.x,
        5.0f,
        target.position.z
       );
       Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

       Vector3 rotatedPostion = currentRotation * Vector3.forward;

       transform.position -= rotatedPostion * 10;
       transform.LookAt(target);


    }
}

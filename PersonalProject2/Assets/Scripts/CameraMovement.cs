using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    public Transform target;

    public float soomthSpeed = 0.125f;

    public Vector3 offset;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, soomthSpeed);
        transform.position = smoothedPosition;

        //ѕрикольно заваливающийс€ угол камеры при передвижении
        //ћожно использовать дл€ большей динамики
        //transform.LookAt(target);
    }
}

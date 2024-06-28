using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] private Transform _target;
    private float _smoothSpeed = 0.125f;
    private Vector3 _offset = new Vector3(0, 2, -15);

    void FixedUpdate()
    {
        Vector3 desiredPosition = _target.position + _offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
        transform.position = smoothedPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    private float length;
    private float startPos;
    private Transform cameraTransform;
    [SerializeField] private float parallaxEffectMultiplier;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void LateUpdate()
    {
        /*Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
        lastCameraPosition = cameraTransform.position;

        if(Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (cameraTransform.position.x - transform.position.x);
            transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
        }*/
        float temp = cameraTransform.position.x * (1 - parallaxEffectMultiplier);
        float distance = (cameraTransform.position.x * parallaxEffectMultiplier);
        transform.position = new Vector3(startPos + distance, transform.position.y);

        if(temp > startPos + length)
        {

        }
    }
}


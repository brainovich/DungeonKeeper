using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwstScript2 : MonoBehaviour
{
    public LayerMask maskToAvoid;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D temp = Physics2D.Raycast(transform.position, -transform.up, 2, ~maskToAvoid);
        Debug.DrawRay(transform.position, -transform.up*2, Color.red);
        if (temp)
        {
            Debug.DrawRay(temp.point, temp.normal*6, Color.blue);
        }
    }


}

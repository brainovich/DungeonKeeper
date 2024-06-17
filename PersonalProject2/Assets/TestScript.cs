using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject circle2;
    private Rigidbody2D body;
    // Start is called before the first frame update
    void Start()
    {
        body = circle2.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            body.AddForce((circle2.transform.position - transform.position) * 20);
         }
    }

    private void FixedUpdate()
    {
        
    }
}

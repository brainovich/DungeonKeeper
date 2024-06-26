using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool canDamage;
    public int damage = 20;
    public float force = 50;

    public bool canMove;
    private bool moveToA;
    public Transform pointA;
    public Transform pointB;
    public float speed = 5;



    private void Update()
    {
        if (canMove)
        {
            if (Vector3.Distance(gameObject.transform.position, pointA.position) < 1)
            {
                moveToA = false;
            }
            else if (Vector3.Distance(gameObject.transform.position, pointB.position) < 1)
            {
                moveToA = true;
            }
        }
    }

    private void FixedUpdate()
    {
       if (moveToA && canMove)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime, Space.Self);
        }
        else if (!moveToA && canMove)
        {
            transform.Translate(Vector3.left * speed * Time.deltaTime, Space.Self);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player") && canDamage)
        {
            Debug.Log("colided");
            collision.rigidbody.AddForce(Vector2.left * Random.Range(-10,10), ForceMode2D.Impulse);
            collision.rigidbody.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
            GameManager.instance.playerControlls.TakeDamage(damage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Player") && canMove)
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && canMove)
        {
            collision.gameObject.transform.SetParent(null);
        }
    }

}

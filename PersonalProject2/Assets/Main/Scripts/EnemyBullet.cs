using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private Rigidbody2D body;
    private float bulletForce = 20;
    public Vector3 playerPosition;
    public float hitForce = 10;
    public int damage = 2;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        playerPosition = GameManager.instance.playerControlls.transform.position;
        Vector2 lookDirection = (playerPosition - transform.position).normalized;
        body.AddForce(lookDirection * bulletForce, ForceMode2D.Impulse);
        Destroy(gameObject, 2);
    }

    private void Update()
    {
        
        Debug.DrawRay(transform.position, (playerPosition - transform.position).normalized * 3f, Color.blue);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        if (collision.transform.CompareTag("Player"))
        {
            collision.rigidbody.AddForce(-collision.relativeVelocity.normalized *  hitForce);
            collision.rigidbody.AddForce(Vector2.up * 10);
            GameManager.instance.playerControlls.TakeDamage(damage);
        }
    }
}

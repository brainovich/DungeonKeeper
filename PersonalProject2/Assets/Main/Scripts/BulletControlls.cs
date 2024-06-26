using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControlls : MonoBehaviour
{
    private Rigidbody2D body;
    private float bulletForce = 30;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        float lookDirection = GameManager.instance.playerControlls.transform.localScale.x;
        body.AddForce(new Vector2(lookDirection, 0) * bulletForce, ForceMode2D.Impulse);
        Destroy(gameObject, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}

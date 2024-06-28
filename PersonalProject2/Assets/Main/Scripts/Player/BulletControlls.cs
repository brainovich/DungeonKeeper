using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControlls : MonoBehaviour
{
    private Rigidbody2D _body;
    private float _bulletForce = 30;

    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        float lookDirection = GameManager.instance.playerControlls.transform.localScale.x;
        _body.AddForce(new Vector2(lookDirection, 0) * _bulletForce, ForceMode2D.Impulse);
        Destroy(gameObject, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(1);
        }
        Destroy(gameObject);
    }
}

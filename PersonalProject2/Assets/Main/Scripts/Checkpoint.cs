using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Collider2D _collider;
    private SpriteRenderer _sprite;
    private Animator _animator;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameManager.instance.playerControlls.SetCheckpoint(transform.position);
            _collider.enabled = false;
            GameManager.instance.itemsBehaviour.SaveScore();
            _animator.SetBool("activated", true);
        }
    }
}

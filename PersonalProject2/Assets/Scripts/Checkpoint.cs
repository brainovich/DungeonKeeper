using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Collider2D collider;
    private SpriteRenderer sprite;
    private Animator animator;

    private void Start()
    {
        collider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GameManager.instance.playerControlls.lastCheckpoint = transform.position;
            collider.enabled = false;
            GameManager.instance.itemsBehaviour.SaveScore();
            animator.SetBool("activated", true);
        }
    }
}

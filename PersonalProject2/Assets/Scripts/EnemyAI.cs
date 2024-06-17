using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform pointPatrolA;
    public Transform pointPatrolB;
    public Transform[] activePoint;
    public Transform pointChaseA;
    public Transform pointChaseB;
    public Transform player;
    private Rigidbody2D rb;
    public LayerMask layerToChase;
    public GameObject bullet;
    public GameObject shootingPoint;
    private Animator animator;
    public float speed;
    public float force;
    public int damage = 10;
    private Collider2D enemyCollider;

    public bool patrol = true;
    public bool moveToA = true;
    public bool moveToB = false;
    public bool chasePlayer = false;
    public bool canShoot = true;
    public bool wasDamaged = false;
    public bool isDead = false;
    public bool enemyInArea = false;
    public bool isBlocked = false;
    public bool playerDead;

    public float playerDirectionX;
    public float playerDirectionY;

    int health = 5;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();
    }
    void Update()
    {
        playerDead = GameManager.instance.playerControlls.isDead;

        if (Vector3.Distance(gameObject.transform.position, pointPatrolA.position) < 2 && !isDead)
        {
            moveToA = false;
            moveToB = true;
        }
        else if (Vector3.Distance(gameObject.transform.position, pointPatrolB.position) < 2 && !isDead)
        {
            moveToA = true;
            moveToB = false;
        }

        if(Vector3.Distance(transform.position, player.transform.position) < 2  && !isDead && !playerDead)
        {
            animator.SetBool("attack", true);
        }
        else if(Vector3.Distance(transform.position, player.transform.position) > 2)
        {
            animator.SetBool("attack", false);
        }

        if (enemyInArea && !isDead)
        {
            patrol = playerDead;
            chasePlayer = !playerDead;
        }
        if (Vector3.Distance(gameObject.transform.position, pointChaseA.position) < 2)
        {
            chasePlayer = false;
            patrol = true;
        }
        else if (Vector3.Distance(gameObject.transform.position, pointChaseB.position) < 2)
        {
            chasePlayer = false;
            patrol = true;
        }

        animator.SetBool("walk", rb.velocity.x != 0);
        animator.SetBool("damaged", wasDamaged);

        if (rb.velocity.x >= 0 && !wasDamaged)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (rb.velocity.x < -0.1f && !wasDamaged)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (playerDead)
        {
            Physics2D.IgnoreCollision(enemyCollider, GameManager.instance.playerControlls.playerCollider, true);
        }
        else if (!playerDead)
        {
            Physics2D.IgnoreCollision(enemyCollider, GameManager.instance.playerControlls.playerCollider, false);
        }

        if (health <= 0)
        {
            isDead = true;
            enemyCollider.enabled = false;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, 0);
            StartCoroutine(DeathAnimation());
        }
    }

    private void FixedUpdate()
    {
        if (chasePlayer && !isDead)
        {
            Chase();
        }
        if (patrol && !isDead)
        {
            Patrol();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float speed = collision.relativeVelocity.magnitude;
        speed = Mathf.Clamp(speed, 0, 10);

        if (collision.transform.CompareTag("Player"))
        {
            collision.rigidbody.AddForce(-collision.relativeVelocity.normalized * speed * force);
            collision.rigidbody.AddForce(Vector2.up * 10);
            GameManager.instance.playerControlls.TakeDamage(damage);
        }
        else if (collision.transform.CompareTag("Projectile"))
        {
            StartCoroutine(DamagedAnimation());
            health--;
        }
        else if (collision.transform.CompareTag("Obstacle"))
        {
            health -= 5;
        }
    }
    private void Chase()
    {
        //REWRITE USING MINUS VECTORS
        if(player.transform.position.x < gameObject.transform.position.x)
        {
            rb.AddForce(Vector2.left * speed);
        }
        else if (player.transform.position.x > gameObject.transform.position.x)
        {
            rb.AddForce(Vector2.right * speed);
        }

    }
    private void Patrol()
    {
        if (moveToA)
        {
            rb.AddForce(Vector2.right * speed);
            Debug.DrawRay(transform.position, (pointPatrolA.transform.position - transform.position).normalized * 2f, Color.black);
        }
        else if(moveToB)
        {
            rb.AddForce(Vector2.left * speed);
            Debug.DrawRay(transform.position, (pointPatrolB.transform.position - transform.position).normalized * 2f, Color.black);
        }
    }

    IEnumerator ShootCooldown()
    {
        canShoot = false;
        yield return new WaitForSeconds(1);
        canShoot = true;
    }

    IEnumerator DamagedAnimation()
    {
        wasDamaged = true;
        if (rb.velocity.x >= 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (rb.velocity.x < -0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        yield return new WaitForSeconds(.6f);
        wasDamaged = false;
    }

    IEnumerator DeathAnimation()
    {
        animator.SetBool("dead", true);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}

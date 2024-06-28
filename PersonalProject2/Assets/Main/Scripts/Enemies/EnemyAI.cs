using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamagable
{
    [Header("Basics")]
    private float _speed = 15;
    private float _force = 200;
    private int _damage = 10;
    private int _health = 5;

    [Header("Points")]
    [SerializeField] private Transform[] _pointsToPatrol;
    [SerializeField] private Transform _pointChaseA;
    [SerializeField] private Transform _pointChaseB;
    private int _targetPoint;

    [Header("Links")]
    private Transform _player;
    private Animator _animator;
    private Collider2D _enemyCollider;
    private Rigidbody2D _rb;

    [Header("States")]
    [HideInInspector] public bool enemyInArea = false;
    private bool _chasePlayer = false;
    private bool _wasDamaged = false;
    private bool _isDead = false;
    private bool _playerDead;

    public void DetectEnemy(bool detected) => enemyInArea = detected;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _enemyCollider = GetComponent<Collider2D>();
        _player = GameManager.instance.playerControlls.transform;

    }
    void Update()
    {
        StatesCheck();
        PlayAnimation();

        Physics2D.IgnoreCollision(_enemyCollider, GameManager.instance.playerControlls.PlayerCollider, _playerDead);
    }

    private void FixedUpdate()
    {
        if (!_isDead)
        {
            Patrol();
            Chase();
            FlipModel();
        }
    }

    private void StatesCheck()
    {
        _playerDead = GameManager.instance.playerControlls.IsDead;

        if (enemyInArea && !_isDead)
        {
            _chasePlayer = !_playerDead;
        }
        if (Vector3.Distance(gameObject.transform.position, _pointChaseA.position) < 2)
        {
            _chasePlayer = false;
        }
        else if (Vector3.Distance(gameObject.transform.position, _pointChaseB.position) < 2)
        {
            _chasePlayer = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float speed = collision.relativeVelocity.magnitude;
        speed = Mathf.Clamp(speed, 0, 10);

        if (collision.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            collision.rigidbody.AddForce(-collision.relativeVelocity.normalized * speed * _force);
            collision.rigidbody.AddForce(Vector2.up * 10);
            damagable.TakeDamage(_damage);
        }
    }

    private void Chase()
    {
        if (_chasePlayer)
        {
            MoveTo(_player);
        }
    }

    private void Patrol()
    {
        if (!_chasePlayer)
        {
            if (Vector3.Distance(_pointsToPatrol[_targetPoint].position, gameObject.transform.position) < 2)
            {
                ChoosePoint();
            }
            else
            {
                MoveTo(_pointsToPatrol[_targetPoint]);
            }
        }
    }

    private void MoveTo(Transform target)
    {
        float enemyX = gameObject.transform.position.x;
        float targetX = target.transform.position.x;
        float relativeDirection = (enemyX - enemyX) + (targetX - enemyX);
        float directionX = relativeDirection / Mathf.Abs(relativeDirection);
        Vector2 direction = new Vector2(directionX, 0);

        _rb.AddForce(direction * _speed);
    }

    private void ChoosePoint()
    {
        _targetPoint++;
        if(_targetPoint == _pointsToPatrol.Length)
        {
            _targetPoint = 0;
        }
    }

    private void FlipModel()
    {
        if(_rb.velocity.x != 0 && !_wasDamaged)
        {
            float directionIndex = _rb.velocity.x / Mathf.Abs(_rb.velocity.x);
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * directionIndex, transform.localScale.y, transform.localScale.z);
        }
    }

    IEnumerator DamagedAnimation()
    {
        _wasDamaged = true;
        if (_rb.velocity.x >= 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (_rb.velocity.x < -0.1f)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        yield return new WaitForSeconds(.6f);
        _wasDamaged = false;
    }

    private void PlayAnimation()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) < 2 && !_isDead && !_playerDead)
        {
            _animator.SetTrigger("attack");
        }

        _animator.SetBool("walk", _rb.velocity.x != 0);
        _animator.SetBool("damaged", _wasDamaged);
    }

    IEnumerator DeathAnimation()
    {
        _animator.SetBool("dead", true);
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        StartCoroutine(DamagedAnimation());
        if (_health <= 0)
        {
            _isDead = true;
            _enemyCollider.enabled = false;
            _rb.gravityScale = 0;
            _rb.velocity = new Vector2(0, 0);
            StartCoroutine(DeathAnimation());
        }
    }
}

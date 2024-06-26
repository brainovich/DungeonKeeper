using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerControlls : MonoBehaviour
{
    [Header("Movement")]
    private float _speed = 70;
    private float _speedMultiplier;
    private float _horizontalInput;

    [Header("Jump")]
    private float _jumpForce = 800;
    private float _jumpCoyoteTime = .2f;
    private float _jumpCoyoteCounter;

    [Header("Fall")]
    private float _fallSpeedMultiplier = 1.05f;
    private float _fallSpeedMax = 8;

    //[Header("States")]
    public bool IsDead { get; private set; } = false;
    public bool CanShoot { get; private set; } = true;
    private bool _isGrounded = false;
    private bool _isFalling = false;
    private bool _isDashing = false;
    private bool _isOnWall = false;
    private bool _isNearWall = false;
    private bool _wasOnGround = false;
    private bool _wasOnWall = false;
    private bool _canDash = true;
    private bool _canJump = true;
    private bool _canControl = true;

    [Header("Health")]
    private int _maxHealth = 100;
    private int _currentHealth;

    [Header("Particles")]
    [SerializeField] private ParticleSystem _dustJump;
    [SerializeField] private ParticleSystem _dustRun;
    [SerializeField] private ParticleSystem _dustImpact;
    [SerializeField] private ParticleSystem _dashEffect;
    private ParticleSystem.EmissionModule dustRunEmission;
    private ParticleSystem.EmissionModule dustImpactEmission;

    [Header("Links")]
    [SerializeField] private GameObject _bullet;
    [SerializeField] private GameObject _shootingPoint;
    [SerializeField] private LayerMask _maskToAvoid;
    [HideInInspector] public Collider2D PlayerCollider { get; private set; }
    private SoundManager _soundManager;
    private Rigidbody2D _body;
    private Animator _animator;

    private Vector3 lastCheckpoint;

    private RaycastHit2D slopeHit;
    private float maxSlopeAngle = 50;
    private float wallScale;

    public void AllowShoot(bool canShoot) => CanShoot = canShoot;
    public void SetCheckpoint(Vector3 point) => lastCheckpoint = point;

    void Start()
    {
        _body = gameObject.GetComponent<Rigidbody2D>();
        PlayerCollider = GetComponent<Collider2D>();
        _animator = gameObject.GetComponent<Animator>();
        _soundManager = gameObject.GetComponent<SoundManager>();

        dustRunEmission = _dustRun.emission;
        dustImpactEmission = _dustImpact.emission;

        GameManager.instance.uiController.SetMaxHealth(_maxHealth);
        GameManager.instance.uiController.TurnOffCursor();
        _currentHealth = _maxHealth;

        lastCheckpoint = transform.position;
    }

    void Update()
    {
        Debug.Log(_body.velocity.x);

        if (!IsDead)
        {
            GetInput();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

        //ANIMATION LOGICS
        PlayParticlesEffects();
        SetAnimatorValues();
        //STATES
        CheckIfGrounded();
        CheckInAir();
        OnSlope();

        if (_wasOnWall && !_isOnWall)
        {
            transform.localScale = new Vector3(-wallScale, transform.localScale.y, 0);
        }

        _wasOnGround = _isGrounded; //do not move this line
        _wasOnWall = _isOnWall;
    }

    private void FixedUpdate()
    {
        //MOVEMENT
        if (_canControl && !IsDead)
        {
            MovePlayer();
            //FLIP PLAYER
            if (_horizontalInput > 0.01f)
                transform.localScale = Vector3.one;
            else if (_horizontalInput < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void GetInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void MovePlayer()
    {
        if (!OnSlope())
        {
            _body.AddForce((Vector2.right * _horizontalInput) * _speed * _speedMultiplier);
        }
        else if (OnSlope())
        {
            if(transform.localScale.x > 0)
            {
                _body.AddForce((GetSlopeMoveDirection() * _horizontalInput) * _speed);
            }
            else if(transform.localScale.x < 0)
            {
                _body.AddForce((GetSlopeMoveDirection() * -_horizontalInput) * _speed);
            }
        }
    }

    private void Jump()
    {
        if (_canJump)
        {
            bool jumped = false;
            //REGULAR JUMP
            if (_jumpCoyoteCounter > 0 && !_isOnWall)
            {
                _dustJump.Play();
                _body.AddForce(Vector2.up * _jumpForce);
                jumped = true;
            }
            else if (_isGrounded && _isOnWall)
            {
                //jump if on ground but touches the wall
                _dustJump.Play();
                _body.AddForce(Vector2.up * _jumpForce);
                jumped = true;
            }
            //WALL JUMP
            else if (_isOnWall && !_isGrounded)
            {
                _body.AddForce(new Vector2(-transform.localScale.x, 1.5f) * _jumpForce);
                //transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 0);
                jumped = true;
            }

            if (jumped)
            {
                _canJump = false;
                PerformCooldown(() => _canJump = true, .5f);
            }
        }
    }

    private void Dash()
    {
        if (_canDash)
        {
            _body.AddForce(new Vector2(transform.localScale.x, 0) * 30, ForceMode2D.Impulse);
            _canDash = false;
            _isDashing = true;
            _body.gravityScale = 0;
            _dashEffect.Play();
            StartCoroutine(gravityCooldown(.3f));
            PerformCooldown(() => _canDash = true, 1);
        }
    }

    private void Shoot()
    {
        if (CanShoot)
        {
            Instantiate(_bullet, _shootingPoint.transform.position, Quaternion.identity);
            _soundManager.PlayAttackSound();
            CanShoot = false;
            PerformCooldown(() => CanShoot = true, .5f);
        }
    }

    private void CheckIfGrounded()
    {
        _isGrounded = Physics2D.Raycast(transform.position, -transform.up, 1.2f, ~_maskToAvoid);
        _isOnWall = Physics2D.Raycast(transform.position, new Vector2(0.45f * transform.localScale.x, 0), 0.45f, ~_maskToAvoid);
        _isNearWall = Physics2D.Raycast(transform.position, new Vector2(0.45f * transform.localScale.x, 0), 0.8f, ~_maskToAvoid);

        if (_isGrounded)
        {
            _canControl = true;
        }
        if (!_isGrounded && !_isOnWall && !_isFalling)
        {
            _body.drag = 4;
            _speedMultiplier = 0.5f;
        }
        if (OnSlope() /*&& isGrounded*/)
        {
            _body.gravityScale = 0;
        }
        if (_isGrounded && !OnSlope())
        {
            _body.drag = 8;
            _speedMultiplier = 1;
            _body.gravityScale = 2;
            _isFalling = false;
        }
        if(!_isGrounded && !OnSlope() && _body.velocity.y > -1 && _body.velocity.y < 1)
        {
            _body.gravityScale = 2;
        }
        if (_isOnWall && !_isGrounded)
        {
            wallScale = transform.localScale.x;
            _body.drag = 10;
            _speedMultiplier = 1;
            _body.gravityScale = 2;
            _isFalling = false;
            _canControl = false;
        }
        if (_isNearWall && !_isGrounded)
        {
            _body.AddForce(new Vector3(transform.localScale.x, 0, 0) * 3);
        }
    }

    private bool OnSlope()
    {
        slopeHit = Physics2D.Raycast(transform.position, -transform.up, 1.2f, ~_maskToAvoid);
        Debug.DrawRay(transform.position, -transform.up * 1.2f, Color.yellow);
        if (slopeHit)
        {
            _isFalling = false;
            float angle = Vector2.Angle(Vector2.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector2 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(new Vector3(transform.localScale.x, 0, 0), slopeHit.normal).normalized;
    }

    private void CheckInAir()
    {
        if (_body.velocity.y > 0 && !_isGrounded && !_isOnWall &&!_isDashing)
        {
            _body.gravityScale = 2;
        }

        //enlarge gravity if falling
        if (_body.velocity.y < -1 && !_isGrounded && !_isOnWall && !OnSlope())
        {
            _isFalling = true;
            _body.gravityScale = (Mathf.Clamp(_body.gravityScale * _fallSpeedMultiplier, 2, _fallSpeedMax));
        }

        //coyote jump
        if (_isGrounded || _isOnWall)
        {
            _jumpCoyoteCounter = _jumpCoyoteTime;
        }
        else
        {
            _jumpCoyoteCounter -= Time.deltaTime;
        }
    }
    private void PerformCooldown(Action action, float time)
    {
        StartCoroutine(PerformCooldownCoroutine(action, time));
    }

    IEnumerator PerformCooldownCoroutine(Action action, float time)
    {
        yield return new WaitForSeconds(time);
        action?.Invoke();
    }

    IEnumerator gravityCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        _body.gravityScale = 2;
        _isDashing = false;
    }
    // ANIMATION LOGIC - TODO move to another script

    private void PlayParticlesEffects()
    {
        //PARTICLES PLAY
        if (!_wasOnGround && _isGrounded)
        {
            dustImpactEmission.rateOverTime = 100 * _body.gravityScale;
            _dustImpact.Play();
        }

        if (_horizontalInput != 0 && _isGrounded)
        {
            dustRunEmission.rateOverTime = 30f;
        }
        else
        {
            dustRunEmission.rateOverTime = 0;
        }
    }

    private void SetAnimatorValues()
    {
        _animator.SetBool("run", _horizontalInput != 0);
        _animator.SetBool("grounded", _isGrounded);
        _animator.SetBool("onwall", _isOnWall);
        _animator.SetBool("dead", IsDead);
    }
    IEnumerator damageAnimation()
    {
        _animator.SetBool("damaged", true);
        yield return new WaitForSeconds(.5f);
        _animator.SetBool("damaged", false);
    }


    // HEALTH LOGIC - TODO move to another script
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        StartCoroutine(damageAnimation());
        _soundManager.PlayHitSound();
        if(_currentHealth <= 0)
        {
            _currentHealth = 0;
            GameManager.instance.uiController.DeathScrren(true);
            IsDead = true;
        }

        GameManager.instance.uiController.SetHealth(_currentHealth);
    }

    public void Respawn()
    {
        transform.position = lastCheckpoint;
        PlayerCollider.enabled = true;
        _currentHealth = _maxHealth;
        GameManager.instance.uiController.SetHealth(_currentHealth);
        GameManager.instance.uiController.DeathScrren(false);
        GameManager.instance.itemsBehaviour.RespawnCoins();
        IsDead = false;
    }
}

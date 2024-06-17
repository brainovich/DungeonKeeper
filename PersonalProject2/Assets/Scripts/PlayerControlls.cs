using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerControlls : MonoBehaviour
{
    //movement
    public float speed = 8;
    public float speedMultiplier;
    float horizontalInput;
    //jumping
    public float jumpForce = 400;
    public float jumpCoyoteTime = .05f;
    public float jumpHangTime = .2f;
    public float jumpCoyoteCounter;
    public float jumpGravityMultiplier = 1.01f;
    //falling
    public float fallSpeedMultiplier = 1.01f;
    public float fallSpeedMax = 5;
    //states
    public bool isGrounded = false;
    public bool isFalling = false;
    public bool isJumping = false;
    public bool isDashing = false;
    public bool isOnWall = false;
    public bool isNearWall = false;
    public bool isOnSlope = false;
    public bool wasOnGround = false;
    public bool wasOnWall = false;
    public bool canShot = true;
    private bool canDash = true;
    private bool canJump = true;
    public bool canControl = true;
    public bool isDead = false;
    //health
    private int maxHealth = 100;
    private int currentHealth;
    //particles
    public ParticleSystem dustJump;
    public ParticleSystem dustRun;
    public ParticleSystem dustImpact;
    public ParticleSystem dashEffect;
    private ParticleSystem.EmissionModule dustRunEmission;
    private ParticleSystem.EmissionModule dustImpactEmission;
    //links
    private Rigidbody2D body;
    public Collider2D playerCollider;
    private Animator animator;
    public GameObject bullet;
    public GameObject shootingPoint;
    public LayerMask maskToAvoid;
    public TextMeshProUGUI velocityXtext;
    public TextMeshProUGUI velocityYtext;
    public SoundManager soundManager;

    public Vector3 lastCheckpoint;

    private RaycastHit2D slopeHit;
    public float maxSlopeAngle;
    private float wallScale;

    public GameObject screenDeath;


    void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        animator = gameObject.GetComponent<Animator>();

        dustRunEmission = dustRun.emission;
        dustImpactEmission = dustImpact.emission;

        GameManager.instance.uiController.SetMaxHealth(maxHealth);
        GameManager.instance.uiController.TurnOffCursor();
        currentHealth = maxHealth;

        lastCheckpoint = transform.position;
    }

    void Update()
    {
        //TEMPORARY STATE CHECK
        isOnSlope = OnSlope();

        //CONTROLS
            horizontalInput = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && canJump && !isDead)
        {
            Jump();
            canJump = false;
            StartCoroutine(jumpCooldown());
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDead)
        {
            //DASH
            body.AddForce(new Vector2(transform.localScale.x, 0) * 30, ForceMode2D.Impulse);
            canDash = false;
            isDashing = true;
            body.gravityScale = 0;
            dashEffect.Play();
            StartCoroutine(gravityCooldown(.3f));
            StartCoroutine(dashCooldown());
        }
        if (Input.GetMouseButtonDown(0) && canShot && !isDead)
        {
            Instantiate(bullet, shootingPoint.transform.position, Quaternion.identity);
            soundManager.PlayAttackSound();
            canShot = false;
            StartCoroutine(shotCooldown());
        }

        //PARTICLES PLAY
        if(!wasOnGround && isGrounded)
        {
            dustImpactEmission.rateOverTime = 100 * body.gravityScale;
            dustImpact.Play();
        }

        if (horizontalInput != 0 && isGrounded)
        {
            dustRunEmission.rateOverTime = 30f;
        }
        else
        {
            dustRunEmission.rateOverTime = 0;
        }
        //ANIMATOR VALUES
        animator.SetBool("run", horizontalInput != 0);
        animator.SetBool("grounded", isGrounded);
        animator.SetBool("onwall", isOnWall);
        animator.SetBool("dead", isDead);
        //STATES
        CheckIfGrounded();
        CheckInAir();
        OnSlope();
        //DEATH SYSTEM
        if (currentHealth <= 0)
        {
            screenDeath.SetActive(true);
            isDead = true;
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = lastCheckpoint;
                playerCollider.enabled = true;
                currentHealth = maxHealth;
                GameManager.instance.uiController.SetHealth(currentHealth);
                screenDeath.SetActive(false);
                GameManager.instance.itemsBehaviour.RespawnCoins();
                isDead = false;
            }
        }

        if (wasOnWall && !isOnWall)
        {
            transform.localScale = new Vector3(-wallScale, transform.localScale.y, 0);
        }

        //velocityXtext.text = "X:" + body.velocity.x.ToString("0.00");
        //velocityYtext.text = "Y:" + body.velocity.y.ToString("0.00");

        wasOnGround = isGrounded; //do not move this line
        wasOnWall = isOnWall;
    }

    private void FixedUpdate()
    {
        //MOVEMENT
        if (canControl && !isDead)
        {
            MovePlayer();
            //FLIP PLAYER
            if (horizontalInput > 0.01f)
                transform.localScale = Vector3.one;
            else if (horizontalInput < -0.01f)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void MovePlayer()
    {
        if (!OnSlope())
        {
            body.AddForce((Vector2.right * horizontalInput) * speed * speedMultiplier);
        }
        else if (OnSlope())
        {
            if(transform.localScale.x > 0)
            {
                body.AddForce((GetSlopeMoveDirection() * horizontalInput) * speed);
            }
            else if(transform.localScale.x < 0)
            {
                body.AddForce((GetSlopeMoveDirection() * -horizontalInput) * speed);
            }
            
            Debug.DrawRay(transform.position, GetSlopeMoveDirection()*2, Color.yellow);
        }
    }

    private void Jump()
    {
        //REGULAR JUMP
        if (jumpCoyoteCounter > 0 && !isOnWall)
        {
            dustJump.Play();
            body.AddForce(Vector2.up * jumpForce);
        }
        else if (isGrounded && isOnWall)
        {
            //jump if on ground but touches the wall
            dustJump.Play();
            body.AddForce(Vector2.up * jumpForce);
        }
        //WALL JUMP
        else if (isOnWall && !isGrounded)
        {
            body.AddForce(new Vector2(-transform.localScale.x, 1.5f) * jumpForce);
            //transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 0);
        }
    }

    void CheckIfGrounded()
    {
        isGrounded = Physics2D.Raycast(transform.position, -transform.up, 1.2f, ~maskToAvoid);
        isOnWall = Physics2D.Raycast(transform.position, new Vector2(0.45f * transform.localScale.x, 0), 0.45f, ~maskToAvoid);
        isNearWall = Physics2D.Raycast(transform.position, new Vector2(0.45f * transform.localScale.x, 0), 0.8f, ~maskToAvoid);
        Debug.DrawRay(transform.position, new Vector2(0.8f * transform.localScale.x, 0), Color.green, 0);
        if (isGrounded)
        {
            canControl = true;
        }
        if (!isGrounded && !isOnWall && !isFalling)
        {
            body.drag = 4;
            speedMultiplier = 0.5f;
        }
        if (OnSlope() /*&& isGrounded*/)
        {
            body.gravityScale = 0;
        }
        if (isGrounded && !OnSlope())
        {
            body.drag = 8;
            speedMultiplier = 1;
            body.gravityScale = 2;
            isFalling = false;
            isJumping = false;
        }
        if(!isGrounded && !OnSlope() && body.velocity.y > -1 && body.velocity.y < 1)
        {
            body.gravityScale = 2;
        }
        if (isOnWall && !isGrounded)
        {
            wallScale = transform.localScale.x;
            body.drag = 10;
            speedMultiplier = 1;
            body.gravityScale = 2;
            isFalling = false;
            isJumping = false;
            canControl = false;
        }
        if (isNearWall && !isGrounded)
        {
            body.AddForce(new Vector3(transform.localScale.x, 0, 0) * 3);
        }
    }

    private bool OnSlope()
    {
        slopeHit = Physics2D.Raycast(transform.position, -transform.up, 1.2f, ~maskToAvoid);
        Debug.DrawRay(transform.position, -transform.up * 1.2f, Color.yellow);
        if (slopeHit)
        {
            isFalling = false;
            float angle = Vector2.Angle(Vector2.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector2 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(new Vector3(transform.localScale.x, 0, 0), slopeHit.normal).normalized;
    }

    void CheckInAir()
    {
        if (body.velocity.y > 0 && !isGrounded && !isOnWall &&!isDashing)
        {
            isJumping = true;
            body.gravityScale = 2;
        }

        //enlarge gravity if falling
        if (body.velocity.y < -1 && !isGrounded && !isOnWall && !OnSlope())
        {
            isJumping = false;
            isFalling = true;
            body.gravityScale = (Mathf.Clamp(body.gravityScale * fallSpeedMultiplier, 2, fallSpeedMax));
        }

        //minimize gravity in the higher point of jump ÍÅ ÐÀÁÎÒÀÅÒ ÏÅÐÅÄÅËÀÒÜ!!
        /*if (isJumping && Mathf.Abs(body.velocity.y) < jumpHangTime)
        {
            body.gravityScale = body.gravityScale * jumpGravityMultiplier;
        }*/

        //coyote jump
        if (isGrounded || isOnWall)
        {
            jumpCoyoteCounter = jumpCoyoteTime;
        }
        else
        {
            jumpCoyoteCounter -= Time.deltaTime;
        }
    }

    IEnumerator shotCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        canShot = true;
    }

    IEnumerator dashCooldown()
    {
        yield return new WaitForSeconds(1);
        canDash = true;
    }
    IEnumerator jumpCooldown()
    {
        yield return new WaitForSeconds(.5f);
        canJump = true;
    }

    IEnumerator gravityCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        body.gravityScale = 2;
        isDashing = false;
    }

    IEnumerator damageAnimation()
    {
        animator.SetBool("damaged", true);
        yield return new WaitForSeconds(.5f);
        animator.SetBool("damaged", false);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(damageAnimation());
        GameManager.instance.uiController.SetHealth(currentHealth);
        soundManager.PlayHitSound();
    }
}

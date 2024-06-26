using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonAI : MonoBehaviour
{
    public float visibilityRange;
    public float shootingRange;
    public float shootingCooldown;
   
    
    public float min = 0;
    public float max = 0;
    public float current = 0;
    public bool minimize = false;
    public float gunRotationSpeed = 1;

    private Transform target;
    public GameObject gun;
    public GameObject projectile;
    public GameObject shootingPoint;
    public LayerMask maskToIgnore;
    public SoundManager soundManager;
    
    private RaycastHit2D visibilityDetector;

    private Vector2 direction;

    private bool canShoot = true;
    public bool isIdle = true;
    public bool isDetected = false;


    // Start is called before the first frame update
    void Start()
    {
        target = GameManager.instance.playerControlls.transform;
    }

    // Update is called once per frame
    void Update()
    {
        direction = (target.position - transform.position).normalized;
        visibilityDetector = Physics2D.Raycast(transform.position, direction, visibilityRange, ~maskToIgnore);
        Debug.DrawRay(transform.position, direction * visibilityRange, Color.green);
        Debug.DrawRay(transform.position, direction * shootingRange, Color.red);

        
        isDetected = visibilityDetector.collider.gameObject.tag == "Player";
        isIdle = !isDetected;

        if (visibilityDetector.collider.gameObject.tag == "Player" && !GameManager.instance.playerControlls.IsDead)
        {
            gun.transform.right = direction;
            if (Vector3.Distance(transform.position, target.position) < shootingRange && canShoot)
            {
                Instantiate(projectile, shootingPoint.transform.position, Quaternion.identity);
                soundManager.PlayAttackSound();
                StartCoroutine(cooldownTimer());
            }
        }

        if (isIdle)
        {
            gun.transform.eulerAngles = new Vector3(0, 0, current);
            if(current > max)
            {
                minimize = true;
            }
            if(current < min)
            {
                minimize = false;
            }

            if (minimize)
            {
                current -= Time.deltaTime * gunRotationSpeed;
            }
            if (!minimize)
            {
                current += Time.deltaTime * gunRotationSpeed;
            }
        }
    }

    IEnumerator cooldownTimer()
    {
        canShoot = false;
        yield return new WaitForSeconds(shootingCooldown);
        canShoot = true;
    }
}

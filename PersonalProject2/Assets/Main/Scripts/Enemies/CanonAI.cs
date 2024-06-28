using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonAI : MonoBehaviour
{
    [Header("Rotation settings")]
    [SerializeField] private float _minRotation;
    [SerializeField] private float _maxRotation;
    [SerializeField] private float _rotationSpeed;
    private float _currentRotation = 90;

    [Header("Aiming and cooldown")]
    private float _visibilityRange = 15;
    private float _shootingRange = 6;
    private float _shootingCooldown = 1;
    private Vector2 _pointDirection;
    private RaycastHit2D _visibilityDetector;

    [Header("Links")]
    [SerializeField] private GameObject _gun;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private GameObject _shootingPoint;
    [SerializeField] private LayerMask _maskToIgnore;
    private Transform _target;
    private SoundManager _soundManager;

    [Header("States")]
    private bool _minimize = false;
    private bool _canShoot = true;
    private bool _isIdle = true;

    void Start()
    {
        _target = GameManager.instance.playerControlls.transform;
        _soundManager = GetComponent<SoundManager>();
    }

    void Update()
    {
        _pointDirection = (_target.position - transform.position).normalized;
        _visibilityDetector = Physics2D.Raycast(transform.position, _pointDirection, _visibilityRange, ~_maskToIgnore);

        _isIdle = !IsDetected();

        if (IsDetected() && !GameManager.instance.playerControlls.IsDead)
        {
            Shoot();
        }
        else if (_isIdle)
        {
            IdleRotation();
        }
    }
    private bool IsDetected()
    {
        if(_visibilityDetector.collider != null)
        {
            return _visibilityDetector.collider.gameObject.tag == "Player";
        }
        else
        {
            return false;
        }
    }

    private void Shoot()
    {
        _gun.transform.right = _pointDirection;
        if (Vector3.Distance(transform.position, _target.position) < _shootingRange && _canShoot)
        {
            Instantiate(_projectile, _shootingPoint.transform.position, Quaternion.identity);
            _soundManager.PlayAttackSound();
            StartCoroutine(cooldownTimer());
        }
    }

    private void IdleRotation()
    {
        _gun.transform.eulerAngles = new Vector3(0, 0, _currentRotation);
        if (_currentRotation > _maxRotation)
        {
            _minimize = true;
        }
        if (_currentRotation < _minRotation)
        {
            _minimize = false;
        }

        if (_minimize)
        {
            _currentRotation -= Time.deltaTime * _rotationSpeed;
        }
        if (!_minimize)
        {
            _currentRotation += Time.deltaTime * _rotationSpeed;
        }
    }

    IEnumerator cooldownTimer()
    {
        _canShoot = false;
        yield return new WaitForSeconds(_shootingCooldown);
        _canShoot = true;
    }
}

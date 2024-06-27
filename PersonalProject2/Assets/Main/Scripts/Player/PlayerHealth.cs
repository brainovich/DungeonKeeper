using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    private int _maxHealth = 100;
    private int _currentHealth;
    public bool IsDead { get; private set; } = false;
    private Action _damageCallback;
    private UIController _uiController;
    private PlayerControlls _contoller;

    public void Initialize(int maxHealth, Action damageCallback)
    {
        _uiController = GameManager.instance.uiController;
        _contoller = GameManager.instance.playerControlls;
        _maxHealth = maxHealth;
        _currentHealth = _maxHealth;
        _damageCallback = damageCallback;

        GameManager.instance.uiController.SetMaxHealth(_maxHealth);
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _damageCallback?.Invoke();

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            _uiController.DeathScrren(true);
            _contoller.SetDead(true);
        }

        _uiController.SetHealth(_currentHealth);
    }

    public void Respawn(Action respawnCallback)
    {
        _currentHealth = _maxHealth;
        GameManager.instance.uiController.SetHealth(_currentHealth);
        GameManager.instance.uiController.DeathScrren(false);
        GameManager.instance.itemsBehaviour.RespawnCoins();
        _contoller.SetDead(false);
        respawnCallback?.Invoke();
    }
}

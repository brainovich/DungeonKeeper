using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIDetector : MonoBehaviour
{
    private EnemyAI _enemyAI;

    private void Start()
    {
        _enemyAI = GetComponentInChildren<EnemyAI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !GameManager.instance.playerControlls.IsDead)
        {
            _enemyAI.DetectEnemy(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _enemyAI.DetectEnemy(false);
        }
    }
}

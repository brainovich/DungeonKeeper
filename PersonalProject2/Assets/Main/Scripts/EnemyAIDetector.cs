using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIDetector : MonoBehaviour
{
    public bool isDetected = false;
    public EnemyAI enemyAI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !GameManager.instance.playerControlls.IsDead)
        {
            enemyAI.enemyInArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            enemyAI.enemyInArea = false;
        }
    }
}

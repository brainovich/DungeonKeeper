using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehavoiur : MonoBehaviour
{
    public bool notRespawn = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            GameManager.instance.scoreManager.IncrementScore();
        }
    }
}

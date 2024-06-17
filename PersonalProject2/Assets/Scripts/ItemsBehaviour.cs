using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsBehaviour : MonoBehaviour
{
    public bool testNewSystem;
    private GameObject[] coins;
    public int coinsAtLevel;

    private void Start()
    {
        coins = GameObject.FindGameObjectsWithTag("Coin");
        for(int i = 0; i < coins.Length; i++)
        {
            Debug.Log(i);
        }
        coinsAtLevel = coins.Length;
    }

    public void SaveScore()
    {
        for(int i = 0; i < coins.Length; i++)
        {
            CoinBehavoiur coinBehavoiur = coins[i].GetComponent<CoinBehavoiur>();
            if (!coins[i].activeSelf) 
            {
                coinBehavoiur.notRespawn = true;
            }   
        }
    }

    public void RespawnCoins()
    {
        for (int j = 0; j < coins.Length; j++)
        {
            CoinBehavoiur coinBehavoiur = coins[j].GetComponent<CoinBehavoiur>();
            if (!coinBehavoiur.notRespawn && !coins[j].activeSelf)
            {
                coins[j].SetActive(true);
                GameManager.instance.scoreManager.DecrementScore();
            }
        }
    }
}

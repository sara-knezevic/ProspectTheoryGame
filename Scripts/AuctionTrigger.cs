using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuctionTrigger : MonoBehaviour
{
    public GameObject gameManager;

    private void OnTriggerEnter(Collider other)
    {
        gameManager.GetComponent<GameManager>().addBonus();
        this.GetComponent<BoxCollider>().enabled = false;
    }
}

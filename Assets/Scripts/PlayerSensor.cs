using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item") || other.CompareTag("Door") || other.CompareTag("Stair"))
        {
            player.AddToList(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item") || other.CompareTag("Door") || other.CompareTag("Stair"))
        {
            player.RemoveFromList(other.gameObject);
        }
    }
}

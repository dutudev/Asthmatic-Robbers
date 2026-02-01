using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueFinalCounter : MonoBehaviour
{
    private List<Item> itemsCollected = new List<Item>();
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            itemsCollected.Add(other.GetComponent<Item>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            if (itemsCollected.Contains(other.GetComponent<Item>()))
            {
                itemsCollected.Remove(other.GetComponent<Item>());
            }
        }
    }

    public int GetValue()
    {
        int val = 0;
        foreach (var item in itemsCollected)
        {
            val += item.GetValue();
        }

        return val;
    }
}

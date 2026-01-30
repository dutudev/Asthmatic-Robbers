using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractible
{
    [SerializeField] private ObjectData objectData;
    
    public void Interact()
    {
        
        Destroy(gameObject);
    }

    public float GetTextHeight()
    {
        return objectData.heightText;
    }

    public GameObject GetPrefab()
    {
        return objectData.prefab;
    }

    public string GetName()
    {
        return objectData.name;
    }
    
    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + objectData.heightText));
    }
}



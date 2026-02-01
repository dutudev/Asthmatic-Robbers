using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Item : MonoBehaviour, IInteractible
{
    [SerializeField] private ObjectData objectData;
    [SerializeField] private ObjectDataRuntime objectDataRuntime;
    
    public virtual void Interact()
    {
        Destroy(gameObject);
    }

    public void Awake()
    {
        if ((objectDataRuntime.value == 0 || objectDataRuntime.weight == 0) && objectData != null)
        {
            objectDataRuntime = new ObjectDataRuntime();
            objectDataRuntime.value = objectData.value + Random.Range(-45, 46);
            objectDataRuntime.weight = objectData.weight + Mathf.Floor(Random.Range(-0.3f, 0.3f) * 100) / 100f;
        }
    }

    public virtual float GetTextHeight()
    {
        return objectData.heightText;
    }

    public GameObject GetPrefab()
    {
        return objectData.prefab;
    }

    public string GetName()
    {
        return objectData.itemName;
    }

    public float GetWeight()
    {
        return objectDataRuntime.weight;
    }

    public int GetValue()
    {
        return objectDataRuntime.value;
    }

    public Sprite GetSprite()
    {
        return objectData.uiImage;
    }

    public ObjectDataRuntime GetDataRuntime()
    {
        return objectDataRuntime;
    }

    public void SetObjectData(ObjectDataRuntime data)
    {
        objectDataRuntime = data;
    }
    
    public void OnDrawGizmos()
    {
        //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + objectData.heightText));
    }
}



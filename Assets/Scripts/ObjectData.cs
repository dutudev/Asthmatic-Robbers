using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "ScriptableObjects/Object", order = 1)]
public class ObjectData : ScriptableObject
{
    public string itemName;
    public float weight;
    public float heightText;
    public Sprite uiImage;
    public GameObject prefab;
}
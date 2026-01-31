using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private AnimationCurve itemImageCurve;

    private List<Item> itemsDisplayed = new List<Item>();
    public static UIManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateItems(List<Item> items)
    {
        
        itemsDisplayed = new List<Item>(items);
    }

    void UpdateItemImages()
    {
        
    }
}

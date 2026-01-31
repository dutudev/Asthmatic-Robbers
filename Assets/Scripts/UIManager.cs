using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private AnimationCurve itemImageCurve;
    [SerializeField] private GameObject imageHolder, imagePrefab;
    
    private List<Item> itemsDisplayed = new List<Item>();
    private List<Image> itemsImages = new List<Image>();
    private int _currentItem = 0; // -1 for inhaler?
    public static UIManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
        //print();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateItems(List<Item> items)
    {
        LeanTween.cancel(imageHolder);
        itemsDisplayed = new List<Item>(items);
        itemsImages.Clear();
        for(int i = 0; i < imageHolder.transform.childCount; i++)
        {
            Destroy(imageHolder.transform.GetChild(i).gameObject);
        }
        // add image for inhaler
        int itemC = 0;
        foreach(var item in itemsDisplayed)
        {
            var obj = Instantiate(imagePrefab, imageHolder.transform);
            obj.transform.localPosition = new Vector3(-62, -256 * itemC); // change y
            obj.GetComponent<Image>().sprite = item.GetSprite();
            itemsImages.Add(obj.GetComponent<Image>());
            
            itemC++;
        }
        if (itemsDisplayed.Count == 0)
        {
            _currentItem = -1;
        }else if (itemsDisplayed.Count >= _currentItem)
        {
            _currentItem = itemsDisplayed.Count - 1;
        }
        AnimateItems();
    }

    public void MoveItemUp()
    {
        _currentItem--;
        if (_currentItem < -1)
        {
            _currentItem = itemsDisplayed.Count - 1;
        }
        LeanTween.cancel(imageHolder);
        AnimateItems();
    }

    public void MoveItemDown()
    {
        _currentItem++;
        if (_currentItem >= itemsDisplayed.Count)
        {
            _currentItem = -1;
        }
        LeanTween.cancel(imageHolder);
        AnimateItems();
        
    }

    public int GetCurrentItem()
    {
        return _currentItem;
    }
    
    private void AnimateItems()
    {

        LeanTween.moveLocal(imageHolder, new Vector3(0, 39 + 256 * _currentItem), 1f).setEaseOutExpo().setOnUpdate((float v) =>
        {
            UpdateItemImages();
        });
    }

    void UpdateItemImages()
    {
        //print(itemsImages[0].gameObject.transform.position.y);
        int itemc = 0;
        foreach (var item in itemsImages)
        {

            item.gameObject.transform.localScale = Vector3.one * itemImageCurve.Evaluate((imageHolder.gameObject.transform.localPosition.y - 39 + 256f / 2f - 256f * itemc ) / 256f);
            itemc++;
        }
    }
}

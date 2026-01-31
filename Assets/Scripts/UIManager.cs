using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private AnimationCurve itemImageCurve;
    [SerializeField] private GameObject imageHolder, imagePrefab;
    [SerializeField] private List<TMP_Text> uiTexts;
    [SerializeField] private Sprite inhalerSprite;
    
    private List<Item> itemsDisplayed = new List<Item>();
    private List<Image> itemsImages = new List<Image>();
    private List<CanvasGroup> itemsCanvasGroups = new List<CanvasGroup>();
    private List<CanvasGroup> uiTextCanvasGroups = new List<CanvasGroup>();
    private int _currentItem = 0; // -1 for inhaler?
    public static UIManager instance { get; private set; }

    private void Awake()
    {
        instance = this;
        //print();
        
        foreach (var text in uiTexts)
        {
            uiTextCanvasGroups.Add(text.gameObject.GetComponent<CanvasGroup>());
        }
        UpdateItems(new List<Item>());
    }

    // Update is called once per frame
    void Update()
    {
        RotateItemImages();
    }

    public void UpdateItems(List<Item> items)
    {
        LeanTween.cancel(imageHolder);
        itemsDisplayed = new List<Item>(items);
        itemsImages.Clear();
        itemsCanvasGroups.Clear();
        for(int i = 0; i < imageHolder.transform.childCount; i++)
        {
            Destroy(imageHolder.transform.GetChild(i).gameObject);
        }
        // add image for inhaler
        int itemC = -1;
        var obj = Instantiate(imagePrefab, imageHolder.transform);
        obj.transform.localPosition = new Vector3(-62, -128 * itemC); // change y
        obj.GetComponent<Image>().sprite = inhalerSprite;
        itemsImages.Add(obj.GetComponent<Image>());
        itemsCanvasGroups.Add(obj.GetComponent<CanvasGroup>());
        itemC++;
        foreach(var item in itemsDisplayed)
        {
            obj = Instantiate(imagePrefab, imageHolder.transform);
            obj.transform.localPosition = new Vector3(-62, -128 * itemC); // change y
            obj.GetComponent<Image>().sprite = item.GetSprite();
            itemsImages.Add(obj.GetComponent<Image>());
            itemsCanvasGroups.Add(obj.GetComponent<CanvasGroup>());
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
        AnimateText();
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
        AnimateText();
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
        AnimateText();
    }

    public int GetCurrentItem()
    {
        return _currentItem;
    }

    private void AnimateText()
    {
        LeanTween.cancel(uiTextCanvasGroups[0].gameObject);
        LeanTween.cancel(uiTextCanvasGroups[1].gameObject);
        LeanTween.cancel(uiTextCanvasGroups[2].gameObject);
        if (_currentItem == -1)
        {
            uiTexts[0].text = "Inhaler";
            uiTexts[1].text = "Press Space to use";
            uiTexts[2].text = "";
            uiTextCanvasGroups[0].alpha = 0;
            uiTextCanvasGroups[1].alpha = 0;
            uiTextCanvasGroups[2].alpha = 0;
            LeanTween.alphaCanvas(uiTextCanvasGroups[0], 1, 0.6f).setEaseOutExpo();
            LeanTween.alphaCanvas(uiTextCanvasGroups[1], 1, 0.6f).setEaseOutExpo();
            LeanTween.alphaCanvas(uiTextCanvasGroups[2], 1, 0.6f).setEaseOutExpo();
            return;
        }

        uiTexts[0].text = itemsDisplayed[_currentItem].GetName();
        uiTexts[1].text = "Weight : " + itemsDisplayed[_currentItem].GetWeight();
        uiTexts[2].text = "Value : " + itemsDisplayed[_currentItem].GetValue();
        uiTextCanvasGroups[0].alpha = 0;
        uiTextCanvasGroups[1].alpha = 0;
        uiTextCanvasGroups[2].alpha = 0;
        LeanTween.alphaCanvas(uiTextCanvasGroups[0], 1, 0.6f).setEaseOutExpo();
        LeanTween.alphaCanvas(uiTextCanvasGroups[1], 1, 0.6f).setEaseOutExpo();
        LeanTween.alphaCanvas(uiTextCanvasGroups[2], 1, 0.6f).setEaseOutExpo();
    }
    
    private void AnimateItems()
    {

        LeanTween.moveLocal(imageHolder, new Vector3(0, 39 + 128 * _currentItem), 1f).setEaseOutExpo().setOnUpdate((float v) =>
        {
            UpdateItemImages();
        }).setOnComplete(() =>
        {
            imageHolder.transform.localPosition = new Vector3(0, 39 + 128 * _currentItem);
            UpdateItemImages();
        });
    }

    void UpdateItemImages()
    {
        
        foreach (var item in itemsImages)
        { 
            Vector2 positionBased = item.transform.localPosition + imageHolder.transform.localPosition;
            item.gameObject.transform.localScale = Vector3.one * itemImageCurve.Evaluate(0.5f + Mathf.Abs((positionBased.y - 39f) / 460.8f));
        }
        foreach (var item in itemsCanvasGroups)
        { 
            Vector2 positionBased = item.transform.localPosition + imageHolder.transform.localPosition;
            item.alpha = itemImageCurve.Evaluate(0.5f + Mathf.Abs((positionBased.y - 39f) / 384f));
        }
    }

    void RotateItemImages()
    {
        foreach (var item in itemsImages)
        {
            item.gameObject.transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time) * 15);
        }
    }
}

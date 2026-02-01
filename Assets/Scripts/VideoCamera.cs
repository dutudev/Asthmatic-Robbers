using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class VideoCamera : MonoBehaviour
{
    [SerializeField] private GameObject triangle;
    [SerializeField] private SpriteRenderer triangleSpriteRenderer;

    private Material _triangleMat;
    private float _nextScan;
    
    // Start is called before the first frame update
    void Start()
    {
        _triangleMat = triangleSpriteRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _nextScan)
        {
            Scan();
        }
    }


    void Scan()
    {
        _nextScan = Time.time + Random.Range(6f, 20.0f);
        LeanTween.cancel(triangle);
        triangle.SetActive(true);
        float startPos, endPos;
        if (Random.Range(0, 2) == 0)
        {
            startPos = 70;
            endPos = -70;
        }
        else
        {
            startPos = -70;
            endPos = 70;
        }
        triangle.transform.localRotation = Quaternion.Euler(0, 0, startPos);
        _triangleMat.SetFloat("_transparency", 0);
        LeanTween.rotate(triangle, new Vector3(0, 0, endPos), 3f).setOnComplete(() =>
        {
            triangle.SetActive(false);
        });
        LeanTween.value(triangle, 0, 1, 0.3f).setOnUpdate((float f) =>
        {
            _triangleMat.SetFloat("_transparency", f);
        });
        LeanTween.value(triangle, 1, 0, 0.3f).setOnUpdate((float f) =>
        {
            _triangleMat.SetFloat("_transparency", f);
        }).setDelay(2.7f);
    }
}

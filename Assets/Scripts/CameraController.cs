using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject followTarget;
    [SerializeField] private Vector2 followOffset;
    [SerializeField] private float followSpeed;

    private Vector3 _finalLocation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _finalLocation = new Vector3(followTarget.transform.position.x + followOffset.x, followTarget.transform.position.y + followOffset.y, -10);
        transform.position = Vector3.Lerp(transform.position, _finalLocation, followSpeed * Time.deltaTime);
    }
}

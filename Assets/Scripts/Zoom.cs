using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Zoom : MonoBehaviour
{
    public GameObject cameraObject;  // Reference to your 2DViewCamera
    
    Slider slider;  // Reference to your Slider component

    public float min;

    Vector3 StartPosition;

    private void Start()
    {
        slider = GetComponent<Slider>();
        StartPosition = cameraObject.transform.position;
    }

    private void Update()
    {
        float value = slider.value;
        float newYPosition = Mathf.Lerp(StartPosition.y, min, value);  
        Vector3 cameraPosition = cameraObject.transform.position;
        cameraPosition.y = newYPosition;
        cameraObject.transform.position = cameraPosition;
    }
}

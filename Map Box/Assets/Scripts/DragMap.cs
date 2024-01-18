using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragMap : MonoBehaviour
{
    public float centerX = MapControl.centerLongitude; 
    public float centerY = MapControl.centerLatitude; 

    private float beforeMouseX;
    private float beforeMouseY; 
    private float diffMouseX;
    private float diffMouseY;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BeforeDrag();
        }
        if (Input.GetMouseButtonUp(0))
        {
            AfterDrag();
        }
    }

    private void BeforeDrag()
    {
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            //Debug.Log("Before: X Value -" + Input.GetAxis("Mouse X") + "Y Value -" + Input.GetAxis("Mouse Y"));
            beforeMouseX = Input.GetAxis("Mouse X");
            beforeMouseY = Input.GetAxis("Mouse Y"); 
        }
    }
    private void AfterDrag()
    {
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            //Debug.Log("After: X Value - " + Input.GetAxis("Mouse X") + "Y Value - " + Input.GetAxis("Mouse Y"));
            diffMouseX = beforeMouseX - Input.GetAxis("Mouse X");
            diffMouseY = beforeMouseY - Input.GetAxis("Mouse Y");
        }
    }
}



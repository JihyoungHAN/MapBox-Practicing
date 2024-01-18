using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    
    private Vector3 mouseWorldPosStart; 
    private float rotationSpeed = 500.0f; 
    private float zoomScale = 50.0f;
    private float zoomMax = 10000.0f;
    private float zoomMin = 0.5f;

    // Update is called once per frame
    void Update()
    {
        //LeftShite + Middle Mouse Button 
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Mouse2)) 
        {
            CamOrbit();
        }
        if (Input.GetMouseButtonDown(2) && !Input.GetKey(KeyCode.LeftShift))
        {
            mouseWorldPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
        }
        if (Input.GetMouseButton(2) && !Input.GetKey(KeyCode.LeftShift))
        {
            Pan(); 
        }

        Zoom(Input.GetAxis("Mouse ScrollWheel")); 
    }

    private void CamOrbit()
    {
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            float verticalInput = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            float horizontalInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime; 
            transform.Rotate(Vector3.right, verticalInput); 
            transform.Rotate(Vector3.up, horizontalInput, Space.World); 
        }
    }

    private void Pan()
    {
        if (Input.GetAxis("Mouse Y") != 0 || Input.GetAxis("Mouse X") != 0)
        {
            Vector3 mouseWorldPosDiff = mouseWorldPosStart - Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            transform.position += mouseWorldPosDiff; 
        }
    }

    private void Zoom(float zoomDiff)
    {
        if (zoomDiff != 0)
        {
            mouseWorldPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomDiff * zoomScale, zoomMin, zoomMax);
            Vector3 mouseWorldPosDiff = mouseWorldPosStart - Camera.main.ScreenToWorldPoint(Input.mousePosition); 
            transform.position += mouseWorldPosDiff; 
        }
    }
}

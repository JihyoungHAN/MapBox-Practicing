using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Control : MonoBehaviour
{

    private float zoomDiff;
    private float zoomScale = 5.0f;

    // Update is called once per frame
    void Update()
    {
        zoomDiff = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDiff != 0)
        {
            if (zoomDiff * zoomScale < 0 )
            {
                Map.zoom = 0;
            }
            else if (zoomDiff * zoomScale > 22)
            {
                Map.zoom = 22;   
            }
            else
            {
                Map.zoom += zoomDiff * zoomScale; 
            }
            
        }
    }

}

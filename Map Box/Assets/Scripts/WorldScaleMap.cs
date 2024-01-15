using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Networking;
using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

public class WorldScaleMap : MonoBehaviour
{
    public float centerLongitude = -121.7498f; 
    public float centerLatitude = 38.5419f;
    public enum style { Streets, Outdoors, Light, Dark, Satellite, SatelliteStreets, NavigationDay, NavigationNight };
    public style mapStyle = style.Streets;
    public string accessToken;  

    //private bool mapIsLoading = false; 

    private double planeToCameraDistance; //Distance between plane and camera (y values' difference)
    private double mapWidthMeter; 
    private double mapHeightMeter; 
    private int mapWidthPx = 1280; 
    private int mapHeightPx = 1280; 
    private Material mapMaterial; 
    private Vector2 screenResolution; 

    private string[] styleStr = new string[] {"streets-v12", "outdoors-v12", "light-v11", "dark-v11", "satellite-v9", "satellite-streets-v12", "navigation-day-v1", "navigation-night-v1"};  

    private double[] boundingBox; 
    private string url = ""; 

    
    //Saving last value
    private float centerLatitudeLast = 38.5419f;
    private float centerLongitudeLast = -121.7498f; 
    private string accessTokenLast; 
    private style mapStyleLast = style.Streets; 
    private bool updateMap = true;
    private bool resetMap = false;  

    // Start is called before the first frame update
    void Start()
    {   
        planeToCameraDistance = Vector3.Distance(gameObject.transform.position, Camera.main.transform.position); 
        screenResolution = new Vector2(Screen.width, Screen.height); 
        MatchPlaneToScreenSize();
        if (gameObject.GetComponent<MeshRenderer>() == null)
        {
            gameObject.AddComponent<MeshRenderer>(); 
        }
        mapMaterial = new Material(Shader.Find("Unlit/Texture")); 
        gameObject.GetComponent<MeshRenderer>().material = mapMaterial; 
        StartCoroutine(GetMapBox()); 
    }

    // Update is called once per frame
    void Update()
    {
        //screen size or camera positon is changed 
        if (resetMap || screenResolution.x != Screen.width || screenResolution.y != Screen.height || !Mathf.Approximately((float)planeToCameraDistance, Vector3.Distance(gameObject.transform.position, Camera.main.transform.position)))
        {
            planeToCameraDistance = Vector3.Distance(gameObject.transform.position, Camera.main.transform.position); 
            screenResolution.x = Screen.width; 
            screenResolution.y = Screen.height; 
            MatchPlaneToScreenSize();
            StartCoroutine(GetMapBox());
            updateMap = false; 
        }
        //checking map attribute is changed or not
        else if (resetMap || (updateMap && (accessTokenLast != accessToken || !Mathf.Approximately(centerLatitudeLast, centerLatitude) || Mathf.Approximately(centerLongitudeLast, centerLongitude) ||  mapStyleLast != mapStyle)))
        {
            StartCoroutine(GetMapBox());
            updateMap = false; 
        }
    }

    private void MatchPlaneToScreenSize()
    {
        // planeHeightScale: The proportion of the plane's area in ​​the camera's field
        // (2.0 * Math.Tan(0.5f * Camera.main.fieldOfView * (Math.PI / 180)) : The area size that the camera can cover 
        // Plane size is 10 units(10*10) 
        double planeHeightScale = (2.0 * Math.Tan(0.5f * Camera.main.fieldOfView * (Math.PI / 180)) * planeToCameraDistance) / 10.0; 
        double planeWidthScale = planeHeightScale * Camera.main.aspect; 
        gameObject.transform.localScale = new Vector3((float)planeWidthScale, 1, (float)planeHeightScale);

        // To making map real size fit with the camera field  
        mapWidthMeter = planeWidthScale * 10; 
        mapHeightMeter = planeHeightScale * 10; 
        if(Camera.main.aspect > 1) // Width is bigger than height 
        {
            mapWidthPx = 1280; 
            mapHeightPx = (int)Math.Round(1280 / Camera.main.aspect); 

        }
        else // Height is bigger than width
        {
            mapHeightPx = 1280;     
            mapWidthPx = (int)Math.Round(1280 / Camera.main.aspect); 
        }
        
        //mapIsLoading = false; 
    }

    IEnumerator GetMapBox()
    {
        boundingBox = GetRectCoord(centerLongitude, centerLatitude, mapWidthMeter, mapHeightMeter);
        url = "https://api.mapbox.com/styles/v1/mapbox/" + styleStr[(int)mapStyle] + "/static/[" + boundingBox[0] + "," + boundingBox[1] + "," + boundingBox[2] + "," + boundingBox[3] + "]/" + mapWidthPx + "x" + mapHeightPx + "?access_token=" + accessToken; 
        //mapIsLoading = true; 
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
        yield return webRequest.SendWebRequest(); 
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            //mapIsLoading = false; 
            Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture; 
            gameObject.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", (texture));  

            accessTokenLast = accessToken; 
            centerLatitudeLast = centerLatitude;
            centerLongitudeLast = centerLongitude;
            mapStyleLast = mapStyle; 
            
            updateMap = true; 
            resetMap = false;
        }
        else
        {
            Debug.LogError(url + "Error downloading texture: " + webRequest.error);
             
            accessToken = accessTokenLast;  
            centerLatitude = centerLatitudeLast;
            centerLongitude = centerLongitudeLast;
            mapStyle = mapStyleLast; 
            
            updateMap = true;
            resetMap = true;  
        }
    }

    private double[] GetRectCoord(double centerLon, double centerLat, double width, double height)
    {
        double diagonal = Math.Sqrt(Math.Pow(height / 2.0, 2) + Math.Pow(width / 2.0, 2));
        double topRightBearing = Math.Atan((width/2.0) / (height/2.0));
        double bottomLeftBearing = 3.14159f + topRightBearing; // bottomLeftBearing = 180 degree(3.14159..radian) + topRightBearing 
        double[] bottomLeft = GetPointLonLat(centerLon, centerLat, diagonal, bottomLeftBearing);  
        double[] topRight = GetPointLonLat(centerLon, centerLat, diagonal, topRightBearing); 
        return new double[] {bottomLeft[0], bottomLeft[1], topRight[0], topRight[1]}; 
    }

    private double[] GetPointLonLat(double startLonDegree, double startLatDegree, double distance, double bearingRadian)
    {
        double earthRadius = 6378100.000; // meter 
        double startLonRadians = startLonDegree * (Math.PI / 180); 
        double startLatRadians = startLatDegree * (Math.PI / 180); 
        double targetLatRadians = Math.Asin(Math.Sin(startLatRadians) * Math.Cos(distance/earthRadius) + Math.Cos(startLatRadians) * Math.Sin(distance/earthRadius) * Math.Cos(bearingRadian)); 
        double targetLonRadians = startLonRadians + Math.Atan2(Math.Sin(bearingRadian) * Math.Sin(distance/earthRadius) * Math.Cos(startLatRadians), Math.Cos(distance/earthRadius)-Math.Sin(startLatRadians)*Math.Sin(targetLatRadians)); 
        return new double[] {targetLonRadians * (180.0 / Math.PI), targetLatRadians * (180.0 / Math.PI)}; 
    }
}

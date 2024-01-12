using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Networking; 
using System;

public class Map : MonoBehaviour
{

    // Inspector attribute 
    public float centerLatitude = 37.8703f; // San francisco Latitude & Longitude 
    public float centerLongitude = -122.3372f;  
    public float zoom = 10.0f; 
    public int bearing = 0; 
    public int pitch = 0; 
    public string accessToken; 
    public enum style { Streets, Outdoors, Light, Dark, Satellite, SatelliteStreets, NavigationDay, NavigationNight };
    public style mapStyle = style.Streets; 

    // Map Box URL & Object & Attribute 
    private string url = ""; 
    private string[] styleStr = new string[] {"streets-v12", "outdoors-v12", "light-v11", "dark-v11", "satellite-v9", "satellite-streets-v12", "navigation-day-v1", "navigation-night-v1"};  
    private Rect rect; 
    private int mapHeight = 800; 
    private int mapWidth = 800; 
    // Status 
    //private bool mapIsLoading = false; 
    private bool updateMap = true; 

    


    //Saving last value
    private float centerLatitudeLast = 37.8703f;
    private float centerLongitudeLast = -122.3372f;
    private float zoomLast = 10.0f;
    private int bearingLast = 0; 
    private int pitchLast = 0;  
    private string accessTokenLast; 
    private style mapStyleLast = style.Streets; 


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetMapBox()); 
        rect = gameObject.GetComponent<RawImage>().rectTransform.rect; 
        mapWidth = (int)Math.Round(rect.width); 
        mapHeight = (int)Math.Round(rect.height); 
        
    }

    // Update is called once per frame
    void Update()
    {
        //checking map attribute is changed or not
        if (updateMap && (accessTokenLast != accessToken || !Mathf.Approximately(centerLatitudeLast, centerLatitude) || Mathf.Approximately(centerLongitudeLast, centerLongitude) || zoomLast != zoom || bearingLast != bearing || pitchLast != pitch || mapStyleLast != mapStyle))
        {
            rect = gameObject.GetComponent<RawImage>().rectTransform.rect; 
            mapWidth = (int)Math.Round(rect.width); 
            mapHeight = (int)Math.Round(rect.height); 
            StartCoroutine(GetMapBox()); 
            updateMap = false; 
        }
    }

    IEnumerator GetMapBox()
    {
        url = "https://api.mapbox.com/styles/v1/mapbox/" + styleStr[(int)mapStyle] + "/static/" + centerLongitude + "," + centerLatitude + "," + zoom + "," + bearing + "," + pitch + "/" + mapWidth + "x" + mapHeight + "?access_token=" + accessToken; 
        //mapIsLoading = true; 
        UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
        yield return webRequest.SendWebRequest(); 
        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            //mapIsLoading = false; 
            Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture; 
            gameObject.GetComponent<RawImage>().texture = texture; 

            centerLatitudeLast = centerLatitude;
            centerLongitudeLast = centerLongitude;
            zoomLast = zoom;
            bearingLast = bearing; 
            pitchLast = pitch;   
            accessTokenLast = accessToken; 
            mapStyleLast = mapStyle; 
            updateMap = true; 
        }
        else
        {
            Debug.LogError(url + "Error downloading texture: " + webRequest.error); 
        }
    }
}

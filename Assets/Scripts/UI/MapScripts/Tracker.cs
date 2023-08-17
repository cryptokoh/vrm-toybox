using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    public GameObject player;
    public Camera mainCamera;

    public RectTransform mapCircle;
    public float mapRadius = 100f;
    public GameObject mapUI; // The UI GameObject that is the parent of the dots

    private MapObjectPool poolManager; // Reference to the PoolManager
    private GameObject dot; // The dot for this tracker

    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mapCircle = GameObject.Find("Map").GetComponent<RectTransform>();
        poolManager = GameObject.Find("_MapPooler").GetComponent<MapObjectPool>(); // Assume the PoolManager is attached to a GameObject named "PoolManager"
        mapUI = GameObject.FindGameObjectWithTag("EnemiesOnMap");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        // Get a dot from the pool and set it up
        dot = poolManager.GetDot();
        dot.transform.SetParent(mapUI.transform);
        dot.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    void Update()
    {
        Vector3 directionToTracker = transform.position - player.transform.position;
        directionToTracker.y = 0;

        // Rotate the direction vector by the camera's rotation
        directionToTracker = Quaternion.Euler(0, -mainCamera.transform.eulerAngles.y, 0) * directionToTracker;

        // Map the tracker's distance to the player to the mapRadius
        float distanceToTracker = directionToTracker.magnitude;
        float mappedDistance = Mathf.Min(distanceToTracker, mapRadius); // Clamp at mapRadius

        // Create the position on the radar
        Vector2 mapPos = new Vector2(directionToTracker.x, directionToTracker.z).normalized * (mappedDistance / mapRadius) * mapRadius;

        dot.GetComponent<RectTransform>().anchoredPosition = mapPos;
    }

    void OnDisable()
    {
        // When the tracker is disabled/destroyed, return the dot to the pool
        // Check if dot still exists before trying to return it
        if (dot != null)
        {
            poolManager.ReturnDot(dot);
        }
    }
}

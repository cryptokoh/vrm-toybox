using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    public GameObject platformPrefab;

    private PlayerController playerController;  // The player controller that should subscribe to platform events

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public void AddPlatform(Vector3 position)
    {
        GameObject newPlatform = Instantiate(platformPrefab, position, Quaternion.identity);
        MovingWalkable movingWalkable = newPlatform.GetComponent<MovingWalkable>();
        
        if (movingWalkable != null)
        {
            movingWalkable.OnEntityEnter += playerController.HandleEntityEnter;
            movingWalkable.OnEntityExit += playerController.HandleEntityExit;
        }
    }
}

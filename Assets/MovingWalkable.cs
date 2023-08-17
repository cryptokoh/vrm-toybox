using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



public class MovingWalkable : MonoBehaviour
{
    public delegate void EntityEnter(GameObject entity);
    public delegate void EntityExit(GameObject entity);

    public event EntityEnter OnEntityEnter;
    public event EntityExit OnEntityExit;

    private Vector3 previousPosition;

private List<GameObject> entitiesOnPlatform = new List<GameObject>();

public void Update()
{
    Vector3 deltaMovement = transform.position - previousPosition;

    foreach (GameObject entity in entitiesOnPlatform)
    {
        entity.transform.position += deltaMovement;
    }

    previousPosition = transform.position;
}

private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        entitiesOnPlatform.Add(collision.gameObject);
    }
}

private void OnCollisionExit(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        entitiesOnPlatform.Remove(collision.gameObject);
    }
}

}

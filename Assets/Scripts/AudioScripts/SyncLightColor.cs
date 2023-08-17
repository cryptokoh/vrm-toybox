using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncLightColor : MonoBehaviour
{
    public EmissionFade emissionFade; // Reference to the EmissionFade script
    private Light lightComponent; // Reference to the Light component

    void Start()
    {
        // Get the Light component attached to the same GameObject
        lightComponent = GetComponent<Light>();

        // Check if the EmissionFade script is assigned
        if (emissionFade == null)
        {
            Debug.LogError("EmissionFade script reference is not assigned!");
        }
    }

    void Update()
    {
        // Check if the EmissionFade script is assigned
        if (emissionFade != null && emissionFade.material != null)
        {
            // Get the current emission color from the material
            Color currentColor = emissionFade.material.GetColor("_EmissionColor");

            // Update the light color with the current color
            lightComponent.color = currentColor;
        }
    }
}

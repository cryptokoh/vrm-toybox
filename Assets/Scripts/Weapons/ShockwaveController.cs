using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class ShockwaveController : MonoBehaviour
{
    public GameObject player;
    public GameObject hitPrefab; // the prefab to instantiate at each point of impact
    public float distance = 10f;
    public float power = 10f;
    public float damage = 10f;
    public ParticleSystem shockwaveParticles;
    public float drag = 0.5f;
    public string[] hitTags;
    public float shockwaveDuration = 2f; // how long the shockwave lasts
    public bool debugGameGizmos = true;
    public bool visualizeOverlapSphere = false;
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>(); // keeps track of objects hit by the shockwave
    private Vector3 lastSpherePosition = Vector3.zero;
    private float lastSphereRadius = 0f;
    private FloatingText floatingTextRef;
    public float chargeTime = 1f; // time it takes to fully charge
    private float charge = 0f; // the current charge
    private bool isCharging = false; // if the ability is currently charging
    private Vector3 chargeDirection = Vector3.zero; // the direction of the charge
    public float maxCharge = 100f; // the maximum charge
    public float chargeDistanceMultiplier = 1f; // how the charge affects the distance of the shockwave
    public float chargePowerMultiplier = 1f; // how the charge affects the power of the shockwave
    public float chargeDamageMultiplier = 1f; // how the charge affects the damage of the shockwave
    public Slider chargeSlider;
    private float currentCharge = 0f;
    private float totalDamage = 0f;

    public AudioSource chargeSound; // Assign this in the Inspector
    private bool isPlayingChargeSound = false;

    public AudioClip lowChargeSound; // Assign these in the Inspector
    public AudioClip midChargeSound;
    public AudioClip maxChargeSound;

    public AudioSource lowChargeAudioSource;
    public AudioSource midChargeAudioSource;
    public AudioSource maxChargeAudioSource;


    private AudioSource currentChargeAudioSource; // This will be used to play the current charge sound




    private void Awake()
    {
        if(shockwaveParticles == null)
        {
            shockwaveParticles = GetComponent<ParticleSystem>();
        }
    }

    void Update()
    {
        if (isCharging)
        {
            // Increase charge over time
            charge += Time.deltaTime / chargeTime;
            // Clamp charge to 1
            charge = Mathf.Min(charge, 1f);
            // Update charge direction based on the main camera's forward direction
            chargeDirection = Camera.main.transform.forward;

            if (debugGameGizmos)
            {
                // Calculate the adjusted distance based on the charge
                float adjustedDistance = distance + (charge * maxCharge * chargeDistanceMultiplier);
                // Calculate the end position of the line
                Vector3 endPos = player.transform.position + chargeDirection * adjustedDistance;

                // Draw the line from player position to the end position
                Debug.DrawLine(player.transform.position, endPos, Color.blue);
            }

            // Set the slider value
            //chargeSlider.value = charge * 100f;

            // Calculate the color
            if (charge < 0.5f)
            {
                // If charge is less than 50%, interpolate between yellow and orange
                chargeSlider.fillRect.GetComponent<Image>().color = Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), charge * 2);
            }
            else if (charge < 1f)
            {
                // If charge is more than 50% but less than 100%, interpolate between orange and red
                chargeSlider.fillRect.GetComponent<Image>().color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, (charge - 0.5f) * 2);
            }
            else
            {
                // If charge is full, make it blink by alternating between red and another color (e.g. white)
                chargeSlider.fillRect.GetComponent<Image>().color = Mathf.Sin(Time.time * 100) > 0 ? Color.red : new Color(1f, 0.5f, 0f);
            }
            
            // Start playing the charge sound if it's not playing
            if (!isPlayingChargeSound && chargeSound != null)
            {
                chargeSound.Play();
                isPlayingChargeSound = true;
            }
        }
        else
        {
            // Stop playing the charge sound if it's playing
            if (isPlayingChargeSound && chargeSound != null)
            {
                chargeSound.Stop();
                isPlayingChargeSound = false;
            }
            
        }

        chargeSlider.value = charge * 100f; // Convert charge to a scale of 1 to 100 for the slider

    }

    public void StartCharging()
    {
        if (!isCharging)
        {
            isCharging = true;
            charge = 0f;
            chargeDirection = Vector3.zero;
        }
    }

    public void ShootShockwave()
{

    totalDamage = 0f;

    if (!isCharging) 
    {
        if (charge == 0f) // Check if there is no charge
        {
            if (shockwaveParticles != null)
            {
                shockwaveParticles.Play();
            }

            hitObjects.Clear();
            // Trigger a shockwave with default power and distance.
            StartCoroutine(ExpandShockwave(power, distance));
        }
        else 
        {
            // If there is a charge, trigger a shockwave and then start a new charging process.
            StartCoroutine(ExpandShockwave(power, distance)); // Add this line
            
            isCharging = true;
            charge = 0f;
            chargeDirection = Vector3.zero;
        }
    }
    else 
    {
        // Stop charging and trigger a charged shockwave.
        isCharging = false;
        chargeDirection = player.transform.forward; // get the player's forward direction when the shockwave is triggered

        if (shockwaveParticles != null)
        {
            shockwaveParticles.Play();
        }

        hitObjects.Clear();

        // Adjust power and distance based on charge.
        float adjustedPower = power + (charge * maxCharge * chargePowerMultiplier);
        float adjustedDistance = distance + (charge * maxCharge * chargeDistanceMultiplier);

        // Save the current charge
        currentCharge = charge;

        StartCoroutine(ExpandShockwave(adjustedPower, adjustedDistance));

        // Stop the current sound
        /* if (currentChargeAudioSource != null)
        {
            currentChargeAudioSource.Stop();
        } */

        // Play the appropriate sound based on charge level
        if (charge < 0.33f && lowChargeSound != null)
        {
            lowChargeAudioSource.clip = lowChargeSound;
            lowChargeAudioSource.PlayOneShot(lowChargeSound);
        }
        else if (charge < 0.66f && midChargeSound != null)
        {
            midChargeAudioSource.clip = midChargeSound;
            midChargeAudioSource.PlayOneShot(midChargeSound);
        }
        else if (maxChargeSound != null)
        {
            maxChargeAudioSource.clip = maxChargeSound;
            maxChargeAudioSource.PlayOneShot(maxChargeSound);
        }

        // Reset the charge
        charge = 0f;

    }

    
}

    private IEnumerator ExpandShockwave(float power, float distance)
    {
        float startTime = Time.time;
        float initialDistance = distance;

        while (Time.time - startTime < shockwaveDuration)
        {
            float progress = (Time.time - startTime) / shockwaveDuration; // goes from 0 to 1 over the shockwave duration
            float currentRadius = progress * distance; // using the local distance variable here
            lastSpherePosition = player.transform.position; // update the last sphere position
            lastSphereRadius = currentRadius; // update the last sphere radius
            Collider[] hits = Physics.OverlapSphere(player.transform.position, currentRadius);
            foreach (Collider hit in hits)
            {
                ApplyShockwaveEffect(hit, power); // passing power to the function 
                
            }

            yield return null; // wait until the next frame
        }

       

        if(totalDamage != 0){
             SendMessageToChat("Total Damage: " + totalDamage.ToString(), Message.MessageType.totalDamage);
            SendMessageToHitUI(totalDamage.ToString(), HitMessage.MessageType.hitMessage);
        }
        
    }



    public bool enableVisualHit = true; // add this line to your class fields

    private void ApplyShockwaveEffect(Collider collider, float power)
    {

       string dmgText = ""; // Set a default value for dmgText
       

        if (System.Array.IndexOf(hitTags, collider.gameObject.tag) >= 0 && !hitObjects.Contains(collider.gameObject))
        {
            hitObjects.Add(collider.gameObject);

            float distanceToPlayer = Vector3.Distance(player.transform.position, collider.transform.position);
            float distanceFactor = 1f / Mathf.Pow(distanceToPlayer, 2);

            distanceFactor = Mathf.Clamp(distanceFactor, .1f, 1f);

            float randomFactor = Random.Range(.5f, 1.5f);
            float scaledDamage = damage * randomFactor * distanceFactor;
            // Apply charge damage multiplier to scaledDamage
            //Debug.Log(currentCharge);
            float adjustedDamage = scaledDamage * (1f + (currentCharge * chargeDamageMultiplier));
            int truncatedDamage = (int)adjustedDamage;
            totalDamage = totalDamage + truncatedDamage;
            Color color = GetAmountColor(truncatedDamage);
            FloatingText.TriggerFloatingTextWithColor(collider.gameObject.transform.position, truncatedDamage, color);
            
            float adjustedPower = power + (currentCharge * chargePowerMultiplier);

            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 hitDirection = (collider.transform.position - Camera.main.transform.position).normalized;
                rb.AddForce(hitDirection * adjustedPower, ForceMode.Impulse);
                rb.drag = drag;

                if (debugGameGizmos)
                {
                    //Debug.DrawRay(Camera.main.transform.position, hitDirection * distance, Color.red, 2f);
                    //Debug.DrawRay(Camera.main.transform.position, -hitDirection * distance, Color.red, 2f);
                }

                HealthBar healthBar = collider.GetComponent<HealthBar>();
                if (healthBar != null)
                {
                    healthBar.ApplyDamage((int)adjustedDamage);
                }
                
                
            }

            if (enableVisualHit)
            {
                GameObject hitInstance = Instantiate(hitPrefab, collider.transform.position, Quaternion.identity);
                Destroy(hitInstance, 2f);
            }

            

            
        }

        
    }

    private void OnDrawGizmos()
    {
        if (visualizeOverlapSphere)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lastSpherePosition, lastSphereRadius);
        }
    }

    public Color GetAmountColor(int amount)
    {
        // Convert amount into a percentage of the maximum possible damage.
        float percentage = Mathf.Clamp((float)amount / 50f, 0f, 1f);
        // Use the percentage to interpolate between orange and red.
        return Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, percentage);
    }

    // Example function to send a player message from this script
    public void SendMessageToChat(string message, Message.MessageType messageType)
    {
        ChatManager.instance.SendMessageToChat(message, messageType);
    }

    public void SendMessageToHitUI(string message, HitMessage.MessageType messageType)
    {
        HitManagerUI.instance.SendMessageToChat(message, messageType);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Pickupable : MonoBehaviour
{
    // Declare the event
    public static event System.Action OnCoinPickedUp;
    public static event System.Action OnHealthPickedUp;
    public static event System.Action OnGasCanPickedUp;
    
    public enum PickupType
    {
        Kickdrum,
        Highhat,
        Basedrum,
        Health,
        Ammo,
        Powerup,
        GasCan,
        Coin
        // Add any other pickup types here.
    }

    //public AudioClip pickupSound;
    public PickupType pickupType;
    public PlayerHealthBar playerHealthBar;
    private FloatingText floatingTextRef;
    //private Scorekeeping scorekeepingRef;
    //public TextMeshProUGUI moneyUiText;
    //public GameObject CoinSoundPlayerObject;
    private CoinSoundPlayer soundPlayer;
    private PickUpPlayer pickupPlayer;

    void OnEnable(){
        playerHealthBar = FindObjectOfType<PlayerHealthBar>();
        soundPlayer = FindObjectOfType<CoinSoundPlayer>();
        pickupPlayer = FindObjectOfType<PickUpPlayer>();
        //scorekeepingRef = FindObjectOfType<Scorekeeping>();
    }

    private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        // Here, you can use switch statement to differentiate actions based on pickupType
        switch (pickupType)
        {
            case PickupType.Kickdrum:
                Debug.Log("Kickdrum picked up");
                break;
            case PickupType.Highhat:
                Debug.Log("Highhat picked up");
                break;
            case PickupType.Basedrum:
                Debug.Log("Basedrum picked up");
                break;
            case PickupType.Health:
                Debug.Log("Health picked up");
                OnHealthPickedUp?.Invoke();
                FloatingText.TriggerFloatingTextWithColor(transform.position, 100, Color.green);
                playerHealthBar.currentHealth = 100;

                //play sound
                pickupPlayer.PlayRandomSound();
                break;            
            case PickupType.Ammo:
                Debug.Log("Ammo picked up");
                break;
            case PickupType.Powerup:
                Debug.Log("Powerup picked up");
                break;
            case PickupType.GasCan:
                Debug.Log("GasCan picked up");
                OnGasCanPickedUp?.Invoke();
                FloatingText.TriggerFloatingTextWithColor(transform.position, 1, Color.green);
                pickupPlayer.PlayRandomSound();
                break;    
            case PickupType.Coin:

                // Invoke the event
                OnCoinPickedUp?.Invoke();
                FloatingText.TriggerFloatingTextWithColor(transform.position, 100, Color.green);

                //play random coin sound
                soundPlayer.PlayRandomSound();

            //scorekeepingRef.money += 1;
            //moneyUiText.text = scorekeepingRef.money.ToString();
            break;
        }

        // Play the pickup sound
        //AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        // Check for errors with the PickupPool instance or GameObject
        if (PickupPool.Instance == null)
        {
            Debug.LogError("PickupPool.Instance is null");
        }

        if (gameObject == null)
        {
            Debug.LogError("gameObject is null");
        }

        // If neither of the above log messages show up in the console, the issue could be in the ReturnToPool method
        PickupPool.Instance.ReturnToPool(gameObject);
    }
}

}

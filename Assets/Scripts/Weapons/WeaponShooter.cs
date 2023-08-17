using UnityEngine;
using System;
using System.Collections;
using TMPro;

public class WeaponShooter : MonoBehaviour
{
    public delegate void ShootWeaponEventHandler(int weaponIndex);
    public static event ShootWeaponEventHandler OnShootWeapon;

    public WeaponController weaponController;
    public ShockwaveController shockwaveController;
    public RayCastController raycastController;
    public RapidFireController rapidFireController;

    public Transform headTransform;
    public float recoilAngle = 10f; 
    public float recoveryRate = 1f; // Rate at which the head returns to its original rotation
    private float currentRecoilAngle; // Current amount of recoil applied

    public TextMeshProUGUI ammoText;

    public AudioSource audioSource;
    public AudioClip shootSound;

    public float fireRate = 0.1f;
    private float nextFireTime = 0;

    public int maxAmmo = 100;
    private int currentAmmo;

    private void Awake(){
        currentAmmo = maxAmmo;
    }
    
    private void Start()
    {
        GameObject playerObject = GameObject.Find("PlayerObject");
        GameObject headObject = FindChildByNamePart(playerObject, "head");

        if (headObject != null)
        {
            headTransform = headObject.transform;
        }
        else
        {
            Debug.LogError("Head object not found!");
        }
        
        OnShootWeapon += HandleShootWeapon;
    }

    private void OnDisable()
    {
        OnShootWeapon -= HandleShootWeapon;
    }

    private void HandleShootWeapon(int weaponIndex)
    {
        if (weaponIndex == 0)
        {
            ShootWeapon1();
        }

        if (weaponIndex == 1)
        {
            ShootWeapon2();
        }

        if (weaponIndex == 2)
        {
            ShootWeapon3();
        }
    }

    private void ShootWeapon1()
    {
        shockwaveController.ShootShockwave(); 
        raycastController.ShootRay();
        ApplyRecoil();
    }

    private void ShootWeapon2()
    {
        nextFireTime = Time.time + fireRate;
        //rapidFireController.ShootRay();
        rapidFireController.ShootProjectile();
        currentAmmo--;
        ApplyRecoil();
        audioSource.PlayOneShot(shootSound); 
    }

    private void ShootWeapon3()
    {
        raycastController.ShootRay();
        ApplyRecoil();
    }

    private void LateUpdate()
    {
        ammoText.text = currentAmmo.ToString();

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentAmmo = maxAmmo;
        }
        
        if (weaponController.GetCurrentWeaponIndex() == 0)
        {
            if (Input.GetMouseButtonDown(0)) 
            {
                StartChargingWeapon();
            }
            if (Input.GetMouseButtonUp(0)) 
            {
                ShootCurrentWeapon();
            }
        }
        
        if (weaponController.GetCurrentWeaponIndex() == 1)
        {
            if (Input.GetMouseButton(0) && Time.time > nextFireTime && currentAmmo > 0) 
            {
                ShootCurrentWeapon();
            }
        }

        // Recover from recoil over time
        if (currentRecoilAngle > 0)
        {
            float recoveryAmount = recoveryRate * Time.deltaTime; // Amount to recover this frame
            float actualRecoveryAmount = Mathf.Min(recoveryAmount, currentRecoilAngle); // Don't recover more than we have recoiled

            currentRecoilAngle -= actualRecoveryAmount;
            headTransform.rotation *= Quaternion.Euler(-actualRecoveryAmount, 0, 0);
        }
    }

    private void StartChargingWeapon()
    {
        if(weaponController.GetCurrentWeaponIndex() == 0) 
        {
            shockwaveController.StartCharging();
        }
    }

    private void ShootCurrentWeapon()
    {
        int currentWeaponIndex = weaponController.GetCurrentWeaponIndex();
        
        OnShootWeapon?.Invoke(currentWeaponIndex);
    }

    private void ApplyRecoil()
    {
        currentRecoilAngle += recoilAngle;
    }

    private GameObject FindChildByNamePart(GameObject parent, string namePart)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name.ToLower().Contains(namePart.ToLower()))
            {
                return child.gameObject;
            }

            GameObject result = FindChildByNamePart(child.gameObject, namePart);

            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}

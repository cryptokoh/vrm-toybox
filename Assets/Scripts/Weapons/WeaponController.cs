using UnityEngine;
using TMPro;

public class WeaponController : MonoBehaviour
{
    public GameObject[] weapons; // Array of weapon game objects
    public TextMeshProUGUI weaponTextLabel;
    private int currentWeaponIndex = 0; // Index of the current selected weapon

    // Delegate for weapon change event
    public delegate void WeaponChangedEventHandler(int index);
    // Event triggered when the weapon changes
    public event WeaponChangedEventHandler OnWeaponChanged;

    private void Start()
    {
        SelectWeapon(currentWeaponIndex); // Select the initial weapon
    }

    private void Update()
    {
        // Handle weapon switching using events
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToWeapon(0); // Switch to weapon at index 0
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToWeapon(1); // Switch to weapon at index 1
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchToWeapon(2); // Switch to weapon at index 2
        }
    }

    private void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
        {
            Debug.LogWarning("Invalid weapon index.");
            return;
        }

        // Disable all weapons
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }

        // Enable the selected weapon
        weapons[index].SetActive(true);

        // Update the current weapon index
        currentWeaponIndex = index;

        // Update the weapon label
        UpdateWeaponLabel();

        // Trigger the weapon change event
        OnWeaponChanged?.Invoke(currentWeaponIndex);

        Debug.Log("Selected weapon: " + weapons[index].name);
    }

    private void UpdateWeaponLabel()
    {
        if (weaponTextLabel != null)
        {
            // Set the label to the current weapon index as a string
            weaponTextLabel.text = (currentWeaponIndex + 1).ToString();
        }
    }

    public void SwitchToWeapon(int index)
    {
        if (index != currentWeaponIndex)
        {
            SelectWeapon(index);
        }
    }

    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }
}

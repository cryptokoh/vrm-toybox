using UnityEngine;
using TMPro;

public class Scorekeeping : MonoBehaviour
{
    public int score = 0;
    public int money = 0;
    public TextMeshProUGUI moneyUiText;

    private void OnEnable()
    {
        // Subscribe to the event
        Pickupable.OnCoinPickedUp += AddMoney;
    }

    private void OnDisable()
    {
        // Always unsubscribe from events when the object is disabled to prevent memory leaks
        Pickupable.OnCoinPickedUp -= AddMoney;
    }

    private void AddMoney()
    {
        money += 100;
        moneyUiText.text = money.ToString();
    }
}

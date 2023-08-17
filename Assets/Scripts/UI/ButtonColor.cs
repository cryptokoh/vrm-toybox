using UnityEngine;
using UnityEngine.UI;

public class ButtonColor : MonoBehaviour
{
    public Button myButton;  // Assign this in the inspector to your new button

    private bool isSkillOn;

    private void Start()
    {
        // Initialize the skill to be off
        isSkillOn = false;

        SetColor();
        myButton.onClick.AddListener(ToggleSkill);
    }

    // This method will be called when the button is clicked
    public void ToggleSkill()
    {
        // Toggle the skill state
        isSkillOn = !isSkillOn;

        // Set the button color based on the new state
        SetColor();
    }

    private void SetColor()
    {
        // Create an orange color with full opacity for skill off
        Color offColor = new Color(1f, 0.5f, 0f, 1f);

        // Create a blue color with full opacity for skill on
        Color onColor = new Color(0f, 0f, 1f, 1f);

        // Create a new ColorBlock for your button
        ColorBlock colorBlock = myButton.colors;
        colorBlock.normalColor = isSkillOn ? onColor : offColor;
        colorBlock.highlightedColor = isSkillOn ? onColor : offColor;
        colorBlock.pressedColor = isSkillOn ? onColor : offColor;

        // Assign the new ColorBlock to your button
        myButton.colors = colorBlock;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ToggleColor : MonoBehaviour
{
    public Toggle myToggle;

    private void Start()
    {
        SetColor();
        myToggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void OnToggleValueChanged(bool isOn)
    {
        SetColor();
    }

    private void SetColor()
    {
        Color offColor = new Color(1f, 0.5f, 0f, 1f); // Orange
        Color onColor = new Color(0f, 0f, 1f, 1f); // Blue

        ColorBlock colorBlock = myToggle.colors;
        colorBlock.normalColor = myToggle.isOn ? onColor : offColor;
        colorBlock.highlightedColor = myToggle.isOn ? onColor : offColor;
        colorBlock.pressedColor = myToggle.isOn ? onColor : offColor;

        myToggle.colors = colorBlock;
    }
}

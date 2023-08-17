using UnityEngine;
using UnityEngine.UI;

public class SimpleButton : MonoBehaviour
{
    public SkillTree skillTree;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        UpdateButtonColor();
        button.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        skillTree.ToggleSkill();
        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        Color color = skillTree.isSkillEnabled ? skillTree.enabledColor : skillTree.disabledColor;
        button.image.color = color;
    }
}

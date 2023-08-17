using UnityEngine;
using UnityEngine.UI;

public class SkillTree : MonoBehaviour
{
    public Color enabledColor;
    public Color disabledColor;
    public bool isSkillEnabled;

    public void ToggleSkill()
    {
        isSkillEnabled = !isSkillEnabled;
    }
}

public class SkillUI : MonoBehaviour
{
    public SkillTree skillTree;
    public Button skillButton;

    private void Start()
    {
        UpdateButtonColor();
        skillButton.onClick.AddListener(OnButtonClick);
    }

    public void OnButtonClick()
    {
        skillTree.ToggleSkill();
        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        Color color = skillTree.isSkillEnabled ? skillTree.enabledColor : skillTree.disabledColor;
        skillButton.image.color = color;
    }
}

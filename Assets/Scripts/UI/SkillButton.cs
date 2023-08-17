using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public SkillManager skillManager;
    public string skillName;
    public Color enabledColor = Color.green;
    public Color disabledColor = Color.red;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);
        skillManager.OnSkillToggled += SkillToggled;  // Subscribe to the skill toggled event
        UpdateButtonColor();
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnButtonClick);
        skillManager.OnSkillToggled -= SkillToggled;  // Unsubscribe from the skill toggled event
    }

    private void OnButtonClick()
    {
        skillManager.ToggleSkill(skillName);
    }

    private void SkillToggled(Skill skill)
    {
        if (skill.name == skillName)
        {
            UpdateButtonColor();
        }
    }

    private void UpdateButtonColor()
    {
        ColorBlock colorBlock = button.colors;
        var skill = skillManager.skills.Find(s => s.name == skillName);
        if (skill != null)
        {
            var stateColor = skill.isEnabled ? enabledColor : disabledColor;
            colorBlock.normalColor = stateColor;
            colorBlock.highlightedColor = stateColor;
            colorBlock.pressedColor = stateColor;
            colorBlock.selectedColor = stateColor;
            colorBlock.disabledColor = stateColor;
        }
        button.colors = colorBlock;
    }
}

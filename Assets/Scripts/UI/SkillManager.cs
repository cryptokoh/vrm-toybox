

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Skill
{
    public string name;
    public bool isEnabled;
}

public class SkillManager : MonoBehaviour
{
    public List<Skill> skills = new List<Skill>();

    public event Action<Skill> OnSkillToggled;

    private void Start()
    {
        var skillButtons = FindObjectsOfType<SkillButton>();
        foreach (var skillButton in skillButtons)
        {
            var button = skillButton.GetComponent<Button>();
            //button.onClick.AddListener(() => OnButtonClick(skillButton.skillName));
            UpdateButtonColor(button, GetSkill(skillButton.skillName)?.isEnabled ?? false);
        }
    }

    /* private void OnButtonClick(string skillName)
    {
        var skill = GetSkill(skillName);
        if (skill != null)
        {
            skill.isEnabled = !skill.isEnabled;
            Debug.Log($"Toggled skill {skillName} to {skill.isEnabled}");
            OnSkillToggled?.Invoke(skill);
            var button = FindSkillButton(skillName)?.GetComponent<Button>();
            if (button != null)
            {
                UpdateButtonColor(button, skill.isEnabled);
            }
        }
        else
        {
            Debug.Log($"No skill named {skillName} found.");
        }
    }
 */
    private void Update()
    {
        foreach (var skill in skills)
        {
            var skillButton = FindSkillButton(skill.name);
            if (skillButton != null)
            {
                var button = skillButton.GetComponent<Button>();
                UpdateButtonColor(button, skill.isEnabled);
            }
        }
    }

    public void ToggleSkill(string skillName)
    {
        var skill = skills.Find(s => s.name == skillName);
        if (skill != null)
        {
            skill.isEnabled = !skill.isEnabled;
            Debug.Log($"Toggled skill {skillName} to {skill.isEnabled}");
            OnSkillToggled?.Invoke(skill);
        }
        else
        {
            Debug.Log($"No skill named {skillName} found.");
        }
    }

    private SkillButton FindSkillButton(string skillName)
    {
        var skillButtons = FindObjectsOfType<SkillButton>();
        return Array.Find(skillButtons, button => button.skillName == skillName);
    }

    private Skill GetSkill(string skillName)
    {
        return skills.Find(skill => skill.name == skillName);
    }

    private void UpdateButtonColor(Button button, bool isEnabled)
    {
        ColorBlock colorBlock = button.colors;
        var stateColor = isEnabled ? Color.green : Color.red;
        colorBlock.normalColor = stateColor;
        colorBlock.highlightedColor = stateColor;
        colorBlock.pressedColor = stateColor;
        colorBlock.selectedColor = stateColor;
        colorBlock.disabledColor = stateColor;
        button.colors = colorBlock;
    }
}

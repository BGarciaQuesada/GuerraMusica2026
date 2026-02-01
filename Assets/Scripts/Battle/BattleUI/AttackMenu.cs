using System;
using UnityEngine;

public class AttackMenu : MonoBehaviour
{
    public SO_Skill[] skills;

    public event Action<SO_Skill> OnSkillSelected;

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);

    public void SelectSkill(int index)
    {
        OnSkillSelected?.Invoke(skills[index]);
        Close();
    }
}

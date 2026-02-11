using System;
using UnityEngine;
// No confundirse, esta clase engloba todo lo que es UTILIZAR SKILLS DEL COMPAÑERO

public class PartnerAttackMenu : MonoBehaviour
{
    SOSkill[] currentSkills;

    public event Action<SOSkill> OnPartnerSkillSelected;
    public event Action OnBack;

    public void Open(SOSkill[] skills)
    {
        currentSkills = skills;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    // Botones llaman a esto
    public void SelectSkill(int index)
    {
        OnPartnerSkillSelected?.Invoke(currentSkills[index]);
        Close();
    }

    public void Back()
    {
        Close();
        OnBack?.Invoke();
    }
}

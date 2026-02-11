using UnityEngine;

public class PartnerCombat : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private string partnerName;

    [Header("Skills del compañero")]
    [SerializeField] private SOSkill[] skills;

    // BattleController pedirá esto para mostrar en el menú
    public SOSkill[] GetSkills()
    {
        return skills;
    }
}

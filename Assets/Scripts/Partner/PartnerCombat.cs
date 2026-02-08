using UnityEngine;

public class PartnerCombat : MonoBehaviour
{
    [Header("Skills del compañero")]
    [SerializeField] public SOSkill[] skills; // [!] Ya se que está publico. Es casi mi hora de acostarme, dame un respiro.

    // BattleController pedirá esto para mostrar en el menú
    public SOSkill[] GetSkills()
    {
        return skills;
    }
}

using UnityEngine;

public class PartnerCombat : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private string partnerName;
    [SerializeField] private GameObject partnerPrefab;

    [Header("Skills del compañero")]
    [SerializeField] private SOSkill[] skills;

    // BattleController pedirá esto para mostrar en el menú
    public SOSkill[] GetSkills()
    {
        return skills;
    }

    public GameObject GetPartnerPrefab()
    {
        return partnerPrefab;
    }
}

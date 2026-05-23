using UnityEngine;

public class PartnerCombat : MonoBehaviour
{
    [Header("Info")]
    public string partnerName;
    [SerializeField] private GameObject partnerPrefab;

    [Header("Skills del compa�ero")]
    public SOSkill[] skills;

    // BattleController pedir� esto para mostrar en el men�
    public SOSkill[] GetSkills()
    {
        return skills;
    }

    public GameObject GetPartnerPrefab()
    {
        return partnerPrefab;
    }
}

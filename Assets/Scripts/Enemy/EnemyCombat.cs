using UnityEngine;

// Esta clase tendrá las stats del enemigo. Ahora mismo solo hay daño. El plan es que aquí haya métodos según el tipo de enemigo y que los llame.
public class EnemyCombat : MonoBehaviour
{
    [Header("Skills del enemigo")]
    [SerializeField] private SOSkill[] skills;
    [SerializeField] private int[] skillProbability;

    [Header("Skill especial cada X turnos")]
    [SerializeField] private SOSkill specialSkill;
    [SerializeField] private int specialEveryTurns = 0; // 0 = desactivado, usado para jefes

    private int turnCounter = 0;

    
    public SOSkill GetNextSkill()
    {
        // Aumentar turno para el especial
        turnCounter++;

        // SI HA LLEGADO EL TURNO, forzar skill especial
        // Si no tiene skill special, la primera condición lo echa
        if (specialEveryTurns > 0 && turnCounter % specialEveryTurns == 0)
        {
            return specialSkill;
        }

        // Ataque random normal
        return GetSkillByProbability();
    }

    // Elegir skill por probabilidad (75% / 25% etc)
    SOSkill GetSkillByProbability()
    {
        if (skills.Length == 0)
            return null;

        // Calcula el porcentaje total sumando todas las probabilidades de sus skills
        int totalProbability = 0;
        foreach (int probabilityValue in skillProbability)
            totalProbability += probabilityValue;

        // Saca número dentro del rango
        int roll = Random.Range(0, totalProbability);

        // Recorrer skills hasta llegar al n�mero rolleado
        int accumulatedProbability = 0;

        for (int i = 0; i < skills.Length; i++)
        {
            accumulatedProbability += skillProbability[i];
            if (roll < accumulatedProbability) // Encontrada, mandar de vuelta
                return skills[i];
        }

        // No se ha encontrado, usar 1º (no se debería llegar aquí de normales)
        return skills[0];
    }
}
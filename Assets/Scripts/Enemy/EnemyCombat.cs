using UnityEngine;

// Esta clase tendrá las stats del enemigo. Ahora mismo solo hay daño. El plan es que aquí haya métodos según el tipo de enemigo y que los llame.
public class EnemyCombat : MonoBehaviour
{
    [SerializeField] int attackDamage = 8;

    public int GetAttackDamage()
    {
        return attackDamage;
    }
}

using UnityEngine;
using System;
// [!] PlayerHealth y EnemyHealth se parecen demasiado... Espero que esto no signifique que se pueden unificar mejor... Por ahora se queda así

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 30;
    [SerializeField] private int currentHP;
    private bool pendingDeath;

    [SerializeField] private int stunTurnsRemaining = 0; // Turnos que se queda KO

    // [!] Método flecha no explotes otra vez y entres en bucle grax
    public bool IsDead => pendingDeath;

    public event Action OnEnemyDeath;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        if (pendingDeath) return;

        currentHP -= dmg;

        if (currentHP <= 0)
        {
            currentHP = 0;
            pendingDeath = true; // PONEMOS LA VANDERA DE QUE SE MUERA PORQUE SI SE DESTRUYE AQUÍ TE SACA AL MUNDO CON EL MINIJUEGO
            Debug.Log("Enemigo derrotado (pendiente de finalizar turno)");

            OnEnemyDeath?.Invoke();
        }
    }

    // ===== STUN ======
    public void ApplyStun(int turns)
    {
        stunTurnsRemaining += turns;
    }

    public bool ShouldSkipTurn()
    {
        if (stunTurnsRemaining <= 0)
            return false;

        stunTurnsRemaining--;
        return true;
    }
}

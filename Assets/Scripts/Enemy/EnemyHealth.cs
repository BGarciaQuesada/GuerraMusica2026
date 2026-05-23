using UnityEngine;
using System;
// [!] PlayerHealth y EnemyHealth se parecen demasiado... Espero que esto no signifique que se pueden unificar mejor... Por ahora se queda así

public class EnemyHealth : MonoBehaviour
{

    public GameObject auxAssigns;

    [SerializeField] private int maxHP = 30;
    public int currentHP;

    // Getters de salud
    public int MaxHP => maxHP;
    public int CurrentHP => currentHP;

    public int damageModifier = 0;
    public int modifierTurns = 0;
    private bool pendingDeath;

    [SerializeField] private int stunTurnsRemaining = 0; // Turnos que se queda KO

    // [!] Método flecha no explotes otra vez y entres en bucle grax
    public bool IsDead => pendingDeath;

    public event Action OnEnemyDeath;

    void Awake()
    {
        auxAssigns = GameObject.FindWithTag("AuxAssigns");
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        if (pendingDeath) return;

        currentHP -= dmg;

        if (currentHP <= 0)
        {
            currentHP = 0;
            pendingDeath = true; // PONEMOS LA BANDERA DE QUE SE MUERA PORQUE SI SE DESTRUYE AQUÍ TE SACA AL MUNDO CON EL MINIJUEGO
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

    // ====== BUFF/DEBUFF ======
    public void ApplyDamageModifier(int amount, int turns)
    {
        damageModifier = auxAssigns.GetComponent<AuxiliarAssigns>().botónAtaquePartner.GetComponent<SkillsPatch>().totalBuffs;
        modifierTurns = auxAssigns.GetComponent<AuxiliarAssigns>().botónAtaquePartner.GetComponent<SkillsPatch>().totalTurns;
    }

    public int GetModifiedDamage(int baseDamage)
    {
        return baseDamage + damageModifier;
    }

    // Pasar turnos...
    public void TickModifier()
    {
        if (modifierTurns <= 0) return;

        modifierTurns--;

        if (modifierTurns <= 0)
            damageModifier = 0;
    }
}

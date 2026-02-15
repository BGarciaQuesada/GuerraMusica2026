using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 50;
    private int currentHP;

    [SerializeField] private int damageModifier = 0;
    [SerializeField] private int modifierTurns = 0;
    
    [SerializeField] private int stunTurnsRemaining = 0; // Turnos que se queda KO

    public bool IsDead => currentHP <= 0;

    public event Action OnPlayerDeath;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;

        Debug.Log("Player HP: " + currentHP);

        if (currentHP <= 0)
        {
            currentHP = 0;
            OnPlayerDeath?.Invoke();
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
        damageModifier = amount;
        modifierTurns = turns;
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

    // ====== HEAL ======
    public void Heal(int healAmount)
    {
        if (currentHP + healAmount < maxHP)
            currentHP += healAmount;
        else currentHP = maxHP;
    }
}

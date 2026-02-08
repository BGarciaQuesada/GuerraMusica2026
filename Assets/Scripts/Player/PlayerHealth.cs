using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHP = 50;
    [SerializeField] private int currentHP;

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
}

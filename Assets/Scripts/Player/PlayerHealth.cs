using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHP = 50;

    int currentHP;

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
}

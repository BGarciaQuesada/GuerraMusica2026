using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHP = 30;

    int currentHP;
    bool pendingDeath;

    // [!] Método flecha no explotes otra vez y entres en bucle grax
    public bool IsDead => pendingDeath;

    public event Action OnDeath;

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

            OnDeath?.Invoke();
        }
    }
}

using UnityEngine;

// Esto es solo para dibujar la barra. Lo emplean PlayerHealthUI y EnemyHealthUI, que se encargan de darle los valores correctos.

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] EnemyHealth enemyHealth;
    [SerializeField] HealthBarUI healthBar;

    public void Initialize(EnemyHealth health)
    {
        enemyHealth = health;

        healthBar.Initialize(enemyHealth.MaxHP);
    }

    void Update()
    {
        if (enemyHealth == null) return;
        healthBar.SetHealth(enemyHealth.CurrentHP);
    }
}
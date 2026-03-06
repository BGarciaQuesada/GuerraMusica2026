using UnityEngine;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] PlayerHealth playerHealth;
    [SerializeField] HealthBarUI healthBar;

    void Start()
    {
        healthBar.Initialize(playerHealth.MaxHP);
    }

    void Update()
    {
        healthBar.SetHealth(playerHealth.CurrentHP);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Image fillImage;

    int maxHP;

    public void Initialize(int maxHealth)
    {
        maxHP = maxHealth;
        SetHealth(maxHealth);
    }

    public void SetHealth(int currentHealth)
    {
        float ratio = (float)currentHealth / maxHP;
        fillImage.fillAmount = ratio;
    }
}
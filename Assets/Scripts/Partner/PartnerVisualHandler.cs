using UnityEngine;

// Esta clase maneja el instanciar y destruir el prefab del compañero en el plano de batalla
public class PartnerVisualHandler : MonoBehaviour
{
    [SerializeField] private GameObject smokeEffectPrefab;

    private GameObject currentInstance;

    public void SetPartner(PartnerCombat partner)
    {
        if (partner == null) return;

        // Humo al desaparecer
        if (currentInstance != null)
        {
            if (smokeEffectPrefab != null)
                Instantiate(smokeEffectPrefab, transform.position, Quaternion.identity);

            Destroy(currentInstance);
        }

        GameObject prefab = partner.GetPartnerPrefab();

        if (prefab != null)
        {
            currentInstance = Instantiate(prefab, transform.position, transform.rotation);
        }
    }

    public void ClearPartner()
    {
        if (currentInstance != null)
            Destroy(currentInstance);
    }
}

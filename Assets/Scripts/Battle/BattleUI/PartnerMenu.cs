using System;
using UnityEngine;

// Esto es como en el paper mario........
// No confundirse, esta clase engloba todo lo que es CAMBIAR DE COMPAÑERO
public class PartnerMenu : MonoBehaviour
{
    [Header("Compañeros disponibles")]
    [SerializeField] private PartnerCombat[] partners;

    public event Action<PartnerCombat> OnChangePartner;
    public event Action OnBack;

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);

    public void SelectPartner(int index)
    {
        OnChangePartner?.Invoke(partners[index]);
        Close();
    }

    public void BackPressed()
    {
        OnBack?.Invoke();
        Close();
    }
}

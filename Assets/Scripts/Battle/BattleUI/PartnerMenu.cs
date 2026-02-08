using System;
using UnityEngine;

// Esto es como en el paper mario........
// No confundirse, esta clase engloba todo lo que es CAMBIAR DE COMPAÑERO
public class PartnerMenu : MonoBehaviour
{
    public event Action OnChangePartner;
    public event Action OnBack;

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);

    public void SelectPartner(int index)
    {
        OnChangePartner?.Invoke();
        Close();
    }

    public void BackPressed()
    {
        OnBack?.Invoke();
        Close();
    }
}

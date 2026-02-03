using System;
using UnityEngine;

// Esto es como en el paper mario........
public class PartnerMenu : MonoBehaviour
{
    public event Action<int> OnPartnerSelected;
    public event Action OnBack;

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);

    public void SelectPartner(int index)
    {
        OnPartnerSelected?.Invoke(index);
    }

    public void BackPressed()
    {
        OnBack?.Invoke();
    }
}

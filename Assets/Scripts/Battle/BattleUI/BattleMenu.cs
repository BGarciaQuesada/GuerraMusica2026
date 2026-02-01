using System;
using UnityEngine;

// Esta clase engloba las acciones de los botones del menú de acciones durante el turno del jugador
public class BattleMenu : MonoBehaviour
{
    public event Action OnAttack;
    public event Action OnPartner;
    public event Action OnRun;

    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);

    public void AttackPressed() => OnAttack?.Invoke();
    public void PartnerPressed() => OnPartner?.Invoke();
    public void RunPressed() => OnRun?.Invoke();
}

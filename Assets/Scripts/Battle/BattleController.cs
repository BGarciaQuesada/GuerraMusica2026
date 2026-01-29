using UnityEngine;
using UnityEngine.InputSystem;

public class BattleController : MonoBehaviour
{
    public AttackMinigame minigame;
    public PlayerInput playerInput;

    void Start()
    {
        // Conectar eventos del minijuego
        minigame.OnMinigameHit += HandleHit;
        minigame.OnFinishMinigame += FinishMinigame;

        // Debug de Ataque
        // StartAttack();
    }

    public void StartAttack()
    {
        playerInput.SwitchCurrentActionMap("Minigame");
        minigame.StartMinigame();
    }

    public void OnAttack(InputValue value)
    {
        // Debug.Log("Input recibido en BattleController");
        minigame.RecieveHit();
    }

    // Coge el enum HitPrecision del minijuego que es publico y según la situación, tal...
    void HandleHit(HitPrecision precision)
    {
        switch (precision)
        {
            case HitPrecision.Perfect:
                Debug.Log("PERFECT");
                break;
            case HitPrecision.Good:
                Debug.Log("GOOD");
                break;
            case HitPrecision.Miss:
                Debug.Log("MISS");
                break;
        }
    }

    void FinishMinigame()
    {
        Debug.Log("Minijuego terminado");
        playerInput.SwitchCurrentActionMap("Player");
    }
}

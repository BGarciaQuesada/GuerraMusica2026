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

        StartAttack();
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

    void HandleHit(bool acierto)
    {
        if (acierto)
            Debug.Log("Golpe");
        else
            Debug.Log("Fallo");
    }

    void FinishMinigame()
    {
        Debug.Log("Minijuego terminado");
        playerInput.SwitchCurrentActionMap("Player");
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class BattleInputController : MonoBehaviour
{
    public AttackMinigame minigame;
    public PlayerInput playerInput; // [!] Asignarlo luego automáticamente.......

    void Start()
    {
        // Conectar eventos del minijuego
        minigame.OnMinigameHit += HandleHitOutcome;
        minigame.OnMinigameFinish += FinishMinigame;

        // Test
        AttackStart(); 
    }

    public void AttackStart()
    {
        playerInput.SwitchCurrentActionMap("Minigame");
        minigame.MinigameStart();
    }

    public void OnAttack(InputValue value)
    {
        Debug.Log("ATAQUE");
        minigame.RecieveAttack();
    }

    void HandleHitOutcome(bool acierto)
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
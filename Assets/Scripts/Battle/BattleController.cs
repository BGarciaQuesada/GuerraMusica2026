using UnityEngine;
using UnityEngine.InputSystem;

// [!] Oh Dios Santo, esta clase está engordando mucho, verás como llegue el momento donde lo tenga que separar.............

public class BattleController : MonoBehaviour
{
    public static BattleController Instance;

    EnemyAI currentEnemy; // Solo va a ser 1 por batalla, nada de arrays

    public AttackMinigame minigame;
    public PlayerInput playerInput;
    int damage = 0;

    // Singleton (Esto hace actualmente que Big Vegas no se destruya OnLoad... No pasa nada, ¿no?)
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Conectar eventos del minijuego
        minigame.OnMinigameHit += HandleHit;
        minigame.OnFinishMinigame += FinishMinigame;

        // Debug de Ataque
        // StartAttack();
    }

    public void StartBattle(EnemyAI enemy)
    {
        currentEnemy = enemy;
        StartPlayerTurn();
    }

    // Los pongo en métodos separados porque es posible que reuse esto sin pasarle nada...
    void StartPlayerTurn()
    {
        playerInput.SwitchCurrentActionMap("Minigame");
        damage = 0;
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
                damage += 20;
                break;
            case HitPrecision.Good:
                Debug.Log("GOOD");
                damage += 10;
                break;
            case HitPrecision.Miss:
                Debug.Log("MISS");
                break;
        }
    }
    
    void FinishMinigame()
    {
        if (currentEnemy != null)
        {
            // Hay enemigo, recibe daño
            currentEnemy.TakeDamage(damage);

            // ¿Suficiente daño como para morirse?
            if (currentEnemy.IsPendingDeath)
            {
                // Hala
                EndBattle();
                return;
            }
        }

        // El enemigo NO está esperando a morirse, que empiece el enemigo
        StartEnemyTurn();
    }

    void StartEnemyTurn()
    {
        // Placeholder enemigo (literal se espera un rato para darle otra vez el turno al jugador ahora mismo)
        Invoke(nameof(EndEnemyTurn), 1.5f);
    }

    void EndEnemyTurn()
    {
        StartPlayerTurn();
    }

    void EndBattle()
    {
        BattleTransitionManager.Instance.EndBattle();
        playerInput.SwitchCurrentActionMap("Player");

        currentEnemy.Die();
        currentEnemy = null;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

// [!] Oh Dios Santo, esta clase está engordando mucho, verás como llegue el momento donde lo tenga que separar.............

public class BattleController : MonoBehaviour
{
    public static BattleController Instance;

    EnemyAI currentEnemy; // Solo va a ser 1 por batalla, nada de arrays
    EnemyHealth enemyHealth;
    EnemyCombat enemyCombat;

    public AttackMinigame minigame;
    public PlayerInput playerInput;

    int damage;

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

        enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyCombat = enemy.GetComponent<EnemyCombat>();

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
        // Ya no estoy llamando a enemigo como tal sino a enemy health, así que no necesito liarme de que exista
        enemyHealth.TakeDamage(damage);

        if (enemyHealth.IsDead)
        {
            EndBattle();
            return;
        }

        // El enemigo NO está esperando a morirse, que empiece el enemigo
        StartEnemyTurn();
    }

    void StartEnemyTurn()
    {
        // Coge el daño del enemigo, lo imprime (debería aplicarse a la salud del jugador en un futuro), se espera, invoca fin de turno enemigo.
        int dmg = enemyCombat.GetAttackDamage();

        Debug.Log($"Enemigo ataca con {dmg} de daño");

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

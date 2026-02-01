using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

// [!] Oh Dios Santo, esta clase está engordando mucho, verás como llegue el momento donde lo tenga que separar.............

public class BattleController : MonoBehaviour
{
    public static BattleController Instance;

    EnemyAI currentEnemy; // Solo va a ser 1 por batalla, nada de arrays
    EnemyHealth enemyHealth;
    EnemyCombat enemyCombat;

    public BattleMenu battleMenu;
    public AttackMenu attackMenu;
    public RunMinigame runMinigame;
    public AttackMinigame attackMinigame;
    public PlayerInput playerInput;

    SO_Skill currentSkill;
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
        attackMinigame.OnMinigameHit += HandleHit;
        attackMinigame.OnFinishMinigame += FinishMinigame;

        attackMenu.OnSkillSelected += StartSkill;
        runMinigame.OnFinished += TryEscape;

        attackMinigame.OnMinigameHit += HandleHit;
        attackMinigame.OnFinishMinigame += FinishMinigame;

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
        battleMenu.Open();
    }

    void StartSkill(SO_Skill skill)
    {
        currentSkill = skill;

        damage = 0;
        battleMenu.Close();

        attackMinigame.StartMinigame();
    }

    void HandleHit(HitPrecision precision)
    {
        // Ahora el switch está en skill
        damage += currentSkill.GetDamage(precision);
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

        // [!] El método de EndEnemyTurn al final no iba a llegar a más, suprimido
        Invoke(nameof(StartPlayerTurn), 1.5f);
    }

    void TryEscape(bool success)
    {
        if (success)
            EndBattle();
        else
            StartEnemyTurn();
    }

    void EndBattle()
    {
        BattleTransitionManager.Instance.EndBattle();
        playerInput.SwitchCurrentActionMap("Player");

        battleMenu.Close();
        currentEnemy.Die();
    }
}

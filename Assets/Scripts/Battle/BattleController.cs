using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

// [!] Oh Dios Santo, esta clase está engordando mucho, verás como llegue el momento donde lo tenga que separar.............

public class BattleController : MonoBehaviour
{
    public static BattleController Instance;
    public PlayerInput playerInput;

    [Header("Menus")]
    public BattleMenu battleMenu;
    public AttackMenu attackMenu;
    public PartnerMenu partnerMenu;

    [Header("Minijuegos")]
    public AttackMinigame attackMinigame;
    public RunMinigame runMinigame;

    [Header("Datos Enemigo")]
    EnemyAI currentEnemy; // Solo va a ser 1 por batalla, nada de arrays
    EnemyHealth enemyHealth;
    EnemyCombat enemyCombat;

    SOSkill currentSkill;
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
        // Battle menu
        battleMenu.OnAttack += OpenAttackMenu;
        battleMenu.OnPartner += OpenPartnerMenu;
        battleMenu.OnRun += StartRun;

        // Attack menu
        attackMenu.OnSkillSelected += OnSkillSelected;
        attackMenu.OnBack += ReturnToBattleMenu;

        // Partner menu
        partnerMenu.OnBack += ReturnToBattleMenu;

        // Minijuegos
        attackMinigame.OnMinigameHit += OnHit;
        attackMinigame.OnFinishMinigame += OnMinigameFinished;

        // Debug de Ataque
        // StartAttack();
    }

    public void StartBattle(EnemyAI enemy)
    {
        currentEnemy = enemy;

        enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyCombat = enemy.GetComponent<EnemyCombat>();

        enemyHealth.OnDeath += OnEnemyDeath;

        StartPlayerTurn();
    }

    // ====== TURNO JUGADOR ======

    // Los pongo en métodos separados porque es posible que reuse esto sin pasarle nada...
    void StartPlayerTurn()
    {
        battleMenu.Open();
        playerInput.SwitchCurrentActionMap("UI"); // No paro de usar ActionMaps, espero que no tenga consecuencias ( ._.)
    }

    void EndPlayerTurn()
    {
        battleMenu.Close();
        StartEnemyTurn();
    }

    // ====== TURNO ENEMIGO ======

    void StartEnemyTurn()
    {
        Debug.Log("Turno enemigo");

        int dmg = enemyCombat.GetAttackDamage();

        Debug.Log("Enemigo pega con " + dmg);

        Invoke(nameof(StartPlayerTurn), 1.2f);
    }

    // ====== MENÚS ======

    void OpenAttackMenu()
    {
        battleMenu.Close();
        attackMenu.Open();
    }

    void OpenPartnerMenu()
    {
        battleMenu.Close();
        partnerMenu.Open();
    }

    void ReturnToBattleMenu()
    {
        attackMenu.Close();
        partnerMenu.Close();
        battleMenu.Open();
    }

    // ====== SKILLS ======

    void OnSkillSelected(SOSkill skill)
    {
        currentSkill = skill;
        attackMenu.Close();

        if (skill.useMinigame) // ¿Tiene minijuego?
        {
            // Llama al minijuego empezando a cero y va sumando
            damage = 0;
            playerInput.SwitchCurrentActionMap("Minigame");
            attackMinigame.StartMinigame();
        }
        else
        {
            // Pedir daño directo
            ApplySkillDirect(skill);
            EndPlayerTurn();
        }
    }

    // Daño puro y duro
    void ApplySkillDirect(SOSkill skill)
    {
        enemyHealth.TakeDamage(skill.perfectDamage);
    }

    // ====== MINIJUEGOS ======

    void OnHit(HitPrecision precision)
    {
        damage += currentSkill.GetDamage(precision);
    }

    void OnMinigameFinished()
    {
        enemyHealth.TakeDamage(damage);
        EndPlayerTurn();
    }

    // ====== HUIR ======

    void StartRun()
    {
        //runMinigame.StartGame();
    }

    // ====== FIN POR MUERTE ======

    void OnEnemyDeath()
    {
        Debug.Log("Fin de batalla");

        currentEnemy.Die();
        BattleTransitionManager.Instance.EndBattle();
        playerInput.SwitchCurrentActionMap("Player");
    }
}

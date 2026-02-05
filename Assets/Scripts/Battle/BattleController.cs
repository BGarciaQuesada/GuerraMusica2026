using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

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
        Debug.Log("Comienza la batalla");

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
        Debug.Log("Turno del jugador, abre battle menu");

        battleMenu.Open();
        playerInput.SwitchCurrentActionMap("UI"); // No paro de usar ActionMaps, espero que no tenga consecuencias ( ._.)
    }

    void EndPlayerTurn()
    {
        Debug.Log("Fin del turno del jugador, cierra battle menu");

        battleMenu.Close();

        if (!enemyHealth.IsDead)
        {
            Debug.Log("El enemigo sigue vivo, empezando su turno");
            StartEnemyTurn();
        }
            
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
            playerInput.SwitchCurrentActionMap("Minigame");
            Debug.Log("Skill con minijuego");

            // Llama al minijuego empezando a cero y va sumando
            damage = 0;
            attackMinigame.StartMinigame();
        }
        else
        {
            Debug.Log("Skill sin minijuego");

            // Pedir daño directo
            ApplySkillDirect(skill);
            EndPlayerTurn();
        }
    }

    // Daño puro y duro
    void ApplySkillDirect(SOSkill skill)
    {
        Debug.Log("Aplicar daño directo a enemigo");
        enemyHealth.TakeDamage(skill.perfectDamage);
    }

    // Esto es para cuando tiene el MINIGAME ACTIONMAP, porque si no nadie llama a RecieveHit y no funciona
    public void OnAttack(InputValue value)
    {
        if (attackMinigame.gameObject.activeSelf)
            attackMinigame.RecieveHit();
    }


    // ====== MINIJUEGOS ======

    void OnHit(HitPrecision precision)
    {
        damage += currentSkill.GetDamage(precision);
    }

    void OnMinigameFinished()
    {
        Debug.Log("Minijuego terminado");
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

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleController : MonoBehaviour
{
    public static BattleController Instance;
    public PlayerInput playerInput;

    [Header("Menus")]
    [SerializeField] private BattleMenu battleMenu;
    [SerializeField] private AttackMenu attackMenu;
    [SerializeField] private PartnerMenu partnerMenu;
    [SerializeField] private PartnerAttackMenu partnerAttackMenu;

    [Header("Minijuegos")]
    [SerializeField] private AttackMinigame attackMinigame;
    [SerializeField] private RunMinigame runMinigame;

    [Header("Datos Enemigo")]
    [SerializeField] private EnemyAI currentEnemy; // Solo va a ser 1 por batalla, nada de arrays
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private EnemyCombat enemyCombat;

    [Header("Datos Jugador")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Datos Compañero")]
    [SerializeField] private PartnerCombat[] partnersAvailable;

    PartnerCombat currentPartner;
    int currentPartnerIndex = 0;

    // ESTADOS DE TURNOS
    private bool playerActionFinished;
    private bool partnerActionFinished;
    private SOSkill selectedPartnerSkill;


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
        attackMenu.OnSkillSelected += OnPlayerSkillSelected;
        attackMenu.OnBack += ReturnToBattleMenu;

        // Partner menu
        partnerMenu.OnChangePartner += ChangePartner;
        partnerMenu.OnBack += ReturnToBattleMenu;


        // Minijuegos
        attackMinigame.OnMinigameHit += OnHit;
        attackMinigame.OnFinishMinigame += OnMinigameFinished;

        // Compañero inicial
        currentPartner = partnersAvailable[0];

        // Debug de Ataque
        // StartAttack();
    }

    public void StartBattle(EnemyAI enemy)
    {
        Debug.Log("Comienza la batalla");

        currentEnemy = enemy;

        enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyCombat = enemy.GetComponent<EnemyCombat>();

        enemyHealth.OnEnemyDeath += OnEnemyDeath;
        playerHealth.OnPlayerDeath += OnPlayerDeath;

        StartPlayerTurn();
    }

    // ====== TURNO JUGADOR ======

    // Los pongo en métodos separados porque es posible que reuse esto sin pasarle nada...
    void StartPlayerTurn()
    {
        if (playerHealth.ShouldSkipTurn())
        {
            Debug.Log("Jugador tiene stun, pierde turno");
            StartEnemyTurn();
            return;
        }

        Debug.Log("Turno del jugador, abre battle menu");

        playerActionFinished = false;
        partnerActionFinished = false;

        battleMenu.Open();
        playerInput.SwitchCurrentActionMap("UI"); // No paro de usar ActionMaps, espero que no tenga consecuencias ( ._.)
    }

    void EndPlayerTurn()
    {
        Debug.Log("Fin del turno del jugador, cierra battle menu");

        battleMenu.Close();

        /*if (!enemyHealth.IsDead)
        *{
        *    Debug.Log("El enemigo sigue vivo, empezando su turno");
        *    StartEnemyTurn();
        }*/
            
    }

    // ====== TURNO COMPAÑERO =====

    void StartPartnerTurn()
    {
        if (currentPartner == null)
        {
            StartEnemyTurn();
            return;
        }

        partnerAttackMenu.Open(currentPartner.skills);
    }

    void OnPartnerSkillSelected(SOSkill skill)
    {
        ApplySkillDirect(skill);

        if(!enemyHealth.IsDead)
            StartEnemyTurn();
    }

    void ReturnToPlayerTurn()
    {
        StartPlayerTurn();
    }

    // ====== TURNO ENEMIGO ======
    // [!] Tantos returns... La profesora de 1º de DAM me crucificaría. Ahora mismo buscamos funcionalidad, no optimización.
    void StartEnemyTurn()
    {
        if (enemyHealth.ShouldSkipTurn())
        {
            Debug.Log("Enemigo tiene stun, pierde turno");
            Invoke(nameof(StartPlayerTurn), 1f);
            return;
        }

        Debug.Log("Turno enemigo");

        SOSkill skill = enemyCombat.GetNextSkill();

        if (skill == null)
        {
            StartPlayerTurn();
            return;
        }

        int dmg = skill.perfectDamage;

        Debug.Log("Enemigo usa skill: " + skill.skillName);

        if (skill.stun)
            playerHealth.ApplyStun(skill.stunTurns);

        playerHealth.TakeDamage(dmg);

        if (playerHealth.IsDead)
        {
            // Debug.Log("Jugador muerto, fin batalla");
            return;
        }

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

    void OnPlayerSkillSelected(SOSkill skill)
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

        if (skill.stun)
            enemyHealth.ApplyStun(skill.stunTurns); //stun no tiene minijuego asi que ea
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

    void OnPlayerDeath()
    {
        Debug.Log("Jugador muerto, fin de batalla");

        // Método provisional, esto tendría que llevar a una pantalla de game over
        BattleTransitionManager.Instance.EndBattle();
        playerInput.SwitchCurrentActionMap("Player");
    }
}

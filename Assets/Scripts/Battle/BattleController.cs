using UnityEngine;
using UnityEngine.InputSystem;

public class BattleController : MonoBehaviour
{

    public GameObject auxAssigns;

    public GameObject battleTransitionManager;
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
    public EnemyAI currentEnemy; // Solo va a ser 1 por batalla, nada de arrays
    public EnemyHealth enemyHealth;
    [SerializeField] private EnemyCombat enemyCombat;

    [Header("Datos Jugador")]
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Datos Compa�ero")]
    [SerializeField] private Transform partnerSpawnPoint;
    [SerializeField] private PartnerCombat[] partnersAvailable;

    [Header("UI")]
    [SerializeField] EnemyHealthUI enemyHealthUI;


    public PartnerCombat currentPartner;
    private PartnerCombat currentPartnerInstance;

    SOSkill currentSkill;
    int damage;

    // Singleton (Esto hace actualmente que Big Vegas no se destruya OnLoad... No pasa nada, �no?)
    //si pasaba xdxd
    void Awake()
    {
        auxAssigns = GameObject.FindWithTag("AuxAssigns");
        battleTransitionManager = GameObject.FindWithTag("GameManager");

        /*
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
            */
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

        // Ataques del compa�ero
        partnerAttackMenu.OnPartnerSkillSelected += OnPartnerSkillSelected;
        partnerAttackMenu.OnBack += ReturnToPlayerTurn;

        // Minijuegos
        attackMinigame.OnMinigameHit += OnHit;
        attackMinigame.OnFinishMinigame += OnMinigameFinished;

        // Compa�ero inicial
        if (partnersAvailable.Length > 0)
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

        // Poner la salud del enemigo actual en la barra de vida enemiga
        enemyHealthUI.Initialize(enemyHealth);

        if (partnersAvailable.Length > 0)
        {
            currentPartner = partnersAvailable[0];
            SpawnPartner(currentPartner);
        }

        StartPlayerTurn();
    }

    void SpawnPartner(PartnerCombat partnerPrefab)
    {
        if (currentPartnerInstance != null)
            Destroy(currentPartnerInstance.gameObject);

        currentPartnerInstance = Instantiate(
            partnerPrefab,
            partnerSpawnPoint.position,
            partnerSpawnPoint.rotation);
    }


    // ====== TURNO JUGADOR ======

    // Los pongo en m�todos separados porque es posible que reuse esto sin pasarle nada...
    void StartPlayerTurn()
    {

        if (playerHealth.ShouldSkipTurn())
        {
            Debug.Log("Jugador tiene stun, pierde turno");
            StartEnemyTurn();
            return;
        }

        Debug.Log("Turno del jugador, abre battle menu");

        battleMenu.Open();
        playerInput.SwitchCurrentActionMap("UI"); // No paro de usar ActionMaps, espero que no tenga consecuencias ( ._.)
    }

    void EndPlayerTurn()
    {
        Debug.Log("Fin del turno del jugador, cierra battle menu");

        battleMenu.Close();
        StartPartnerTurn();

        /*if (!enemyHealth.IsDead)
        *{
        *    Debug.Log("El enemigo sigue vivo, empezando su turno");
        *    StartEnemyTurn();
        }*/

    }

    // ====== TURNO COMPA�ERO =====

    void StartPartnerTurn()
    {
        if (auxAssigns.GetComponent<AuxiliarAssigns>().botónAtaquePartner.GetComponent<SkillsPatch>().turnosLulúBuff > 0)
            auxAssigns.GetComponent<AuxiliarAssigns>().botónAtaquePartner.GetComponent<SkillsPatch>().turnosLulúBuff--;
        if (auxAssigns.GetComponent<AuxiliarAssigns>().botónAtaquePartner.GetComponent<SkillsPatch>().turnosParsifalBuff > 0)
            auxAssigns.GetComponent<AuxiliarAssigns>().botónAtaquePartner.GetComponent<SkillsPatch>().turnosParsifalBuff--;

        if (currentPartner == null)
        {
            StartEnemyTurn();
            return;
        }

        if (!enemyHealth.IsDead)
            partnerAttackMenu.Open(currentPartnerInstance.GetSkills());
    }

    void OnPartnerSkillSelected(SOSkill skill)
    {
        partnerAttackMenu.Close();

        ApplySkill(skill, true);

        if (!enemyHealth.IsDead)
            StartEnemyTurn();
    }

    void ReturnToPlayerTurn()
    {
        partnerAttackMenu.Close();
        StartPlayerTurn();
    }

    // ====== CAMBIAR COMPA ======

    void ChangePartner(PartnerCombat selectedPartner)
    {
        currentPartner = selectedPartner;
        SpawnPartner(currentPartner);

        Debug.Log("Compa�ero cambiado a: " + currentPartner.name);

        // cambiar consume turno completo
        partnerMenu.Close();
        StartEnemyTurn();
    }

    // ====== TURNO ENEMIGO ======
    public void StartEnemyTurn()
    {
        if (enemyHealth.ShouldSkipTurn())
        {
            Debug.Log("Enemigo tiene stun, pierde turno");
            Invoke(nameof(StartPlayerTurn), 1f);
            return;
        }

        Debug.Log("Turno enemigo");

        SOSkill skill = enemyCombat.GetNextSkill();

        if (skill != null)
            ApplySkill(skill, false);

        if (!playerHealth.IsDead)
        {
            playerHealth.TickModifier();
            enemyHealth.TickModifier();

            Invoke(nameof(StartPlayerTurn), 1.2f);
        }

    }

    // ====== MEN�S ======

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

        if (skill.useMinigame) // �Tiene minijuego?
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

            // Pedir da�o directo
            ApplySkill(skill, true);
            EndPlayerTurn();
        }
    }

    void ApplySkill(SOSkill skill, bool targetEnemy)
    {
        skill.Apply(playerHealth, enemyHealth, targetEnemy);
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
        currentSkill.ApplyWithDamage(playerHealth, enemyHealth, true, damage);
        EndPlayerTurn();
    }

    // ====== HUIR ======

    void StartRun()
    {
        //runMinigame.StartGame();
    }

    // ====== FIN POR MUERTE ======

    public void OnEnemyDeath()
    {
        Debug.Log("Fin de batalla");

        currentEnemy.Die();
        battleTransitionManager.GetComponent<BattleTransitionManager>().EndBattle();
        playerInput.SwitchCurrentActionMap("Player");
    }

    void OnPlayerDeath()
    {
        Debug.Log("Jugador muerto, fin de batalla");

        // M�todo provisional, esto tendr�a que llevar a una pantalla de game over
        battleTransitionManager.GetComponent<BattleTransitionManager>().EndBattle();
        playerInput.SwitchCurrentActionMap("Player");
    }
}

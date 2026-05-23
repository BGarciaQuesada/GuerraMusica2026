using System;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Patrol,
    Chase,
    Combat
}

public class EnemyAI : MonoBehaviour
{
    private GameObject battleTransitionManager;
    // [!] No quería que fuese por puntos. Quería que lo calculase por su cuenta. POR AHORA debería funcionar
    [Header("Patrulla")]
    [SerializeField] bool canPatrol = true;
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float waitTimeAtPoint = 4f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float patrolSpeed = 2f;

    public NavMeshAgent agent;
    private Transform player;

    private float waitTimer;

    public EnemyState state;

    public Vector3 enemyPos;

    public GameObject auxAssigns;

    // Coger NavMeshAgent automáticamente
    void Awake()
    {
        auxAssigns = GameObject.FindWithTag("AuxAssigns");
        battleTransitionManager = GameObject.FindWithTag("GameManager");
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        // Si puede patrullar empieza patrullando
        if (canPatrol)
        {
            state = EnemyState.Patrol;
            SetPatrol();
        } else
        {
            state = EnemyState.Idle; // Jefes quietos
        }
            
    }

    void Update()
    {
        if(state == EnemyState.Combat && transform.localPosition.y <= 0)
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            transform.localPosition = new Vector3(this.gameObject.transform.localPosition.x, 0, this.gameObject.transform.localPosition.z);
        }
        else
        {
            this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
        // Debug.Log($"Estado actual: {state}");
        switch (state)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                Chase();
                break;

            case EnemyState.Combat:
                // agent.isStopped = true;
                
                break;
        }
    }

    #region Patrol
    public void Patrol()
    {
        // Si no sabe a d�nde ir, llamar a punto (inicio)
        if (!agent.hasPath)
            SetPatrol();

        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            // Se espera a d�nde ir y vuelve a llamar a recibir punto
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                waitTimer = 0f;
                SetPatrol();
            }
        }
    }

    void SetPatrol()
    {
        agent.speed = patrolSpeed;

        // [!] Esto era antes el TryGetRandomPatrolPoint(). Lo he combinado en uno.
        // Calcula una posici�n dentro del radio de patrulla
        Vector3 random = transform.position + UnityEngine.Random.insideUnitSphere * patrolRadius;

        // Intenta buscar ese punto en el NavMesh, si lo encuentra, lo pone como destino
        if (NavMesh.SamplePosition(random, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
    }
    #endregion

    #region Chase
    void Chase()
    {
        if (player == null) return;

        agent.SetDestination(player.position);
    }

    public void OnPlayerDetected(Transform playerTransform)
    {
        if (state == EnemyState.Combat) return;

        player = playerTransform;
        state = EnemyState.Chase;
        agent.speed = chaseSpeed;
    }

    public void OnPlayerLost()
    {
        if (state == EnemyState.Combat) return;
        state = EnemyState.Patrol;
        agent.speed = patrolSpeed;
        SetPatrol();
    }
    #endregion

    #region Combat Initiation

    private void OnCollisionEnter(Collision other)
    {
        // [!] Esto es para iniciar el combate al chocar con el jugador.
        // [!] Usado para enemigos normales, los jefes tienen que entrar en la zona de combate, no chocar con el jugador.
        if (other.collider.CompareTag("Player"))
        {
            auxAssigns.GetComponent<AuxiliarAssigns>().battleMenuObj.transform.GetChild(2).gameObject.SetActive(true);
            Debug.Log("Comenzar combate");
            StartCombat();

        }
    }

    // [!] Llamado por BossBattleTrigger para iniciar el combate al entrar en la zona del jefe (combat requiere player)
    public void StartBossCombat(Transform playerTransform)
    {   
        player = playerTransform;
        StartCombat();
    }

    void StartCombat()
    {
        enemyPos = new Vector3(this.gameObject.transform.position.x, 0, this.gameObject.transform.position.z);
        if (state == EnemyState.Combat) return;
        
        state = EnemyState.Combat;
        agent.isStopped = true;

        Rigidbody rb = rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        Debug.Log("Combate iniciado");

        // Llamar a que inicie el combate
        battleTransitionManager.GetComponent<BattleTransitionManager>().StartBattleTransition(
            player,
            this
        );
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    #endregion
}

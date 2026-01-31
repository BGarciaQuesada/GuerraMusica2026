using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol,
    Chase,
    Combat
}

// Dejemos de olvidarnos grax
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]

public class EnemyAI : MonoBehaviour
{
    // [!] No quería que fuese por puntos. Quería que lo calculase por su cuenta. POR AHORA debería funcionar
    [Header("Patrulla")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float patrolSpeed = 2f;

    [SerializeField] NavMeshAgent agent;
    private Transform player;

    private Vector3 currentPatrolPoint;
    private float waitTimer;

    private EnemyState state = EnemyState.Patrol;

    // Coger NavMeshAgent automáticamente
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        SetPatrol();
    }

    void Update()
    {
        switch (state)
        {
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
    void Patrol()
    {
        // Si no sabe a dónde ir, llamar a punto (inicio)
        if (!agent.hasPath)
            SetPatrol();

        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            // Se espera a dónde ir y vuelve a llamar a recibir punto
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
        // Calcula una posición dentro del radio de patrulla
        Vector3 random = transform.position + Random.insideUnitSphere * patrolRadius;

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
    #endregion

    #region Combat

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Comenzar combate");
            StartCombat();
        }
    }

    void StartCombat()
    {
        if (state == EnemyState.Combat) return;

        state = EnemyState.Combat;
        // agent.isStopped = true;

        Debug.Log("Combate iniciado");

        // Llamar a que inicie el combate
        BattleTransitionManager.Instance.StartBattleTransition(
            player,
            this
        );
    }

    // [!] Total, BattleController tiene a EnemyAI, puedo llamarlo en vez de hacer callbacks entre ellos como una loca
    public void Die()
    {
        Destroy(gameObject);
    }
    #endregion
}

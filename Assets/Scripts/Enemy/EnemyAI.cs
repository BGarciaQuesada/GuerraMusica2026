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

public class EnemyAI : MonoBehaviour
{
    // [!] No quería que fuese por puntos. Quería que lo calculase por su cuenta. POR AHORA debería funcionar
    [Header("Patrulla")]
    [SerializeField] private float patrolRadius = 10f;
    [SerializeField] private float waitTimeAtPoint = 2f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 4.5f;
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Stats")]
    [SerializeField] int maxHP = 30;
    private int currentHP;
    private bool pendingDeath;

    [SerializeField] NavMeshAgent agent;
    private Transform player;

    private EnemyState state = EnemyState.Patrol;

    private Vector3 currentPatrolPoint;
    private bool hasPatrolPoint;
    private float waitTimer;

    // [!] Método flecha no explotes otra vez y entres en bucle grax
    public bool IsPendingDeath => pendingDeath;

    void Awake()
    {
        currentHP = maxHP;
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
    void SetPatrol()
    {
        agent.speed = patrolSpeed;
        hasPatrolPoint = TryGetRandomPatrolPoint(out currentPatrolPoint);
        if (hasPatrolPoint)
        {
            agent.SetDestination(currentPatrolPoint);
        }
    }

    void Patrol()
    {
        if (!hasPatrolPoint) return;

        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                waitTimer = 0f;
                SetPatrol();
            }
        }
    }

    bool TryGetRandomPatrolPoint(out Vector3 point)
    {
        for (int i = 0; i < 10; i++)
        {
            // Calcula una posición dentro del radio de patrulla
            Vector3 randomPos = transform.position + Random.insideUnitSphere * patrolRadius;
            // Intenta buscar ese punto en el NavMesh
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                // ¿Encontrado? Tomar el punto
                point = hit.position;
                return true;
            }
        }

        // ¿No encontrado? Nada
        point = Vector3.zero;
        return false;
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

    public void TakeDamage(int dmg)
    {
        if (pendingDeath) return;

        currentHP -= dmg;

        if (currentHP <= 0)
        {
            currentHP = 0;
            pendingDeath = true; // PONEMOS LA VANDERA DE QUE SE MUERA PORQUE SI SE DESTRUYE AQUÍ TE SACA AL MUNDO CON EL MINIJUEGO
            Debug.Log("Enemigo derrotado (pendiente de finalizar turno)");
        }
    }

    // [!] Total, BattleController tiene a EnemyAI, puedo llamarlo en vez de hacer callbacks entre ellos como una loca
    public void Die()
    {
        Destroy(gameObject);
    }
    #endregion
}

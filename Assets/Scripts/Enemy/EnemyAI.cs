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
    [SerializeField] float patrolRadius = 10f;
    [SerializeField] float waitTimeAtPoint = 2f;

    [Header("Chase")]
    [SerializeField] float chaseSpeed = 4.5f;
    [SerializeField] float patrolSpeed = 2f;

    [SerializeField] NavMeshAgent agent;
    Transform player;

    EnemyState state = EnemyState.Patrol;

    Vector3 currentPatrolPoint;
    bool hasPatrolPoint;
    float waitTimer;

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
                agent.isStopped = true;
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
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCombat();
        }
    }

    void StartCombat()
    {
        if (state == EnemyState.Combat) return;

        state = EnemyState.Combat;
        agent.isStopped = true;

        Debug.Log("Combate iniciado");
        // Llamar a que inicie el combate
    }

    public void OnBattleFinished()
    {
        Destroy(gameObject);
    }
    #endregion
}

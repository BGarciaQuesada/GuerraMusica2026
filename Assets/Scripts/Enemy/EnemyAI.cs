using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Patrol,
    Chase,
    Combat
}

public class EnemyAI : MonoBehaviour
{
    [Header("Patrulla")]
    public Transform[] patrolPoints;

    NavMeshAgent agent;
    Transform player;
    int patrolIndex;
    EnemyState state = EnemyState.Patrol;
    
    void Awake()
    {
        // Obtener NavMeshAgent automáticamente
        agent = GetComponent<NavMeshAgent>();
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

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }
    }

    void Chase()
    {
        if (player == null) return;
        agent.SetDestination(player.position);
    }

    // Llamado por DetectionZone
    public void OnPlayerDetected(Transform playerTransform)
    {
        if (state == EnemyState.Combat) return;

        player = playerTransform;
        state = EnemyState.Chase;
    }

    // Contacto físico real
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCombat();
        }
    }

    void StartCombat()
    {
        state = EnemyState.Combat;
        agent.isStopped = true;

        // Entrar en batalla
    }

    public void OnBattleFinished()
    {
        Destroy(gameObject); // enemigo derrotado
    }
}

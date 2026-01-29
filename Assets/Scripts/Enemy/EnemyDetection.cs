using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    EnemyAI enemy;

    void Awake()
    {
        enemy = GetComponentInParent<EnemyAI>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.OnPlayerDetected(other.transform);
        }
    }
}

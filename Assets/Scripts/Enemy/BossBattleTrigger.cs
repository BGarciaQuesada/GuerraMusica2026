using UnityEngine;

public class BossBattleTrigger : MonoBehaviour
{
    [SerializeField] EnemyAI boss;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        boss.StartBossCombat(other.transform);
    }
}
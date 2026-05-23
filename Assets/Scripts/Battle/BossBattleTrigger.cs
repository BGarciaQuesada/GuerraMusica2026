using UnityEngine;

public class BossBattleTrigger : MonoBehaviour
{
    public EnemyAI boss;

    public Vector3 bossPos;

    public GameObject auxAssigns;

    void Start()
    {
        auxAssigns = GameObject.FindWithTag("AuxAssigns");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (other.CompareTag("Player"))
        {
            auxAssigns.GetComponent<AuxiliarAssigns>().battleMenuObj.transform.GetChild(2).gameObject.SetActive(false);
            auxAssigns.GetComponent<AuxiliarAssigns>().currentEnemy = boss.gameObject;

            bossPos = boss.gameObject.transform.position;

            //el diablo demasiadas referencias ._____.

            auxAssigns.GetComponent<AuxiliarAssigns>().extras.transform.GetChild(0).gameObject.
            GetComponent<ActivateFleeMinigame>().bossPos = bossPos;

            boss.StartBossCombat(other.transform);
        }
        
    }
}
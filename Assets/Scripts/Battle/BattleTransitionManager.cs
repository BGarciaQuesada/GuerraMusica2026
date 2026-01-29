using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// El punto de esta clase es pararlo todo, llamar a BattleTransitionUI para transportar mientras eso ocurre, devolver a la posición normal tras combate
public class BattleTransitionManager : MonoBehaviour
{
    public static BattleTransitionManager Instance;

    [Header("Battle Arena")]
    [SerializeField] Transform battlePlayerSpawn;
    [SerializeField] Transform battleEnemySpawn;

    [Header("UI")]
    [SerializeField] BattleTransitionUI transitionUI;

    Vector3 playerWorldPosition;
    Quaternion playerWorldRotation;

    EnemyAI currentEnemy;
    Transform currentPlayer;

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


    public void StartBattleTransition(Transform player, EnemyAI enemy)
    {
        currentPlayer = player;
        currentEnemy = enemy;

        playerWorldPosition = player.position;
        playerWorldRotation = player.rotation;

        StartCoroutine(EnterBattle());
    }

    IEnumerator EnterBattle()
    {
        Time.timeScale = 0f;

        yield return transitionUI.PlayTransition();

        TeleportToBattle();

        Time.timeScale = 1f;
    }

    void TeleportToBattle()
    {
        TeleportPlayer(currentPlayer, battlePlayerSpawn);
        TeleportEnemy(currentEnemy, battleEnemySpawn);
    }

    #region Finish Battle
    public void EndBattle()
    {
        StartCoroutine(ExitBattle());
    }

    IEnumerator ExitBattle()
    {
        Time.timeScale = 0f;

        yield return transitionUI.PlayTransition();

        TeleportPlayer(currentPlayer, playerWorldPosition, playerWorldRotation);

        Time.timeScale = 1f;
    }
    #endregion

    #region Teleporters

    void TeleportPlayer(Transform player, Transform target)
    {
        TeleportPlayer(player, target.position, target.rotation);
    }

    void TeleportPlayer(Transform player, Vector3 pos, Quaternion rot)
    {
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        player.SetPositionAndRotation(pos, rot);

        if (cc) cc.enabled = true;
    }

    void TeleportEnemy(EnemyAI enemy, Transform target)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent) agent.enabled = false;

        enemy.transform.SetPositionAndRotation(target.position, target.rotation);

        if (agent) agent.enabled = true;
    }
    #endregion
}

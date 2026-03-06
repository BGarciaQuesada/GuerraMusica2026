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
    [SerializeField] GameObject battleUI;

    [Header("Camera Change")]
    [SerializeField] Camera worldCamera;
    [SerializeField] Camera battleCamera;

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

        // Tapar pantalla
        yield return transitionUI.PlayEnter();

        // tp
        TeleportToBattle();

        // Cambiar a cámara de Battle Plane
        worldCamera.gameObject.SetActive(false);
        battleCamera.gameObject.SetActive(true);

        // Mostrar UI de batalla
        battleUI.SetActive(true);

        // Esperar un poco (para apreciar la transición, se puede quitar si eso)
        yield return new WaitForSecondsRealtime(0.5f);

        // Destapar pantalla
        yield return transitionUI.PlayExit();

        Time.timeScale = 1f;

        BattleController.Instance.StartBattle(currentEnemy);
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

        // Tapar pantalla
        yield return transitionUI.PlayEnter();

        // tp
        TeleportPlayer(currentPlayer, playerWorldPosition, playerWorldRotation);

        // Cambiara a cámara del jugador
        battleCamera.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        // Ocultar UI de batalla
        battleUI.SetActive(false);

        // Esperar un poco (para apreciar la transición, se puede quitar si eso)
        yield return new WaitForSecondsRealtime(0.5f);

        // Destapar pantalla
        yield return transitionUI.PlayExit();

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
        if (agent)
        {
            agent.enabled = false;
            agent.updateRotation = false;
        }

        enemy.transform.SetPositionAndRotation(target.position, target.rotation);

        if (agent) agent.enabled = true;
    }
    #endregion
}

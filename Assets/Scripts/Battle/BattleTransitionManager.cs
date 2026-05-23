using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

// El punto de esta clase es pararlo todo, llamar a BattleTransitionUI para transportar mientras eso ocurre, devolver a la posici�n normal tras combate
public class BattleTransitionManager : MonoBehaviour
{

    public GameObject auxAssigns;
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
    {/*
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
            Destroy(gameObject);*/
    }

    void OnEnable()
    {
       auxAssigns = GameObject.FindWithTag("AuxAssigns"); 
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

        // Cambiar a c�mara de Battle Plane
        worldCamera.gameObject.SetActive(false);
        battleCamera.gameObject.SetActive(true);

        // Mostrar UI de batalla
        battleUI.SetActive(true);
        

        // Esperar un poco (para apreciar la transici�n, se puede quitar si eso)
        yield return new WaitForSecondsRealtime(0.5f);

        // Destapar pantalla
        yield return transitionUI.PlayExit();

        Time.timeScale = 1f;

        //BattleController.Instance.StartBattle(currentEnemy);

        currentPlayer.gameObject.GetComponent<BattleController>().StartBattle(currentEnemy);
    }

    void TeleportToBattle()
    {
        TeleportPlayer(currentPlayer, battlePlayerSpawn);
        TeleportEnemy(currentEnemy, battleEnemySpawn);
    }

    #region Finish Battle
    public void EndBattle()
    {
        auxAssigns.GetComponent<AuxiliarAssigns>().battleMenuObj.transform.GetChild(2).gameObject.SetActive(true);
        if(currentPlayer.gameObject.GetComponent<PlayerHealth>().CurrentHP <= 0)
        {
            BackToMenu();
        }
        else
        StartCoroutine(ExitBattle());
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    IEnumerator ExitBattle()
    {
        Time.timeScale = 0f;

        // Tapar pantalla
        yield return transitionUI.PlayEnter();

        // tp
        TeleportPlayer(currentPlayer, playerWorldPosition, playerWorldRotation);

        // Cambiara a c�mara del jugador
        battleCamera.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);

        // Ocultar UI de batalla
        battleUI.SetActive(false);

        // Esperar un poco (para apreciar la transici�n, se puede quitar si eso)
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

        Vector3 newRotation = new Vector3(enemy.transform.eulerAngles.x, target.eulerAngles.y, target.eulerAngles.z);
        Debug.Log($"Teleporting enemy to {target.position} with rotation {newRotation}");
        enemy.transform.SetPositionAndRotation(target.position, Quaternion.Euler(newRotation));

        if (agent) agent.enabled = true;
    }
    #endregion
}

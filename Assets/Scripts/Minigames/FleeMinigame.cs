using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FleeMinigame : MonoBehaviour
{
    public GameObject battleCanvas, auxAssignsObj, textoMinijuego, player, currentEnemy;
    private AuxiliarAssigns auxAssigns;

    public bool onMinigame;

    public Vector3 bossPos;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        auxAssignsObj = GameObject.FindWithTag("AuxAssigns");
        auxAssigns = auxAssignsObj.GetComponent<AuxiliarAssigns>();
        battleCanvas = auxAssigns.battleMenuObj;
        textoMinijuego = this.gameObject.transform.GetChild(1).gameObject;
        player = auxAssigns.player;
        
    }

    // Update is called once per frame
    void Update()
    {
        currentEnemy = auxAssigns.player.GetComponent<BattleController>().currentEnemy.gameObject;
    }

    void OnEnable()
    {
        this.gameObject.transform.GetChild(0).gameObject.GetComponent<Slider>().value = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Slider>().maxValue / 2;
        
        Debug.Log("Enable Minigame");

        Invoke(nameof(MiniGame), 0.1f);
    }

    public void MiniGame()
    {
        StartCoroutine(TextFlickering());

        BarGoesDownAhhFunc();
    }

    IEnumerator TextFlickering()
    {
        Debug.Log("TextFlicker");
        while (onMinigame)
        {
            Debug.Log("TextFlicker");
            textoMinijuego.SetActive(!textoMinijuego.activeSelf);

            yield return new WaitForSeconds(0.5f);
        }
        
    }

    async void BarGoesDownAhhFunc()
    {
        Debug.Log("BarFunc");
        while (onMinigame)
        {   
            Debug.Log("Bar");
            if(Input.GetKeyDown(KeyCode.Space))
            this.gameObject.transform.GetChild(0).gameObject.GetComponent<Slider>().value += 40;

            this.gameObject.transform.GetChild(0).gameObject.GetComponent<Slider>().value -= 0.5f;

            if(this.gameObject.transform.GetChild(0).gameObject.GetComponent<Slider>().value <= 10)
            {
                battleCanvas.SetActive(true);

                this.gameObject.SetActive(false);

                onMinigame = false;

                player.GetComponent<BattleController>().StartEnemyTurn();
            }
            
        
            if(this.gameObject.transform.GetChild(0).gameObject.GetComponent<Slider>().value >= 490)
            {
                player.GetComponent<BattleController>().battleTransitionManager.GetComponent<BattleTransitionManager>().EndBattle();

                this.gameObject.SetActive(false);

                onMinigame = false;

                Invoke(nameof(SendEntitiesBack), 0.3f);
            }

            await Task.Delay(1);
        }
    }

    async void SendEntitiesBack()
    {

        player.GetComponent<CharacterController>().enabled = false;

        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - 5);

        player.GetComponent<BattleController>().playerInput.SwitchCurrentActionMap("Player");

        player.GetComponent<CharacterController>().enabled = true;

        currentEnemy.GetComponent<EnemyAI>().agent.enabled = true;

        currentEnemy.GetComponent<EnemyAI>().state = EnemyState.Patrol;

        currentEnemy.GetComponent<EnemyAI>().agent.updateRotation = true;

        currentEnemy.GetComponent<EnemyAI>().agent.Warp(currentEnemy.GetComponent<EnemyAI>().enemyPos);


    }
}

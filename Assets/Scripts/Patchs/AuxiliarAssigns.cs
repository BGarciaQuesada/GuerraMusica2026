using UnityEngine;

public class AuxiliarAssigns : MonoBehaviour
{

    //para tener todas las referencias importantes en un objeto, no quiero tocar codigo que no entiendo xdd
    public GameObject player, currentEnemy, interchangeableBattleCanvas, extras;

    public GameObject battleMenuObj, fleeMinigameObj, gameManagerObj, botónAtaquePartner;

    public BattleController battleController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        battleController = player.GetComponent<BattleController>();
        gameManagerObj = GameObject.FindWithTag("GameManager");

        for(int i = 0; i < interchangeableBattleCanvas.transform.childCount; i++)
        {   
            interchangeableBattleCanvas.transform.GetChild(i).gameObject.TryGetComponent<FleeMinigame>(out FleeMinigame fleeMinigame);
            if(fleeMinigame != null)
            fleeMinigameObj = interchangeableBattleCanvas.transform.GetChild(i).gameObject;
            
            interchangeableBattleCanvas.transform.GetChild(i).gameObject.TryGetComponent<BattleMenu>(out BattleMenu battleMenu);
            if(battleMenu != null)
            battleMenuObj = interchangeableBattleCanvas.transform.GetChild(i).gameObject;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(battleController != null)
        if(battleController.currentEnemy != null)
        currentEnemy = battleController.currentEnemy.gameObject;
        
    }
}

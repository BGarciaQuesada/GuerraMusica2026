using UnityEngine;

public class ActivateFleeMinigame : MonoBehaviour
{

   [SerializeField] private GameObject auxAssigns, fleeMinigamePanel, battleMenu;

   public Vector3 bossPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        auxAssigns = GameObject.FindWithTag("AuxAssigns");
        Invoke(nameof(AssignLate), 0.5f);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateFleePanel()
    {
        battleMenu.SetActive(false);
        fleeMinigamePanel.SetActive(true);
        fleeMinigamePanel.GetComponent<FleeMinigame>().onMinigame = true;
    }

    private void AssignLate()
    {
        fleeMinigamePanel = auxAssigns.GetComponent<AuxiliarAssigns>().fleeMinigameObj;
        battleMenu = auxAssigns.GetComponent<AuxiliarAssigns>().battleMenuObj;
        
    }
}

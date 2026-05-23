using TMPro;
using UnityEngine;

public class PartnerAttackTextAssign : MonoBehaviour
{

    [SerializeField] private AuxiliarAssigns auxAssigns;
    [SerializeField] private GameObject player, auxAssignsObj;

    public PartnerCombat currentPartner;

    public int numBotonAtaque;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        auxAssignsObj = GameObject.FindWithTag("AuxAssigns");
        auxAssigns = auxAssignsObj.GetComponent<AuxiliarAssigns>();
        player = auxAssigns.player;
        for (int i = 0; i < this.gameObject.transform.parent.childCount; i++)
        {
            if(this.gameObject.transform.parent.GetChild(i).gameObject == this.gameObject)
            {
                numBotonAtaque = i;
                break;
            }
        }
        Debug.Log("entra en enable");
        if(this.gameObject.transform.parent.gameObject)
        ComprobarPartner(numBotonAtaque);
    }

    void ComprobarPartner(int ataque)
    {
        currentPartner = player.GetComponent<BattleController>().currentPartner;
        switch (currentPartner.partnerName)
        {
            case "Lulú":

            switch (ataque)
            {    
                case 0:

                this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Compás Flamenco";

                    break;

                case 1:

                this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Atonalidad";

                    break;

                default:
                Debug.LogError("no sabia que habia tres ataques para los compañeros, awebo \n (si no es así esto es un error xd)");
                    break;
            }

            break;

            case "Parsifal":

            switch (ataque)
            {    
                case 0:

                this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Tormenta de Acordes";

                    break;

                case 1:

                this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Derribo";

                    break;

                default:
                Debug.LogError("no sabia que habia tres ataques para los compañeros, awebo \n (si no es así esto es un error xd)");
                    break;
            }

            break;

            case "Sergey Rajmáninov":

            switch (ataque)
            {    
                case 0:

                this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Armonía";

                    break;

                case 1:

                this.gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Corillo";

                    break;

                default:
                Debug.LogError("no sabia que habia tres ataques para los compañeros, awebo \n (si no es así esto es un error xd)");
                    break;
            }

            break;

            default:
            Debug.LogError("DOU");
                    break;
        }
        
    }
}

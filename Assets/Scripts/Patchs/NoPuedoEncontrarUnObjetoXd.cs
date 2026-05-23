using UnityEngine;

public class NoPuedoEncontrarUnObjetoXd : MonoBehaviour
{
    public BattleTransitionManager objeto;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        objeto = GameObject.FindAnyObjectByType<BattleTransitionManager>();

        Debug.Log(objeto.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishGame : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.transform.GetChild(0).GetComponent<EnemyHealth>().currentHP <= 0 || this.gameObject.transform.GetChild(0).gameObject == null)
        {
            SceneManager.LoadScene(2);
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfGame : MonoBehaviour
{
    public GameObject boss;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.GetComponent<EnemyHealth>().currentHP <= 0)
        {
            SceneManager.LoadScene(2);
        }
    }
}

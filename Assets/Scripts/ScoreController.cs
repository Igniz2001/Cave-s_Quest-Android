using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreController : MonoBehaviour
{
    
    public GameObject Canvas;
    public Text Score;
    public static ScoreController instance;
    [SerializeField] public float totalScore;
    private void Awake()
    {
        if (ScoreController.instance == null)
        {
            
            //Se crea una instancia del Scorecontroller para luego ponerlo en la función
            //DontDestroyOnload que es una funcion de Unity para no destruir el objeto cuando
            //cargue una nueva escena y asi sigan conservando los punto que ha hecho el jugador
            ScoreController.instance = this;
            DontDestroyOnLoad(this.gameObject);
            DontDestroyOnLoad(this.Canvas);
            
        }
        else
        {
            Destroy(gameObject);
            Destroy(this.Canvas);
        }

    }


    void Update()
    {
        Scene scene = SceneManager.GetActiveScene();
        Score.text = "Score: " + totalScore;
        if (scene.buildIndex == 0)
        {
            totalScore = 0;
        }
    }

    public void IncreasePoints(float amount)
    {
        totalScore += amount;
    }
}

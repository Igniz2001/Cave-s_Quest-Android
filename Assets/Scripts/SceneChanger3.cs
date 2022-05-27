using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger3 : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Hero")
        {
            print("Has ganado el nivel");
            SceneManager.LoadScene("LevelPass3");
        }
    }
}

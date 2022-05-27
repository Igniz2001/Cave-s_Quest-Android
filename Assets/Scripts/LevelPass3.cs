using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPass3 : MonoBehaviour
{
    public void ExitGame()
    {
        SceneManager.LoadScene("PrincipalMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPass2 : MonoBehaviour
{
    public void ExitGame()
    {
        SceneManager.LoadScene("PrincipalMenu");
    }

    public void Next()
    {
        SceneManager.LoadScene("Level3");
    }
}

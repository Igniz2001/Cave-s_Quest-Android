using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject pauseMenu;
    public bool pause = false;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //cuando el jugador presione ESC se desplegará el meenu de pausa
        {
            if (pause)
            {
                Resume();
            }
            else
            {
                GamePause();
            }
        }
    }

    public void GamePause() // esto se encarga de desplegar el menu de pausa cuando se presiona esc o click en el boton pause
    {
        pause = true;
        Time.timeScale = 0f;
        pauseButton.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void Resume() // esto resume el juego y quita el menu de pausa
    {
        pause = false;
        Time.timeScale = 1f;
        pauseButton.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void Quit() // esto devuelve al jugador al menu inicial
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PrincipalMenu");
    }

}

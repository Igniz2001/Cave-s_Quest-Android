using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Killzone : MonoBehaviour
{

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        //En este metodo cuando el jugador o un enemigo caigan al abismo, 
        // en el caso del jugador se cargará el menu principal y destruirá el objeto de jugador
        //en el caso de que sea un enemigo, solo se destruira el objeto de enemigo
        if(other.gameObject.tag == "Hero")
        {
            Destroy(other.gameObject);
            SceneManager.LoadScene("PrincipalMenu");
        }
        else if (other.gameObject.tag == "Enemy")
        {
            Destroy (other.gameObject);
        }
    }
}

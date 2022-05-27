using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private float amount;

    private void OnTriggerEnter2D(Collider2D other)
    {   //este metodo está asociado al item ruby, que al ser tocado por un objeto 
        // de etiqueta "Hero", llamará el metodo IncreasePoints y procederá a destruir el ruby
        if (other.CompareTag("Hero"))
        {
            ScoreController.instance.IncreasePoints(amount);
            Destroy(gameObject);
        }
    }
}

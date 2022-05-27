using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinScript : MonoBehaviour
{
    // en este script se maneja todo el comportamiento del enemigo del juego
    AudioSource reproductor;
    public AudioClip death;
    public StateMachine state;
    public GameObject Knight;
    public float Speed;
    private float LastAttack;
    public float cooldown;
    private Animator Animator;
    [SerializeField] private float Life;
    [SerializeField] private Transform AttackController;
    [SerializeField] private float HitRatio;
    [SerializeField] private float HitDamage;
    public float soundEnter;
    public float deathEnter;
    
    [Header("Variables Bonitas")]
    public float attackRange = 1;
    public float followDistance = 4;
    public float escapeDistance = 6;
    public float borderRadio = 1;
    public float borderDistance = 1;

    bool isInBorder = false;
    private void Start()
    {   // Aqui se llama el animador para que las animaciones funcionen cuando se les llame en los metodos
        Animator = GetComponent<Animator>();
        reproductor = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Knight == null) return; // si el jugador no existe, entonces que omita cualquiera de las acciones aqui mostradas

        switch (state)
        {
            case StateMachine.idle:
                IdleState();
                break;
            case StateMachine.follow:
                FollowState();
                break;
            case StateMachine.attack:
                AttackState();
                break;
            case StateMachine.death:
                DeathState();
                break;
            default:
                break;
        }
        
    }

    public void Look()
    {

        Vector3 direction = Knight.transform.position - transform.position;// con esto el enemigo siempre mira al jugador
        if (direction.x >= 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        else transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f); // con la linea 26 y 27 rota al enemigo dependiendo de donde este el jugador

    }

    public void Attack() // despliega la animación, llama el metodo que causa el daño al jugador y le da un tiempo de enfriamiento
    {
        Animator.SetTrigger("enemyAttack");
        EnemyHit();
        LastAttack = Time.time;
    }

    private void Death()
    {   // Aqui se detona la muerte del enemigo 
        Destroy(gameObject);
    }

    public void IdleState() // este es el estado que adopta el enemigo cuando no hay nada a su alrededor y espera al jugador
    {
        float distance = Mathf.Abs(Knight.transform.position.x - transform.position.x);
        if (distance < followDistance)
        {
            StateChange(StateMachine.follow);
        }
    }

    public void FollowState() //este estado se adopta cuando el enemigo persigue al jugador
    {
        Look();
        EdgeLook();
        if (!isInBorder)
        {
            transform.Translate(Speed * Time.deltaTime * transform.localScale.x, 0, 0);
        }
        Animator.SetBool("running", !isInBorder);
        float distance = Mathf.Abs(Knight.transform.position.x - transform.position.x);
        if (distance < attackRange)
        {
            StateChange(StateMachine.attack);
        }
        else if (distance > escapeDistance)
        {
            StateChange(StateMachine.idle);
        }
    }

    public void AttackState()
    {
        Look();
        float distance = Mathf.Abs(Knight.transform.position.x - transform.position.x);
        if (distance < attackRange && Time.time > LastAttack + cooldown)
        {   //cuando la distancia sea menor a 1.0f, el enemigo atacará y cumplira con un tiempo de espera para el proximo ataque 
            Attack();
        }
        else if(distance > attackRange)
        {
            StateChange(StateMachine.follow);
        }
    }

    public void DeathState() //esto es solo para llenar la maquina de estados del enemigo
    {

    }

    public void StateChange(StateMachine e) //esto cambia los estados del enemigo 
    {
        if (state == StateMachine.follow)
        {
            Animator.SetBool("running", false);
        }
        state = e;
    }

    private void EdgeLook() //esto detecta los bordes de los abismos
    {

        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position + new Vector3(transform.localScale.x * borderDistance, 0, 0), borderRadio);
        isInBorder = false;
        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Border"))
            {
                isInBorder = true;
            }
        }
    }

    private void DeathSound()
    {
        reproductor.PlayOneShot(death);
    }
    public void TakeDamage(float damage)
    {   // Aqui muestra como el enemigo toma daño de parte del jugador en caso de ser golpeado
        Life -= damage;
        Animator.SetTrigger("attacked");
        if (Life <= 0)
        {
            HitDamage = 0.0f;
            Speed = 0.0f;
            Animator.SetTrigger("dying");
            Invoke(nameof(DeathSound), soundEnter);
            Invoke(nameof(Death), deathEnter);
            
        }
    }
    private void EnemyHit()
    {   //Esto hace que el enemigo le haga daño al jugador y llama el script del jugador para hacerle daño

        Collider2D[] objects = Physics2D.OverlapCircleAll(AttackController.position, HitRatio);

        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Hero"))
            {
                KnightMovement km = collider.transform.GetComponent<KnightMovement>();
                if (km != null) km.TakeDamage(HitDamage);

            }
        }
    }

    private void OnDrawGizmos()
    {   //Esto es para el enemy attack controller, la bolita verde que contiene el daño que el enemigo hará
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(AttackController.position, HitRatio);
        Gizmos.DrawWireSphere(transform.position + new Vector3(transform.localScale.x * borderDistance, 0, 0), borderRadio);
    }
}

public enum StateMachine{ //esta es la maquina de estados
    idle = 0,
    follow = 1,
    attack = 2,
    death = 3
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class KnightMovement : MonoBehaviour
{
    //en este script se hace referencia al personaje del jugador, todo lo relacionado
    // al jugador menos su ataque
    AudioSource reproductor;
    public float Speed;
    public float JumpForce;
    private Rigidbody2D Rigidbody2D;
    private float Horizontal;
    private float Vertical;
    private bool Grounded;
    private Animator Animator;
    public float Life;
    [SerializeField] AudioClip potionSound;
    [SerializeField] AudioClip rubySound;
    [SerializeField] Slider LifeSlider;
    //-----------------------------------------------------------------------------------------------
    [SerializeField] AudioClip swordSwing;
    [SerializeField] private Transform AttackController;
    [SerializeField] private Transform AttackControllerCrouch;
    public GameObject CircleCollider;
    [SerializeField] private float HitRatio;
    [SerializeField] private float HitDamage;
    [SerializeField] private float TimeBetweenHits;
    [SerializeField] private float TimeForNextHit;
    public bool canAttack = true;
    //-----------------------------------------------------------------------------------------------
    public bool agachar = false;


    void Start()
    {
        //aqui se inicializan los componentes externos, sea rigidbody, sonidos etc.
        reproductor = GetComponent<AudioSource>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        LifeSlider.maxValue = Life;
        LifeSlider.value = LifeSlider.maxValue;
    }

    void Update()
    {
        //aqui se actualiza todas las entradas del jugador a lo largo del gameplay
        
        Horizontal = CrossPlatformInputManager.GetAxis("Horizontal"); //esto se encarga del movimiento 36,38,39,41
        Vertical = CrossPlatformInputManager.GetAxis("Vertical");

        if (Horizontal < 0.0f) transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);


        if (Vertical < 0.0f && Grounded) //Con esto se agacha
        {
            agachar = true;
            CircleCollider.SetActive(false);
            Animator.SetBool("crouch",true);
            Animator.SetBool("crouchWalk", Horizontal != 0.0f);
            Speed = 35;
        }
        else
        {
            if (CircleCollider == null) return;
            agachar = false;
            CircleCollider.SetActive(true);
            Animator.SetBool("crouchWalk", false);
            Animator.SetBool("running", Horizontal != 0.0f);
            Animator.SetBool("crouch", false);
            Speed = 65;
        }





        Debug.DrawRay(transform.position, Vector3.down * 0.3f, Color.red);//esto detecta si hay suelo para generar un enfriamiento para el salto del personaje para que no sea infinito
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.3f))
        {
            Grounded = true;
        }
        else Grounded = false;

        if (CrossPlatformInputManager.GetButtonDown("Jump") && Grounded)
        {
            Jump();
        }

        if (Grounded == false)
        {
            Animator.SetBool("jumping", true);
        }
        else if(Grounded)
        {
            Animator.SetBool("jumping", false);
        }

        //--------------------------------------------------------------

        if (TimeForNextHit > 0)
        {
            TimeForNextHit -= Time.deltaTime;
        }
        if (canAttack)
        {
            if (CrossPlatformInputManager.GetButtonDown("Attack") && TimeForNextHit <= 0) //cuando el jugador presione J y se haya cumplido su tiempo de eenfriamiento, atacará
            {
                reproductor.PlayOneShot(swordSwing);
                if (agachar)
                {
                    Animator.SetTrigger("crouchAttack");
                    CrouchHit();
                    TimeForNextHit = TimeBetweenHits;
                }
                else
                {
                    Animator.SetTrigger("attacking");
                    Hit();
                    TimeForNextHit = TimeBetweenHits;
                }
            }
        }

        //-----------------------------------------------------------------



    }


    private void Jump() //esto genera el salto del personaje
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce);
    }

    private void Death() 
    {
        Destroy(gameObject);
        print("Has perdido bro");
        SceneManager.LoadScene("PrincipalMenu");
    }
    public void TakeDamage(float damage) //esto recibe el daño que le hacen al jugador
    {
        if (damage <= 0.0f)
        {
            return;
        }
        else
        {
            Life -= damage;
            LifeSlider.value = Life;
            if (agachar)
            {
                StartCoroutine(AttackStop());
            }
            else
            {
                Animator.SetTrigger("attacked");
                StartCoroutine(AttackStop());
            }
        }
        if (Life <= 0)
        {
            
            Animator.SetTrigger("dying");
            Invoke(nameof(Death),1.2f);
        }
    }

    private IEnumerator AttackStop() // esto se llama cuando el jugador es atacado para evitar que pueda atacar en ese momento
    {
        print("me llamaron");
        canAttack = false;
        yield return new WaitForSeconds(0.6f);
        canAttack = true;
    }

    private IEnumerator AttackBoost()
    {
        print("se llamo");
        HitDamage = 150;
        yield return new WaitForSeconds(10.0f);
        HitDamage = 50;
    }
    private void OnTriggerEnter2D(Collider2D collision) //esto maneja la barra de vida cuando la pocion de vida es recogida
    {
        if (collision.gameObject.tag == "Ruby")
        {
            reproductor.PlayOneShot(rubySound);
        }
        else if (collision.gameObject.tag == "Potion")
        {
            reproductor.PlayOneShot(potionSound);
            if (Life > 0 && Life <= 800)
            {
                Life += 200;
                LifeSlider.value = Life;
                Destroy(collision.gameObject);
            }
            else if (Life == 1000)
            {
                Life += 0;
                Destroy(collision.gameObject);
            }
            else if (Life >= 850)
            {
                Life = 1000;
                LifeSlider.value = Life;
                Destroy(collision.gameObject);
            }
        }
        else if (collision.gameObject.tag == "Booster")
        {
            reproductor.PlayOneShot(potionSound);
            StartCoroutine(AttackBoost());
            Destroy(collision.gameObject);
        }
    }

    private void Hit() // cuando identifique un enemigo con la etiqueta enemy, mandará una referencia al script del enemigo para causarle daño
    {

        Collider2D[] objects = Physics2D.OverlapCircleAll(AttackController.position, HitRatio);

        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.transform.GetComponent<GoblinScript>().TakeDamage(HitDamage);

            }
            else if (collider.CompareTag("Boss"))
            {
                collider.transform.GetComponent<BossScript>().TakeDamage(HitDamage);

            }
        }
    }

    private void CrouchHit() // cuando identifique un enemigo con la etiqueta enemy, mandará una referencia al script del enemigo para causarle daño
    {

        Collider2D[] objects = Physics2D.OverlapCircleAll(AttackControllerCrouch.position, HitRatio);

        foreach (Collider2D collider in objects)
        {
            if (collider.CompareTag("Enemy"))
            {
                collider.transform.GetComponent<GoblinScript>().TakeDamage(HitDamage);

            }
        }
    }

    private void OnDrawGizmos() // se encarga de generar el circulo que será el rango de ataque del jugador
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackController.position, HitRatio);
        Gizmos.DrawWireSphere(AttackControllerCrouch.position, HitRatio);
    }
    private void FixedUpdate()
    {
       Rigidbody2D.velocity = new Vector2(Horizontal * Speed * Time.fixedDeltaTime, Rigidbody2D.velocity.y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        ALIVE,
        DEAD
    }
    public PlayerState playerState = PlayerState.ALIVE;
    public enum ArmorState
    {
        SHIELD_UP,
        SHIELD_LESS
    }
    public ArmorState armorState = ArmorState.SHIELD_LESS;
    public enum DashState
    {
        NOT_DASH,
        DASH_BEGIN,
        IN_DASH,
        DASH_END
    }
    private DashState dashState = DashState.NOT_DASH;
    public float dashSpeed = 10;
    private Rigidbody2D rigidBody2D;
    public float playerVelocity = 10;
    public float shieldVelocity = 10;
    public Transform jumpPosition;
    public float raycastRadius;//rayon de détection
    public LayerMask mask;
    public float jumpForce = 10;
    [SerializeField] GameObject shieldPrefab;
    public bool spawnShield = false;
    private bool canMove = true;
    private float direction;
    public bool shieldOn = false;

    private Animator animator;
    // Start is called before the first frame update
    
    void Start()
    {
        //Input.GetJoystickNames;
        animator = GetComponentInChildren<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    
    void Update()
    {
        animator.SetFloat("velocity",Mathf.Abs(rigidBody2D.velocity.x));
        //mouvement gauche droite
        
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        shieldOn = false;
        if (canMove == true)
        {
            Vector2 velocity = new Vector2(horizontalInput * playerVelocity, rigidBody2D.velocity.y);
            rigidBody2D.velocity = velocity;
         //inversion du sprite du personnage
                Vector3 scale = transform.localScale;
                if (rigidBody2D.velocity.x > 0)
                {
                    scale.x = Mathf.Abs(scale.x);
                }
                else if (rigidBody2D.velocity.x < 0)
                {
                    scale.x = -Mathf.Abs(scale.x);
                }
                transform.localScale = scale;
        }
        else 
        {
            //inversion du sprite du personnage
            Vector3 scale = transform.localScale;
            if (horizontalInput > 0)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else if (horizontalInput < 0)
            {
                scale.x = -Mathf.Abs(scale.x);
            }
            transform.localScale = scale;
        }
       

        
        //saut
        bool canJump = Physics2D.OverlapCircle(jumpPosition.position, raycastRadius, mask);
        if (canJump && Input.GetButtonDown("Jump"))//si appuye sur boutton saut
        {
            rigidBody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);//donne une impulsion verticale
        }
        //bouclier
        //a mettre dans le code du joueur
        if(Input.GetButtonDown("Fire1")&&(spawnShield != true))
        {
            
            if(transform.localScale.x > 0)
            {
                GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x + 5, transform.position.y, transform.position.z), Quaternion.identity);
                monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(shieldVelocity, verticalInput)*shieldVelocity;
            }
            if(transform.localScale.x < 0)
            {
                GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x - 5, transform.position.y, transform.position.z), Quaternion.identity);
                monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-shieldVelocity, verticalInput)*shieldVelocity;
            }
                spawnShield = true;
        }
        //levée de bouclier

        if (Input.GetButton("Shield")&&(spawnShield == false))
        {
            armorState = ArmorState.SHIELD_UP;
            canMove = false;
            Debug.Log("boucliers");
            GetComponent<CapsuleCollider2D>().enabled = true;
            shieldOn = true;
        }
        else
        {
            armorState = ArmorState.SHIELD_LESS;
            canMove = true;
            GetComponent<CapsuleCollider2D>().enabled = false;
            shieldOn = false;
        }
        //dash
        if(Input.GetButtonDown("Fire2")&& (Input.GetButton("Shield")&&(spawnShield == false)))
        {
            canMove = true;

            dashState = DashState.DASH_BEGIN;


            Debug.Log("dash");
            
        }
        Dash();
    }
    void Dash()
    {
        switch (dashState)
        {
            case DashState.DASH_BEGIN:
                
                if(transform.localScale.x > 0)
                {
                    rigidBody2D.velocity = new Vector2(dashSpeed, 0);
                }
                if(transform.localScale.x < 0)
                {
                    rigidBody2D.velocity = new Vector2(-dashSpeed, 0);
                }
                dashState = DashState.IN_DASH;
                break;
            case DashState.IN_DASH:
                dashState = DashState.DASH_END;
                break;
            case DashState.DASH_END:
                dashState = DashState.NOT_DASH;
                break;
        }
    }
}

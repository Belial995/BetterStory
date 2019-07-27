using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

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
    public bool shieldUP = false;
    public bool shieldDown = false;
    public bool shieldThrow = false;
    public int playerIndex;
    //public GameObject playerShield;
    public CapsuleCollider2D colliderShieldTop;
    public CapsuleCollider2D colliderShieldDown;
    public CapsuleCollider2D colliderShieldFlanc;
    public InputDevice playerDevice;

    private Animator animator;
    // Start is called before the first frame update

    [SerializeField]
    private gameManager _gm;
    

    void Start()
    {
        //Input.GetJoystickNames;
        animator = GetComponentInChildren<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        colliderShieldTop.enabled = false;
        colliderShieldDown.enabled = false;
        colliderShieldFlanc.enabled = false;

        //InputManager.OnDeviceAttached += AssignPlayer;

        //cherche Game Manager
        _gm = GameObject.Find("ControlerManager").GetComponent<gameManager>();
    }
    // Update is called once per frame
    
    void Update()
    { 
        //dash
        if ((playerDevice.GetControl(InputControlType.LeftTrigger).IsPressed)&&(playerDevice.GetControl(InputControlType.LeftBumper).WasReleased)&&(spawnShield == false))
        {
            Debug.Log("dash");
            dashState = DashState.DASH_BEGIN;
            Dash();

        }



        if (playerDevice == null)
            return;

        if(playerDevice.GetControl(InputControlType.Action1).IsPressed)
        {
            Debug.Log("Coucou j'appuie sur X");
        }
       
        Debug.Log(playerIndex);
        //if(playerIndex == 1)
        
            animator.SetFloat("velocity", Mathf.Abs(rigidBody2D.velocity.x));
            //mouvement gauche droite

            float horizontalInput = playerDevice.LeftStickX;
            float verticalInput = playerDevice.LeftStickY;
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
            if (canJump && (playerDevice.GetControl(InputControlType.Action1).IsPressed))//si appuye sur boutton saut
            {
                rigidBody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);//donne une impulsion verticale
            }
            //bouclier
            //a mettre dans le code du joueur
            if ( (playerDevice.GetControl(InputControlType.RightBumper).IsPressed) && (spawnShield != true))
            {

                shieldThrow = true;
                if (transform.localScale.x > 0)
                {
                    GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x + 5, transform.position.y, -2), Quaternion.identity);
                    monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(shieldVelocity, verticalInput) * shieldVelocity;
                    monObject.GetComponent<shieldMovement>().shieldPlayerId = playerIndex;

                }
                if (transform.localScale.x < 0)
                {
                    GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x - 5, transform.position.y, -2), Quaternion.identity);
                    monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-shieldVelocity, verticalInput) * shieldVelocity;
                    monObject.GetComponent<shieldMovement>().shieldPlayerId = playerIndex;
            }
                spawnShield = true;
                
            }
        //levée de bouclier
        if (dashState == DashState.NOT_DASH)
        {
            if ((playerDevice.GetControl(InputControlType.LeftBumper).IsPressed) && (spawnShield == false))
            {
                rigidBody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

                animator.SetBool("ShieldFlanc", true);
                armorState = ArmorState.SHIELD_UP;
                canMove = false;
                Debug.Log("boucliers");
                //playerShield.SetActive(true);
                shieldOn = true;
                colliderShieldFlanc.enabled = true;
                colliderShieldTop.enabled = false;
                colliderShieldDown.enabled = false;
                if (playerDevice.LeftStickY > 0.2f)
                {
                    Debug.Log("prout");
                    colliderShieldTop.enabled = true;
                    colliderShieldDown.enabled = false;
                    colliderShieldFlanc.enabled = false;
                    //playerShield.SetActive(false);
                    shieldDown = false;
                    shieldUP = true;
                }
                else if (playerDevice.LeftStickY < -0.2f)
                {
                    //playerShield.SetActive(false);
                    colliderShieldDown.enabled = true;
                    colliderShieldTop.enabled = false;
                    colliderShieldFlanc.enabled = false;
                    shieldDown = true;
                    shieldUP = false;
                }
                else
                {
                    colliderShieldTop.enabled = false;
                    colliderShieldDown.enabled = false;
                    colliderShieldFlanc.enabled = true;
                    shieldDown = false;
                    shieldUP = false;
                    //playerShield.SetActive(false);
                }

            }
            else
            {
                rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                colliderShieldTop.enabled = false;
                colliderShieldDown.enabled = false;
                colliderShieldFlanc.enabled = false;
                animator.SetBool("ShieldFlanc", false);
                armorState = ArmorState.SHIELD_LESS;
                canMove = true;
                //playerShield.SetActive(false);
                shieldOn = false;
                shieldDown = false;
                shieldUP = false;
            }
        }           
        animator.SetBool("shieldDown", shieldDown);
        animator.SetBool("shieldUP", shieldUP);
        animator.SetBool("shieldThrow", shieldThrow);
    }   
                   
    
    void Dash()
    {
        canMove = false;
        dashState = DashState.DASH_BEGIN;
        LastDashPressed = Time.time;
    }

    float timeToDash = 0.1f;
    float LastDashPressed;

    private void FixedUpdate()
    {

        switch (dashState)
        {
            case DashState.DASH_BEGIN:
               
                rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                if (transform.localScale.x > 0)
                {

                    rigidBody2D.velocity = new Vector2(dashSpeed, 0);

                }
                if (transform.localScale.x < 0)
                {
                    rigidBody2D.velocity = new Vector2(-dashSpeed, 0);

                }

                dashState = DashState.IN_DASH;

                break;
            case DashState.IN_DASH:
                if (Time.time >= LastDashPressed + timeToDash)
                {
                    dashState = DashState.DASH_END;
                }
                break;
            case DashState.DASH_END:
                canMove = true;
                dashState = DashState.NOT_DASH;
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == ("shield") && (collision.gameObject.GetComponent<shieldMovement>().ofGround == false))
        {
            if(collision.GetComponent<shieldMovement>().shieldPlayerId == 0)
            {

                Debug.Log("Marque 1 point!");
                _gm.scorePlayer1++;

            }

            if (collision.GetComponent<shieldMovement>().shieldPlayerId == 1)
            {

                Debug.Log("Marque 2 point!");
                _gm.scorePlayer2++;

            }
            if (collision.GetComponent<shieldMovement>().shieldPlayerId == 2 )
            {

                Debug.Log("Marque 1 point!");
                _gm.scorePlayer3++;

            }
            if (collision.GetComponent<shieldMovement>().shieldPlayerId == 3)
            {

                Debug.Log("Marque 1 point!");
                _gm.scorePlayer4++;

            }


            Destroy(gameObject);


        }
    }


}

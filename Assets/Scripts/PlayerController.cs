﻿using System.Collections;
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
    public int playerIndex;
    public GameObject shield;
    public CapsuleCollider2D colliderShieldTop;
    public CapsuleCollider2D colliderShieldDown;


    private Animator animator;
    // Start is called before the first frame update


    private void AssignPlayer()
    {
        Debug.Log("COucoUCOuouc");
    } 

    void Start()
    {
        //Input.GetJoystickNames;
        animator = GetComponentInChildren<Animator>();
        rigidBody2D = GetComponent<Rigidbody2D>();
        colliderShieldTop.enabled = false;
        colliderShieldDown.enabled = false;

        //InputManager.OnDeviceAttached += AssignPlayer;
    }
    // Update is called once per frame
    
    void Update()
    {

        InputDevice playerDevice = InputManager.ActiveDevice;

        if(playerDevice.GetControl(InputControlType.Action1).IsPressed)
        {
            Debug.Log("Coucou j'appuie sur X");
        }
        

        foreach (string n in Input.GetJoystickNames())
        {
            Debug.Log(n);
        }

        Debug.Log(playerIndex);
        if(playerIndex == 1)
        {
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

                if (transform.localScale.x > 0)
                {
                    GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x + 5, transform.position.y, transform.position.z), Quaternion.identity);
                    monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(shieldVelocity, verticalInput) * shieldVelocity;
                }
                if (transform.localScale.x < 0)
                {
                    GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x - 5, transform.position.y, transform.position.z), Quaternion.identity);
                    monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-shieldVelocity, verticalInput) * shieldVelocity;
                }
                spawnShield = true;
            }
            //levée de bouclier

            if ((playerDevice.GetControl(InputControlType.LeftBumper).IsPressed) && (spawnShield == false))
            {
                animator.SetBool("ShieldFlanc", true);
                armorState = ArmorState.SHIELD_UP;
                canMove = false;
                Debug.Log("boucliers");
                shield.SetActive(true);
                shieldOn = true;
                if(playerDevice.LeftStickY > 0.2f)
                {
                    Debug.Log("prout");
                    colliderShieldTop.enabled = true;
                    colliderShieldDown.enabled = false;
                    shieldDown = false;
                    shieldUP = true;
                }
                else  if (playerDevice.LeftStickY < -0.2f)
                {
                    
                    colliderShieldDown.enabled = true;
                    colliderShieldTop.enabled = false;
                    shieldDown = true;
                    shieldUP = false;
                }
                else
                {
                    colliderShieldTop.enabled = false;
                    colliderShieldDown.enabled = false;
                    shieldDown = false;
                    shieldUP = false;
                }
            }
            else
            {
                colliderShieldTop.enabled = false;
                colliderShieldDown.enabled = false;
                animator.SetBool("ShieldFlanc", false);
                armorState = ArmorState.SHIELD_LESS;
                canMove = true;
                shield.SetActive(false);
                shieldOn = false;
            }
            
            //dash
            if((playerDevice.GetControl(InputControlType.LeftBumper).IsPressed)&& (playerDevice.GetControl(InputControlType.LeftTrigger).WasPressed)&&(spawnShield == false))
            {
                canMove = true;
                dashState = DashState.DASH_BEGIN;
                Dash();
                Debug.Log("dash");
                                            
            }                                       
        }
        animator.SetBool("shieldDown", shieldDown);
        animator.SetBool("shieldUP", shieldUP);
    }   
                   
    /*if(playerIndex == 2)
                {
                    animator.SetFloat("velocity", Mathf.Abs(rigidBody2D.velocity.x));
                    //mouvement gauche droite

                    float horizontalInput = Input.GetAxis("Horizontal2");
                    float verticalInput = Input.GetAxis("Vertical2");
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
                    if (canJump && Input.GetButtonDown("Jump2"))//si appuye sur boutton saut
                    {
                        rigidBody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);//donne une impulsion verticale
                    }
                    //bouclier
                    //a mettre dans le code du joueur
                    if (Input.GetButtonDown("Fire1(2)") && (spawnShield != true))
                    {

                        if (transform.localScale.x > 0)
                        {
                            GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x + 5, transform.position.y, transform.position.z), Quaternion.identity);
                            monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                            monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(shieldVelocity, verticalInput) * shieldVelocity;
                        }
                        if (transform.localScale.x < 0)
                        {
                            GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x - 5, transform.position.y, transform.position.z), Quaternion.identity);
                            monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                            monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-shieldVelocity, verticalInput) * shieldVelocity;
                        }
                        spawnShield = true;
                    }
                    //levée de bouclier

                    if (Input.GetButton("Shield2") && (spawnShield == false))
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
                    if (Input.GetButtonDown("Fire2(2)") && (Input.GetButton("Shield2")) && (spawnShield == false))
                    {
                        canMove = true;

                        dashState = DashState.DASH_BEGIN;


                        Debug.Log("dash");

                    }
            

                }
               /* if (playerIndex == 3)
                    {
                        animator.SetFloat("velocity", Mathf.Abs(rigidBody2D.velocity.x));
                        //mouvement gauche droite

                        float horizontalInput = Input.GetAxis("Horizontal3");
                        float verticalInput = Input.GetAxis("Vertical3");
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
                        if (canJump && Input.GetButtonDown("Jump3"))//si appuye sur boutton saut
                        {
                            rigidBody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);//donne une impulsion verticale
                        }
                        //bouclier
                        //a mettre dans le code du joueur
                        if (Input.GetButtonDown("Fire1(3)") && (spawnShield != true))
                        {

                            if (transform.localScale.x > 0)
                            {
                                GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x + 5, transform.position.y, transform.position.z), Quaternion.identity);
                                monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                                monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(shieldVelocity, verticalInput) * shieldVelocity;
                            }
                            if (transform.localScale.x < 0)
                            {
                                GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x - 5, transform.position.y, transform.position.z), Quaternion.identity);
                                monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                                monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-shieldVelocity, verticalInput) * shieldVelocity;
                            }
                            spawnShield = true;
                        }
                        //levée de bouclier

                        if (Input.GetButton("Shield3") && (spawnShield == false))
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
                        if (Input.GetButtonDown("Fire2(3)") && (Input.GetButton("Shield3")) && (spawnShield == false))
                        {
                            canMove = true;

                            dashState = DashState.DASH_BEGIN;


                            Debug.Log("dash");

                        }

                    }
                if (playerIndex == 4)
                    {
                        animator.SetFloat("velocity", Mathf.Abs(rigidBody2D.velocity.x));
                        //mouvement gauche droite

                        float horizontalInput = Input.GetAxis("Horizontal4");
                        float verticalInput = Input.GetAxis("Vertical4");
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
                        if (canJump && Input.GetButtonDown("Jump4"))//si appuye sur boutton saut
                        {
                            rigidBody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);//donne une impulsion verticale
                        }
                        //bouclier
                        //a mettre dans le code du joueur
                        if (Input.GetButtonDown("Fire1(4)") && (spawnShield != true))
                        {

                            if (transform.localScale.x > 0)
                            {
                                GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x + 5, transform.position.y, transform.position.z), Quaternion.identity);
                                monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                                monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(shieldVelocity, verticalInput) * shieldVelocity;
                            }
                            if (transform.localScale.x < 0)
                            {
                                GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x - 5, transform.position.y, transform.position.z), Quaternion.identity);
                                monObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                                monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-shieldVelocity, verticalInput) * shieldVelocity;
                            }
                            spawnShield = true;
                        }
                        //levée de bouclier

                        if (Input.GetButton("Shield4") && (spawnShield == false))
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
                        if (Input.GetButtonDown("Fire2(4)") && (Input.GetButton("Shield4")) && (spawnShield == false))
                        {
                            canMove = true;

                            dashState = DashState.DASH_BEGIN;


                            Debug.Log("dash");

                        }
                        Dash();
            }
            */
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
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == ("shield") && (collision.gameObject.GetComponent<shieldMovement>().ofGround == false))
        {
            Destroy(gameObject);

        }
    }


}

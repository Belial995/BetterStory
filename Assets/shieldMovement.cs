using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldMovement : MonoBehaviour
{
    public float shieldVelocity;
    private bool isFlying;
    public bool ofGround;

    

    // Start is called before the first frame update
    void Start()
    {
        
        isFlying = true;
        ofGround = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (isFlying)
        {
            
            float verticalInput = Input.GetAxis("Vertical");
            GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
        }
       


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "sol")
        {
            isFlying = false;
            Debug.Log("Bouclier touche!");
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            GetComponent<Rigidbody2D>().gravityScale = 10.0f;
            ofGround = true;

        }
        if((collision.gameObject.tag == "shield") || (collision.gameObject.tag == "player"))
        {
            isFlying = false;
            Debug.Log("Bouclier touche!");
            GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
            GetComponent<Rigidbody2D>().gravityScale = 10.0f;
            ofGround = true;


        }
        if ((collision.gameObject.tag == "player") && (ofGround == false))
        {
            if(collision.gameObject.GetComponent<PlayerController>().armorState == PlayerController.ArmorState.SHIELD_UP)
            {
                Debug.Log("Touche bouclier");
                ofGround = true;
                Destroy(gameObject);
                
            }
            else
            {
                ofGround = true;
                Debug.Log("joueur mort");
            }
            
        }

        if ((collision.gameObject.tag == "player") && (ofGround == true)&&(collision.gameObject.GetComponent<PlayerController>().spawnShield == true))
        {
            
            collision.gameObject.GetComponent<PlayerController>().spawnShield = false;
            Destroy(gameObject);
            Debug.Log("test");
            
        }






    }
}

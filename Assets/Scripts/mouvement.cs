using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouvement : MonoBehaviour
{
    Rigidbody2D rigid;
    public float MaxJump;
    private bool AuSol = false;
    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        
        //transform.position = new Vector3(10,2,0);
    }

    // Update is called once per frame
    void Update()
    {
        
        
        //droite
        if(Input.GetKey(KeyCode.D))
        {
            rigid.velocity = new Vector2(10.0f,rigid.velocity.y);
            //transform.position += new Vector3(0.1f,0,0);
        }
        //gauche
        if (Input.GetKey(KeyCode.A))
        {
            rigid.velocity = new Vector2(-10.0f,rigid.velocity.y);
            //transform.position -= new Vector3(0.1f, 0, 0);
        }
        //jump
        if(Input.GetKey(KeyCode.W) && AuSol == true)
        {
            Jump();


        }
        //bas
        if (Input.GetKey(KeyCode.S))
        {
            rigid.velocity = new Vector2(0.0f,-15.0f);
            //transform.position -= new Vector3(0.1f, 0, 0);
        }



    }
    //jump
    void Jump()
    {
        rigid.velocity = new Vector2(0.0f, MaxJump);
    }
    void OnCollisionEnter2D(Collision2D collider)
    {
        if(collider.gameObject.tag == "sol")
        {
            Debug.Log("Je peux sauter");
            AuSol = true;
        }
    }
    void OnCollisionExit2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "sol")
        {
            Debug.Log("Je ne peux pas sauter");
            AuSol = false;
        }
    }
}

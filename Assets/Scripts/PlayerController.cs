using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidBody2D;
    public float playerVelocity = 10;
    public float shieldVelocity = 10;
    public Transform jumpPosition;
    public float raycastRadius;//rayon de détection
    public LayerMask mask;
    public float jumpForce = 10;
    [SerializeField] GameObject shieldPrefab;
    public bool spawnShield = false;
    private float direction;
    // Start is called before the first frame update
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        //mouvement gauche droite
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector2 velocity = new Vector2(horizontalInput * playerVelocity, rigidBody2D.velocity.y);
        rigidBody2D.velocity = velocity;
        //inversion du sprite du personnage
        Vector3 scale = transform.localScale;
        if (velocity.x > 0)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else if (velocity.x < 0)
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
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
                monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(shieldVelocity, verticalInput)*shieldVelocity;
            }
            if(transform.localScale.x < 0)
            {
                GameObject monObject = Instantiate(shieldPrefab, new Vector3(transform.position.x - 5, transform.position.y, transform.position.z), Quaternion.identity);
                monObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-shieldVelocity, verticalInput)*shieldVelocity;
            }
                spawnShield = true;
        }
        //dash
        if(Input.GetButtonDown("enter"))
        {
            if(transform.localScale.x > 0)
            {

            }
            if(transform.localScale.x < 0)
            {

            }
        }
    }
}

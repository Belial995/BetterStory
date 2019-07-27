using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teletranspotration : MonoBehaviour
{
    public Vector2 position;
    Vector2 velo;
    private void OnTriggerEnter2D(Collider2D collision)
    {
            collision.gameObject.transform.position = position;
        print(collision.gameObject.GetComponent<Rigidbody2D>().velocity);
    }
}

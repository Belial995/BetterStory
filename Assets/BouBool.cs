using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bouBool : MonoBehaviour
{
    [SerializeField] bool BoutonAppuie = false;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
     if ()
        {
            BoutonAppuie = true;
        }

     if (BoutonAppuie == true)
     {
            animator.SetBool("BouBool", true);
     }
         

        
    }
}

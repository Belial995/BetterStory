using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameManager : MonoBehaviour
{
    [SerializeField]
    public float scorePlayer1;
    public float scorePlayer2;
    public float scorePlayer3;
    public float scorePlayer4;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("gamemanager");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
             SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

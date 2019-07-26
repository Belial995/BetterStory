using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[System.Serializable]
public struct PlayerToController
{
    public int playerId;
    public InputDevice device;
}

public class ControllerManager : MonoBehaviour
{

    public List<PlayerToController> controllerConnected;
    public int NumManettes = 0;


    [SerializeField] GameObject playerPrefab;
   
    // Start is called before the first frame update
    void Start()
    {
        InputManager.OnDeviceAttached += AssignPlayer;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AssignPlayer(InputDevice inputDevice)
    {
        Debug.Log("Coucou");
        PlayerToController newPlayerController;
        newPlayerController.playerId = NumManettes;
        newPlayerController.device = inputDevice;
        controllerConnected.Add(newPlayerController);
        
        PlayerController newPlayerObj = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
        newPlayerObj.playerDevice = inputDevice;
        newPlayerObj.playerIndex = NumManettes;

        NumManettes++;
    }
}

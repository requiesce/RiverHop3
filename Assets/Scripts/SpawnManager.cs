using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnManager : NetworkBehaviour //MonoBehaviour
{
    public GameObject[] lilyPadObjs = null;

    //void Start()  
    public override void OnStartServer()
    {
        InvokeRepeating("SpawnLilyPad", 2f, 5f);
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }


    void SpawnLilyPad()
    {
        foreach (GameObject lilyPad in lilyPadObjs)
        {
            GameObject tempLilypad = Instantiate(lilyPad);
            NetworkServer.Spawn(tempLilypad);
        }
    }
}
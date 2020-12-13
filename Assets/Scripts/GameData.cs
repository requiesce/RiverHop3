using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static float GamePlayStart { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        //Debug.Log(Time.time);
        //Debug.Log(gamePlayStart);
        Debug.Log("Player started at " + GamePlayStart + " seconds.");
    }
}

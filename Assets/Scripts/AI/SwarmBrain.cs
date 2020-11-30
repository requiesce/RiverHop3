using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmBrain : MonoBehaviour
{
    // while it has hive, patrol around hive
    // when no hive, pursue player

    private bool hasHave = true;
    private Patrol patrol = null;
    private Bot bot = null;
    
    // Start is called before the first frame update
    void Start()
    {
        patrol = GetComponent<Patrol>();
        bot = GetComponent<Bot>();
        HivePickUp.HivePickedUp += HiveTaken;
    }

    void HiveTaken()
    {
        hasHave = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasHave)
        {
            //patrol.PatrolWaypoints();
        }
        else
        {
            bot.Pursue();
        }
    }
}

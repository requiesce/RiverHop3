using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody rbPlayer;
    private Vector3 direction = Vector3.zero;
    public float speed = 10f;
    public GameObject[] spawnPoints = null;

    void Start()
    {
        if (!isLocalPlayer) return;

        rbPlayer = GetComponent<Rigidbody>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
    }


    void Update()
    {
        if (!isLocalPlayer) return;

        float horMove = Input.GetAxis("Horizontal");
        float verMove = Input.GetAxis("Vertical");

        direction = new Vector3(horMove, 0, verMove);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, direction * 10);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, rbPlayer.velocity * 5);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(5, 5, 5));
    }


    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        rbPlayer.AddForce(direction * speed, ForceMode.Force);

        if (transform.position.z > 40)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 40);
        }
        else if (transform.position.z < -40)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -40);
        }
    }


    private void Respawn()
    {
        int index = 0;
        while(Physics.CheckBox(spawnPoints[index].transform.position, new Vector3(1.5f,1.5f,1.5f)))
        {
            index++;
        }
        //Debug.Log("index of spawn point: " + index);
        rbPlayer.MovePosition(spawnPoints[index].transform.position);
        rbPlayer.velocity = Vector3.zero;
    }


    private void OnTriggerExit(Collider other)
    {
        if (!isLocalPlayer) return;

        if (other.CompareTag("hazard"))
        {
            Respawn();
        }
    }
}
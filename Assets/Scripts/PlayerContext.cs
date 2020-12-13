using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public abstract class PlayerState
{
    protected NetworkBehaviour thisObject;
    protected string stateName;
    protected GameObject player;

    protected PlayerState(NetworkBehaviour thisObj)
    {
        thisObject = thisObj;
        player = thisObject.gameObject;
    }

    public abstract void Start();
    public abstract void Update();
    public abstract void FixedUpdate();
    public abstract void OnCollisionEnter(Collision collision);
    public abstract void OnTriggerEnter(Collider other);
    public abstract void OnTriggerExit(Collider other);
}

public class RiverState : PlayerState
{
    private Rigidbody rbPlayer;
    private Vector3 direction = Vector3.zero;
    public float speed = 20f;
    public GameObject[] spawnPoints = null;

    public RiverState(NetworkBehaviour thisObj) : base(thisObj)
    {
        stateName = "RiverLevel";
        GameData.GamePlayStart = Time.time;
    }

    public override void Start()
    {
        rbPlayer = player.GetComponent<Rigidbody>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
    }


    public override void Update()
    {
        float horMove = Input.GetAxis("Horizontal");
        float verMove = Input.GetAxis("Vertical");

        direction = new Vector3(horMove, 0, verMove);
    }


    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(player.transform.position, direction * 10);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(player.transform.position, rbPlayer.velocity * 5);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireCube(player.transform.position, new Vector3(5, 5, 5));
    }*/


    public override void FixedUpdate()
    {
        rbPlayer.AddForce(direction * speed, ForceMode.Force);

        if (player.transform.position.z > 40)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 40);
        }
        else if (player.transform.position.z < -40)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -40);
        }
    }


    private void Respawn()
    {
        int index = 0;
        while (Physics.CheckBox(spawnPoints[index].transform.position, new Vector3(1.5f, 1.5f, 1.5f)))
        {
            index++;
        }
        //Debug.Log("index of spawn point: " + index);
        rbPlayer.MovePosition(spawnPoints[index].transform.position);
        rbPlayer.velocity = Vector3.zero;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("exit"))
        {
            NetworkManager networkManager = GameObject.Find("networkManager").GetComponent<NetworkManager>();
            networkManager.ServerChangeScene("ForestLevel");
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("hazard"))
        {
            Respawn();
        }
    }
}

public class ForestState : PlayerState
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    Rigidbody rgBody = null;
    float trans = 0;
    float rotate = 0;
    private Animator anim;
    private Camera camera;
    private Transform lookTarget;

    public delegate void DropHive(Vector3 pos);
    public static event DropHive DroppedHive;

    public ForestState(NetworkBehaviour thisObj) : base(thisObj)
    {
        stateName = "ForestLevel";
    }

    public override void Start()
    {
        player.transform.position = new Vector3(-20, 0.5f, -10);

        Transform rabbit = player.transform.Find("Rabbit");
        rabbit.transform.localEulerAngles = Vector3.zero;
        rabbit.transform.localScale = Vector3.one;

        rgBody = player.GetComponent<Rigidbody>();
        anim = player.GetComponentInChildren<Animator>();
        camera = player.GetComponentInChildren<Camera>();
        camera.enabled = true;
        lookTarget = GameObject.Find("headAimTarget").transform;
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DroppedHive?.Invoke(player.transform.position + (player.transform.forward * 10));
        }

        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translation = Input.GetAxis("Vertical");// * speed;
        float rotation = Input.GetAxis("Horizontal");// * rotationSpeed;

        anim.SetFloat("speed", translation);

        /*// Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Move translation along the object's z-axis
        transform.Translate(0, 0, translation);

        // Rotate around our y-axis
        transform.Rotate(0, rotation, 0);*/
        trans += translation;
        rotate += rotation;
    }

    public override void FixedUpdate()
    {
        Vector3 rot = player.transform.rotation.eulerAngles;
        rot.y += rotate * rotationSpeed * Time.deltaTime;
        rgBody.MoveRotation(Quaternion.Euler(rot));
        rotate = 0;

        Vector3 move = player.transform.forward * trans * speed;
        move.y = rgBody.velocity.y;
        rgBody.velocity = move; // * Time.deltaTime;

        trans = 0;
    }


    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("hazard"))
        {
            anim.SetTrigger("died");
            thisObject.StartCoroutine(ZoomOut());
        }
        else //if(!collision.collider.CompareTag("ground"))
        {
            anim.SetTrigger("twitchLeftEar");
        }
    }

    IEnumerator ZoomOut()
    {
        const int ITERATIONS = 25;
        for (int z = 0; z < ITERATIONS; z++)
        {
            camera.transform.Translate(camera.transform.forward * -1 * 15.0f / ITERATIONS);
            yield return new WaitForSeconds(1.0f / ITERATIONS);
        }
    }


    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("hazard"))
        {
            //lookTarget.position = other.transform.position;
            thisObject.StartCoroutine(LookAndLookAway(lookTarget.position, other.transform.position));
        }
        else if (other.CompareTag("exit"))
        {
            NetworkManager networkManager = GameObject.Find("networkManager").GetComponent<NetworkManager>();
            networkManager.ServerChangeScene("EndScene");
        }
   
    }

    public override void OnTriggerExit(Collider other)
    {
        //throw new System.NotImplementedException();
    }

    private IEnumerator LookAndLookAway(Vector3 targetPos, Vector3 hazardPos)
    {
        Vector3 targetDir = targetPos - player.transform.position;
        Vector3 hazardDir = hazardPos - player.transform.position;

        float angle = Vector2.SignedAngle(new Vector2(targetPos.x, targetPos.z), new Vector2(hazardPos.x, hazardPos.z));

        const int INTERVALS = 20;
        const float INTERVAL = 0.5f / INTERVALS;

        float angleInterval = angle / INTERVALS;

        for (int i = 0; i < INTERVALS; i++)
        {
            lookTarget.RotateAround(player.transform.position, Vector3.up, /*-*/angleInterval);
            yield return new WaitForSeconds(INTERVAL);
        }
        for (int i = 0; i < INTERVALS; i++)
        {
            lookTarget.RotateAround(player.transform.position, Vector3.up, /**/-angleInterval);
            yield return new WaitForSeconds(INTERVAL);
        }
    }
}

public class PlayerContext : NetworkBehaviour
{
    PlayerState currentState;

    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer) return;

        if(SceneManager.GetActiveScene().name == "RiverLevel")
        {
            currentState = new RiverState(this);
        }
        else if (SceneManager.GetActiveScene().name == "ForestLevel")
        {
            currentState = new ForestState(this);
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        if (currentState != null)
        {
            currentState.Start();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        currentState.Update();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        currentState.FixedUpdate();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isLocalPlayer) return;

        currentState.OnCollisionEnter(collision);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer) return;

        currentState.OnTriggerEnter(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (!isLocalPlayer) return;

        currentState.OnTriggerExit(other);
    }
}
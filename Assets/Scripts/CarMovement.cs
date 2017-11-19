using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {
    private string key = "Fire1";
    private GameObject player;
    private Rigidbody rigidBody;
    private BasicMovement playerMove;

    private Vector3 moveSpeed;
    private Vector3 groundNormal;
    public float minCarDistance =3f;
    private bool onRide = false;
    public float speed=10f;
    public float gravity = 0.9f;
    // Use this for initialization
    void Start () {
        player = GameObject.Find("edgarMCSkin");
        playerMove = player.GetComponent<BasicMovement>();
        rigidBody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        if (!onRide) {
            if (Vector3.Distance(transform.position, player.transform.position) < minCarDistance && Input.GetButtonDown("Fire1")) {
                Debug.LogWarning(onRide);
                playerMove.overridePosition(transform.position);
                playerMove.toggleColDetect();
                onRide = true;
            }
        }
        else {
            if (Input.GetButtonDown(key)) {
                Debug.LogWarning(onRide);
                playerMove.freePosition();
                playerMove.toggleColDetect();
                onRide = false;
            }
            playerMove.overridePosition(transform.position);
            move();
        }
	}
    
    void move() {
        
        moveSpeed = new Vector3(Input.GetAxis("Horizontal"), moveSpeed.y+ gravity, Input.GetAxis("Vertical")) * speed;
Debug.Log(isGro +"  1");
        if (isGro)
            moveSpeed.y = 0;
        isGro = false;
        transform.Translate(moveSpeed);
        Debug.Log(isGro + "  2");
        //rigidBody.velocity = newPosition;
    }
    public float groundToleranceAngle = 40;
    private bool isGro = false;

    private void OnCollisionEnter(Collision collision) {

        foreach (ContactPoint hit in collision.contacts) {
            if (Vector3.Angle(hit.normal, Vector3.up) < groundToleranceAngle) {
                Debug.LogWarning("we collided OO:");
                isGro = true;
                moveSpeed.y = 0;
                groundNormal = hit.normal;
            }
        }
           
    }
}

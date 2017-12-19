using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {
    // buttons
    private string accelKey = "Fire1";
    private string brakeKey = "Fire2";
    private string directionAxis = "Horizontal";
    private string driftKey = "Fire3";
    private string ioKey = "Jump";


    private GameObject player;
    private Rigidbody rigidBody;
    private BasicMovement playerMove;

    // movement
    private Vector3 moveSpeed;
    private Vector3 groundNormal;
    public float minCarDistance =3.5f;
    private bool onRide = false;
    public float speed=5f;
    public float gravity = -0.9f;
    private float outPositon = -3.5f;
    private CamController cameraCon;

    // car
    private GameObject[] wheelsFront = new GameObject[2];
    private GameObject[] wheelsBack = new GameObject[2];
    private GameObject[] seats = new GameObject[2];
    private GameObject[] wheelDirAxis = new GameObject[2];
    private Rigidbody[] wheelTractionAxis = new Rigidbody[2];

    private JointMotor forwardMotor = new JointMotor();
    private JointMotor backwardMotor = new JointMotor();
    private JointMotor zeroMotor = new JointMotor();

    private int L = 0;
    private int R = 1;
    // Use this for initialization
    void Start () {

        forwardMotor.force = 50;
        forwardMotor.freeSpin = true;
        forwardMotor.targetVelocity = 500;
        backwardMotor.force = 50;
        backwardMotor.freeSpin = true;
        backwardMotor.targetVelocity = -500;
        zeroMotor.force = 0;
        zeroMotor.freeSpin = true;
        zeroMotor.targetVelocity = 0;

        cameraCon = GameObject.Find("Main Camera").GetComponent<CamController>();
        player = GameObject.Find("edgarMCSkin");
        playerMove = player.GetComponent<BasicMovement>();
        rigidBody = GetComponent<Rigidbody>();
        GameObject[] wheels = GameObject.FindGameObjectsWithTag("wheel");
        GameObject[] seats_ = GameObject.FindGameObjectsWithTag("seat");

        foreach(GameObject wheel in wheels) {
            
            if (wheel.name == "frontLeftAxis") {
                wheelsFront[L] = wheel;
            }
            if (wheel.name == "backLeftAxis") {
                wheelsBack[L] = wheel;
            }
            if (wheel.name == "frontRightAxis") {
                wheelsFront[R] = wheel;
            }
            if (wheel.name == "backRightAxis") {
                wheelsBack[R] = wheel;
            }

            if (wheel.name == "rightDirectionAxis") {
                wheelDirAxis[R] = wheel;
            }
            if (wheel.name == "leftDirectionAxis") {
                wheelDirAxis[L] = wheel;
            }
        }

        wheelTractionAxis[L] = wheelsBack[L].GetComponent<Rigidbody>();
        wheelTractionAxis[R] = wheelsBack[R].GetComponent<Rigidbody>();



        foreach (GameObject seat in seats_) {
            if (seat.name == "seatLeft") {
                seats[L] = seat;
            }
            if (seat.name == "seatRight") {
                seats[R] = seat;
            }
        }
        GameObject wheelObject = Resources.Load<GameObject>("defaultWheel");
        Mesh seatMesh = Resources.Load<Mesh>("defaultSeat");
        
        Transform wheelColliderTr = wheelObject.transform.Find("wheelCollider");
        Transform wheelMeshTr = wheelObject.transform.Find("wheelMesh");
        
        Mesh wheelMesh = wheelMeshTr.GetComponent<MeshFilter>().sharedMesh;
        Mesh wheelColl = wheelColliderTr.GetComponent<MeshFilter>().sharedMesh;
        //Debug.Log(seatMesh.vertexCount);
        wheelsFront[L].GetComponent<MeshFilter>().mesh = wheelMesh;
        wheelsFront[L].GetComponent<MeshCollider>().sharedMesh = wheelColl;
        wheelsFront[L].transform.localRotation = Quaternion.AngleAxis(90,Vector3.forward);
        wheelsFront[R].GetComponent<MeshFilter>().mesh = wheelMesh;
        wheelsFront[R].GetComponent<MeshCollider>().sharedMesh = wheelColl;
        wheelsFront[R].transform.localRotation = Quaternion.AngleAxis(-90, Vector3.forward);
        wheelsBack[L].GetComponent<MeshFilter>().mesh = wheelMesh;
        wheelsBack[L].GetComponent<MeshCollider>().sharedMesh = wheelColl;
        wheelsBack[L].transform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
        wheelsBack[R].GetComponent<MeshFilter>().mesh = wheelMesh;
        wheelsBack[R].GetComponent<MeshCollider>().sharedMesh = wheelColl;
        wheelsBack[R].transform.localRotation = Quaternion.AngleAxis(-90, Vector3.forward);
        seats[L].GetComponent<MeshFilter>().mesh = seatMesh;
        seats[R].GetComponent<MeshFilter>().mesh = seatMesh;

        Physics.IgnoreCollision(GetComponentInChildren<Collider>(), wheelsFront[L].GetComponent<Collider>(), true);
        Physics.IgnoreCollision(GetComponentInChildren<Collider>(), wheelsFront[R].GetComponent<Collider>(), true);
    }

    private Vector3 front;
    private Vector3 right;
	// Update is called once per frame
	void Update () {
        front = transform.forward;
        right = transform.right;

        if (!onRide) {
            if (Vector3.Distance(transform.position, player.transform.position) < minCarDistance && Input.GetButtonDown(ioKey)) {
                playerMove.overridePosition(seats[L].transform);
                playerMove.toggleColDetect();
                playerMove.seat(transform.forward);
                cameraCon.changeTarget(gameObject);
                onRide = true;
            }
        }
        else {
            move2();
            if (Input.GetButtonDown(ioKey)) {
                playerMove.freePosition( transform.position + (transform.right * outPositon) );
                playerMove.toggleColDetect();
                cameraCon.resetTarget();
                onRide = false;
            }

        }
	}

    private float Zspd = 0;
    public float maxFrontSpd = 20f;
    public float maxBackSpd = 13f;
    public float accelForce = 8f;
    public float brakeForce = 18f;
    public float freeBrakeForce = 5f;

    public float rotSpeed = 70;

    
    void move2() {

        JointMotor frMotorL = new JointMotor();
        frMotorL.force = accelForce;
        frMotorL.freeSpin = true;
        frMotorL.targetVelocity = -maxFrontSpd;

        JointMotor frMotorR = new JointMotor();
        frMotorR.force = accelForce;
        frMotorR.freeSpin = true;
        frMotorR.targetVelocity = maxFrontSpd;
        if (Input.GetButtonDown(accelKey)) {
            //wheelTractionAxis[L].AddRelativeTorque(-Vector3.up*accelForce, ForceMode.Force);
            //wheelTractionAxis[R].AddRelativeTorque(Vector3.up*accelForce, ForceMode.Force);

            wheelsBack[L].GetComponent<HingeJoint>().motor = frMotorL;
            wheelsBack[R].GetComponent<HingeJoint>().motor = frMotorR;
            wheelsBack[L].GetComponent<HingeJoint>().useMotor = true;
            wheelsBack[R].GetComponent<HingeJoint>().useMotor = true;
        }
        if (Input.GetButtonUp(accelKey)) {
            wheelsBack[L].GetComponent<HingeJoint>().motor = zeroMotor;
            wheelsBack[R].GetComponent<HingeJoint>().motor = zeroMotor;

        }
        /*else {
            if (Input.GetButton(brakeKey)) {

                wheelTractionAxis[L] = backwardMotor;
                wheelTractionAxis[R] = backwardMotor;
            }
            else {
                wheelTractionAxis[L] = zeroMotor;
                wheelTractionAxis[R] = zeroMotor;
            }
        }*/
        //wheelsBack[L].GetComponent<Rigidbody>().AddTorque(,);

        float horSpd = Input.GetAxis("Horizontal");

        wheelDirAxis[L].transform.rotation = Quaternion.AngleAxis((60 * horSpd), transform.up) * transform.rotation;
        wheelDirAxis[R].transform.rotation = Quaternion.AngleAxis((60 * horSpd), transform.up) * transform.rotation;

    }
    void move() {
        if (Input.GetButton(accelKey)) {
            if(Zspd < maxFrontSpd)
                Zspd += accelForce * Time.deltaTime;
        }
        else {
            if (Input.GetButton(brakeKey)) {
                if (Zspd > -maxBackSpd)
                    Zspd -= brakeForce * Time.deltaTime;
            }
            else {
                if(Zspd != 0) { 
                    float Sign = Mathf.Sign(Zspd);
                    Zspd -= Sign * freeBrakeForce * Time.deltaTime;
                    if (Sign != Mathf.Sign(Zspd)) Zspd = 0;
                }
            }
        }
        if (isGro)
            moveSpeed.y = 0;
        isGro = false;

        float horSpd = Input.GetAxis("Horizontal");
        rigidBody.AddForceAtPosition(front * Zspd * speed, wheelsBack[0].transform.position);
        rigidBody.AddForceAtPosition(front * Zspd * speed, wheelsBack[1].transform.position);


        if(horSpd != 0.0f) {
            transform.rotation *= Quaternion.AngleAxis(horSpd * rotSpeed * Time.deltaTime, Vector3.up);
        }


        //rigidBody.AddForceAtPosition(right * horSpd * speed, wheelsFront[0].transform.position);
        //rigidBody.AddForceAtPosition(right * horSpd * speed, wheelsFront[1].transform.position);

        Debug.DrawRay(transform.position,velocityDirection,Color.red);
        Debug.LogWarning(velocityDirection);
        
        
    }
    public float groundToleranceAngle = 45;
    private bool isGro = false;
    private Vector3 velocityDirection = Vector3.up;
    private void OnCollisionStay(Collision collision) {
        //velocityDirection = collision.relativeVelocity;
        //velocityDirection.y = 0;
        //transform.rotation = Quaternion.LookRotation(-velocityDirection,transform.up);
        foreach (ContactPoint hit in collision.contacts) {
            if (Vector3.Angle(hit.normal, Vector3.up) < groundToleranceAngle) {

                //Debug.LogWarning("we collided OO:");
                isGro = true;
                moveSpeed.y = 0;
                groundNormal = hit.normal;
            }
        }
           
    }
}

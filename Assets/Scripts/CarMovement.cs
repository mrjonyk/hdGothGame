using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {
    // buttons
    private string accelKey = "Fire1";
    private string brakeKey = "Fire2";
    private string directionAxis = "Horizontal";
    private string reverseKey = "Fire3";
    private string ioKey = "Jump";

    // movement
    public float minCarDistance =3.5f;
    private bool onRide = false;
    private float outPositon = -3.5f;
    //public float maxFrontSpd = 20f;
    //public float maxBackSpd = 13f;
    public float accelForwardForce = 300f;
    public float brakeForce = 400f;
    public float accelBackForce = 200f;
    public float maxWheelDirAngle = 55;
    // wheels
    private GameObject[] wheelsAxis = new GameObject[4];
    private GameObject[] wheelsMeshAxis = new GameObject[4];
    private GameObject[] seatsBase = new GameObject[2];
    private WheelCollider[] wheelsCollider = new WheelCollider[4];
    // index dictionary
    private int FL = 0; // Front Left
    private int FR = 1; // Front Right
    private int BL = 2; // Back Left
    private int BR = 3; // Back Right

    // miscelaneous objects
    public Material defaultMaterial;
    private CamController mainCamera;
    private GameObject player;
    private Rigidbody rigidBody;
    private BasicMovement playerMove;

    void Start () {

        mainCamera = GameObject.Find("Main Camera").GetComponent<CamController>();
        player = GameObject.Find("edgarMCSkin");
        playerMove = player.GetComponent<BasicMovement>();
        rigidBody = GetComponent<Rigidbody>();

        SetWheels();
        SetChasis();
    }

    void SetWheels() {

        // setup the wheels
        GameObject[] wheels = FindGameObjectInChildWithTag(transform,"wheel");
        
        

        foreach (GameObject wheel in wheels) {

            if (wheel.name == "frontLeftAxis") {
                wheelsAxis[FL] = wheel;
            }
            if (wheel.name == "frontRightAxis") {
                wheelsAxis[FR] = wheel;
            }
            if (wheel.name == "backLeftAxis") {
                wheelsAxis[BL] = wheel;
            }
            if (wheel.name == "backRightAxis") {
                wheelsAxis[BR] = wheel;
            }
        }

        JointSpring spring = new JointSpring();
        spring.spring = 5000;
        spring.damper = 100;
        spring.targetPosition = 0.5f;

        WheelFrictionCurve forwardFrCurve = new WheelFrictionCurve();
        forwardFrCurve.extremumSlip = 1.4f;
        forwardFrCurve.extremumValue = 1f;
        forwardFrCurve.asymptoteSlip = 1.8f;
        forwardFrCurve.asymptoteValue = 0.5f;
        forwardFrCurve.stiffness = 1f;

        WheelFrictionCurve sideFrCurve = new WheelFrictionCurve();
        sideFrCurve.extremumSlip = 1.2f;
        sideFrCurve.extremumValue = 1f;
        sideFrCurve.asymptoteSlip = 1.5f;
        sideFrCurve.asymptoteValue = 0.75f;
        sideFrCurve.stiffness = 1f;

        Mesh wheelMesh = Resources.Load<GameObject>("defaultWheel").transform.Find("wheelMesh").GetComponent<MeshFilter>().sharedMesh;
        Mesh wheelRad = Resources.Load<GameObject>("defaultWheel").transform.Find("wheelRadius").GetComponent<MeshFilter>().sharedMesh;

        float wheelRadius = 0;
        foreach (Vector3 vertice in wheelRad.vertices)
            if (vertice.x > wheelRadius)
                wheelRadius = vertice.x;
        for (int a = 0; a < 4; a++) {
            GameObject wheel = new GameObject("wheelMesh");
            wheelsMeshAxis[a] = new GameObject("wheelAxis");


            wheel.AddComponent<MeshFilter>();
            wheel.AddComponent<MeshRenderer>();

            wheel.GetComponent<MeshRenderer>().sharedMaterial = defaultMaterial;
            wheel.GetComponent<MeshFilter>().mesh = wheelMesh;

            wheel.transform.parent = wheelsMeshAxis[a].transform;

            wheelsMeshAxis[a].transform.parent = wheelsAxis[a].transform;
            wheelsMeshAxis[a].transform.position = wheelsAxis[a].transform.position;
            wheel.transform.position = wheelsMeshAxis[a].transform.position;

            if(a%2==0)
                wheel.transform.rotation *= Quaternion.AngleAxis(90, Vector3.forward);
            else
                wheel.transform.rotation *= Quaternion.AngleAxis(-90, Vector3.forward);

            wheelsAxis[a].AddComponent<WheelCollider>();
            
            wheelsCollider[a] = wheelsAxis[a].GetComponent<WheelCollider>();
            wheelsCollider[a].mass = 10;
            wheelsCollider[a].radius = wheelRadius*1.1f;
            wheelsCollider[a].wheelDampingRate = 0.8f;
            wheelsCollider[a].suspensionDistance = 0.3f;
            wheelsCollider[a].suspensionSpring = spring;
            wheelsCollider[a].forwardFriction = forwardFrCurve;
            wheelsCollider[a].sidewaysFriction = sideFrCurve;
        }
        //wheelsMesh[FL].transform.localRotation *= Quaternion.AngleAxis(90, Vector3.forward);
        //wheelsMesh[FR].transform.localRotation *= Quaternion.AngleAxis(-90, Vector3.forward);
        //wheelsMesh[BL].transform.localRotation *= Quaternion.AngleAxis(90, Vector3.forward);
        //wheelsMesh[BR].transform.localRotation *= Quaternion.AngleAxis(-90, Vector3.forward);



        //Physics.IgnoreCollision(GetComponentInChildren<Collider>(), wheelsFront[L].GetComponent<Collider>(), true);
        //Physics.IgnoreCollision(GetComponentInChildren<Collider>(), wheelsFront[R].GetComponent<Collider>(), true);
    }

    void SetChasis() {

        GameObject[] seats = FindGameObjectInChildWithTag(transform, "seat");
        foreach (GameObject seat in seats) {
            if (seat.name == "seatLeft") {
                seatsBase[FL] = seat;
            }
            if (seat.name == "seatRight") {
                seatsBase[FR] = seat;
            }
        }

        Mesh seatMesh = Resources.Load<Mesh>("defaultSeat");

        foreach(GameObject seat in seatsBase) {
            seat.AddComponent<MeshFilter>();
            seat.GetComponent<MeshFilter>().mesh = seatMesh;
            seat.AddComponent<MeshRenderer>();
            seat.GetComponent<MeshRenderer>().material = defaultMaterial;
        }

        rigidBody.centerOfMass = Vector3.zero;

    }

	// Update is called once per frame
	void Update () {
        if (!onRide) {
            if (Vector3.Distance(transform.position, player.transform.position) < minCarDistance && Input.GetButtonDown(ioKey)) {
                playerMove.overridePosition(seatsBase[FL].transform);
                playerMove.toggleColDetect();
                playerMove.seat(transform.forward);
                player.SendMessage("deactivate");
                mainCamera.changeTarget(gameObject);
                onRide = true;
            }
        }
        else {
            move();
            if (Input.GetButtonDown(ioKey)) {
                playerMove.freePosition( transform.position + (transform.right * outPositon) );
                playerMove.toggleColDetect();
                player.SendMessage("activate");
                mainCamera.resetTarget();
                onRide = false;
            }
        }
	}
    
    void move() {
        if (Input.GetButton(accelKey)) {
            wheelsCollider[BL].motorTorque = accelForwardForce;
            wheelsCollider[BR].motorTorque = accelForwardForce;
            wheelsCollider[BL].brakeTorque = 0;
            wheelsCollider[BR].brakeTorque = 0;
        }
        else {
            if (Input.GetButton(reverseKey)) {
                
                wheelsCollider[BL].motorTorque = -accelBackForce;
                wheelsCollider[BR].motorTorque = -accelBackForce;
                wheelsCollider[BL].brakeTorque = 0;
                wheelsCollider[BR].brakeTorque = 0;
            }
            else {
                if (Input.GetButton(brakeKey)) {
                    wheelsCollider[BL].motorTorque = 0;
                    wheelsCollider[BR].motorTorque = 0;
                    wheelsCollider[BL].brakeTorque = brakeForce;
                    wheelsCollider[BR].brakeTorque = brakeForce;
                }
                else {
                    if (Input.GetButtonUp(accelKey) || Input.GetButtonUp(brakeKey) || Input.GetButtonUp(reverseKey)) {
                        wheelsCollider[BL].motorTorque = 0;
                        wheelsCollider[BR].motorTorque = 0;
                        wheelsCollider[BL].brakeTorque = 0;
                        wheelsCollider[BR].brakeTorque = 0;
                    }
                }
            }
        }
        float horSpd = Input.GetAxis("Horizontal");

        wheelsCollider[FL].steerAngle = (maxWheelDirAngle * horSpd);
        wheelsCollider[FR].steerAngle = (maxWheelDirAngle * horSpd);

        Vector3 positionTmp;
        Quaternion rotationTmp;

        for(int a = 0; a < 4; a++) {
            wheelsCollider[a].GetWorldPose(out positionTmp, out rotationTmp);
            wheelsMeshAxis[a].transform.position = positionTmp;
            wheelsMeshAxis[a].transform.rotation = rotationTmp;
        }
    }

    public GameObject[] FindGameObjectInChildWithTag(Transform parent, string tag) {
        if(parent.childCount > 0) {
            List<GameObject> childList = new List<GameObject>();
            foreach (Transform tr in parent) {
                if (tr.tag == tag) {
                    childList.Add(tr.gameObject);
                }
                if (tr.childCount > 0) {
                    foreach(GameObject obj in FindGameObjectInChildWithTag(tr, tag)) {
                        childList.Add(obj);
                    }
                }
            }
            return childList.ToArray();
        }
        else {
            return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour {
    // buttons

    private string trinagle = "Jump";
    private string square = "Fire3";
    private string circle = "Fire2";
    private string cross = "Fire1";

    // self's camera
    private GameObject mainCamera;
    private Vector3 camRelMovement;

    // animation
    private Animator anim;
    public float walkLimit = 0.5f;
    private IEnumerator blinkCoroutine;

    // for movement
    public float globalSpeed = 1;
    public float jumpSpeed = 20;
    private CharacterController charController;
    public float gravity = -0.9f;
    public float maxGravity = -40f;
    private Vector2 stickVector;
    private Vector2 camForward;
    private Vector2 camRight;
    private Vector3 movementToSpeed;
    // this value multiply the stick raw input, increase for more speed
    public float speedScale = 14f;
    public float brakeFactor = 35f;
    public float counterSpeed = 40f;

    // for collitions
    private bool isGro;
    private bool isWal;
    private Vector3 floorNormal;
    private Vector3 wallNormal;
    public float groundAngleTolerance=40;
    public float wallAngleTolerance = 10;

    // the direction you are looking at
    private Quaternion lookRot;
    public float rotSpeed = 400;
    private bool isLookDirLocked;

    void Start () {
        charController = GetComponent<CharacterController>();
        mainCamera = GameObject.Find("Main Camera");
        anim = GetComponent<Animator>();
        isGro = false;
        isWal = false;
        movementToSpeed = Vector3.zero;

        blinkCoroutine = Blinking();
        StartCoroutine(blinkCoroutine);
    }
	
	void Update () {
        if (!isOverride)
		    move();
	}

    void move() {
        stickVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        camForward = new Vector2(mainCamera.transform.forward.x, mainCamera.transform.forward.z);
        camRight = new Vector2(mainCamera.transform.right.x, mainCamera.transform.right.z);
        camForward.Normalize();
        camRight.Normalize();

        camRelMovement = new Vector3((camForward.x * stickVector.y) + (camRight.x * stickVector.x), 0f, (camForward.y * stickVector.y) + (camRight.y * stickVector.x));
        float magnitude = stickVector.magnitude;

        if (magnitude > 0) {
            if(!isLookDirLocked)
                lookRot = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, Angle(camRelMovement), 0), rotSpeed * Time.deltaTime);
            anim.SetBool("walking", true);
            if (magnitude > walkLimit) {
                anim.SetBool("running", true);
            }
            else {
                anim.SetBool("running", false);
            }
        }
        else {
            anim.SetBool("walking", false);
            anim.SetBool("running", false);
        }

        if(movementToSpeed.y > maxGravity)
            movementToSpeed.y += (-maxGravity+ movementToSpeed.y) * gravity * Time.deltaTime;

        if (!isLookDirLocked) 
            transform.rotation = lookRot;
        else
            magnitude = 0;

        if (magnitude > 0) {
            movementToSpeed.x = axisMoveTowards(movementToSpeed.x, camRelMovement.x);
            movementToSpeed.z = axisMoveTowards(movementToSpeed.z, camRelMovement.z);
        }
        else {
            movementToSpeed.x = axisStop(movementToSpeed.x);
            movementToSpeed.z = axisStop(movementToSpeed.z);
        }

        isGro = false;
        isWal = false;
        charController.Move(movementToSpeed * Time.deltaTime * globalSpeed);

        if (Input.GetButtonDown(cross) && isGro) {
            movementToSpeed.y = jumpSpeed;
            anim.Play("Jump", 0);
        }
    }
    private float axisMoveTowards(float speedAxis, float moveAxis) {
        if (speedAxis != moveAxis * speedScale) {
            if (moveAxis > 0) {
                if (speedAxis > moveAxis * speedScale) {
                    return speedAxis - (brakeFactor * Time.deltaTime);
                }
                else {
                    return speedAxis + (counterSpeed * Time.deltaTime);
                }
            }
            else {
                if (speedAxis < moveAxis * speedScale) {
                    return speedAxis + (brakeFactor * Time.deltaTime);
                }
                else {
                    return speedAxis - (counterSpeed * Time.deltaTime);
                }
            }
        }
        return speedAxis;
    }

    private float axisStop(float speedAxis) {
        if (speedAxis != 0) {
            float sign = Mathf.Sign(speedAxis);
            float newSpd = speedAxis - (brakeFactor * Time.deltaTime * sign);
            if (sign != Mathf.Sign(newSpd))
                return 0;
            else
                return newSpd;
        }
        return speedAxis;
    }

    private float hitAngle;
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        hitAngle = Vector3.Angle(hit.normal, Vector3.up);
        if (hitAngle < groundAngleTolerance) {
            isGro = true;
            movementToSpeed.y = 0;
            floorNormal = hit.normal;
        }

        if (hitAngle > 90 - wallAngleTolerance && hitAngle < 90 + wallAngleTolerance) {
            isWal = true;
            wallNormal = hit.normal;
        }
    }


    // getters
    public bool isGrounded() {
        return isGro;
    }
    public bool isWalled() {
        return isWal;
    }
    public Vector3 getFloorNormal() {
        return floorNormal;
    }
    public Vector3 getWallNormal() {
        return wallNormal;
    }
    // setters
    public void lockLookDirection(Quaternion lookDirection) {
        isLookDirLocked = true;
        transform.rotation = lookDirection;
    }
    
    public void freeLookDirection() {
        isLookDirLocked = false;
    }

    // others
    public void addForce(Vector3 forceVector) {
        movementToSpeed += forceVector;
    }

    public bool isFalling() {
        return movementToSpeed.y < 0;
    }

    private bool isOverride = false;
    public void overridePosition(Transform newParent) {
        transform.position = newParent.position;
        transform.parent = newParent;
        isOverride = true;
    }
    public void freePosition( Vector3 atPosition) {
        transform.parent = null;
        isOverride = false;
        transform.position = atPosition;
        anim.Play("Idle");
    }

    public void toggleColDetect() {
        charController.detectCollisions = !charController.detectCollisions;
    }

    public void seat(Vector3 seatLooking) {
        transform.forward = seatLooking;
        anim.Play("Seat");
    }
    // this function calculate the angle just in the bottom plane
    public static float Angle(Vector3 dirVector) {
        if (dirVector.x < 0) {
            return 360 - (Mathf.Atan2(dirVector.x, dirVector.z) * Mathf.Rad2Deg * -1);
        }
        else {
            return Mathf.Atan2(dirVector.x, dirVector.z) * Mathf.Rad2Deg;
        }
    }

    private IEnumerator Blinking() {
        while (true) {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 6));
            anim.SetTrigger("blink");
        }
    }
}

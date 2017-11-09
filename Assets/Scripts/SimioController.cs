#define NORMALIZED 
//#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SimioController : MonoBehaviour {
    public LayerMask mask = -1;

    private Animator anim;
    private GameObject camera;
    private CharacterController charController;

    private float hSpd = 0;
    private float vSpd = 0;
    private float upSpd = 0;
    public float speed = 15;
    private Vector3 movement;
    private IEnumerator blinkCoroutine;
    public float walkLimit = 0.5f;
    public float groundAngleTolerance = 40;
    public float wallAngleTolerance = 5;

    private Vector3 currentFloorNormal = Vector3.up;
    private Quaternion lookRot;
    public float rotateSpeed = 400;

    public float gravity = -3.5f;
    public float wallGravityReduction = 0.2f;
    private float onWallGravity;
    public float wallSlideSpd = -0.2f;
    public float jumpSpd = 1.5f;

    public bool onParalelWalls = false;

    // for DiscCast method
    private int discCastNum = 18; // must be pair
    private float discHeight = 0;
    private Vector3[] discRays;
    private RaycastHit[] rayHits;
    private bool[] isRayHit;


    private bool isGrounded = false;
    private bool isWalled = false;
    void Start () {
        onWallGravity = gravity*wallGravityReduction;
        currentGravity = gravity;
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        lookRot = transform.rotation;
        camera = GameObject.Find("Main Camera");

        discCastNum = discCastNum % 2 == 0 ? discCastNum / 2 : (discCastNum + 1) / 2;
        discHeight = charController.height / 2;
        discRays = new Vector3[discCastNum];
        rayHits = new RaycastHit[discCastNum*2];
        isRayHit = new bool[discCastNum*2];
        float degreeDif = (180/ discCastNum) *Mathf.Deg2Rad;
        float angle = 0;
        for (uint a=0;a< discCastNum; a++) {
            angle = a * degreeDif;
            discRays[a] = new Vector3(Mathf.Sin(angle),0, Mathf.Cos(angle));
        }

        blinkCoroutine = Blinking();
        StartCoroutine(blinkCoroutine);
    }

    private Vector3 camPlayerVector;
    private Vector3 discCastOrigin;
#if DEBUG
    private float timeStamp = 0;
    private float lateTimeStamp = 0;
#endif
    void Update () {
        discCastOrigin = transform.position + (Vector3.up * discHeight);
        camPlayerVector = transform.position - camera.transform.position;
        move();
        onParalelWalls = false;
#if DEBUG
        Debug.Log("-________-");
#endif

#if DEBUG
        timeStamp = Time.realtimeSinceStartup;
#endif
        checkParaWall(paraWallRayLength);
#if DEBUG
        lateTimeStamp = Time.realtimeSinceStartup;
        //Debug.Log(lateTimeStamp - timeStamp);
#endif
        
    }
    private float currentGravity;
    public float moveAcc = 0.1f;
    private Vector2 finalMove = Vector2.zero;
    void move2() {
        hSpd = Input.GetAxis("Horizontal");
        vSpd = Input.GetAxis("Vertical");
        Vector2 camForward = new Vector2(camera.transform.forward.x, camera.transform.forward.z);
        Vector2 camRight = new Vector2(camera.transform.right.x, camera.transform.right.z);
#if NORMALIZED
        camForward.Normalize();
        camRight.Normalize();
#endif

        if (hSpd == 0) {
            if (finalMove.x != 0) { 
                float sign = Mathf.Sign(finalMove.x);
                finalMove.x -= moveAcc * sign;
                if (Mathf.Sign(finalMove.x) != sign) {
                    finalMove.x = 0;
                }
            }
        }
        else {
            if (hSpd < 0) {
                if(finalMove.x > hSpd) {
                    finalMove.x += hSpd * moveAcc* Time.deltaTime;
                }
                else {
                    finalMove.x = hSpd;
                }
            }
            else {
                if (finalMove.x < hSpd) {
                    finalMove.x += hSpd * moveAcc * Time.deltaTime;
                }
                else {
                    finalMove.x = hSpd;
                }
            }
        }

        if (vSpd == 0){
            if(finalMove.y != 0) {
                float sign = Mathf.Sign(finalMove.y);
                finalMove.y -= moveAcc * sign;
                if (Mathf.Sign(finalMove.y) != sign) {
                    finalMove.y = 0;
                }
            }
        }
        else {
            if (vSpd < 0) {
                if (finalMove.y > vSpd) {
                    finalMove.y += vSpd * moveAcc * Time.deltaTime;
                }
                else {
                    finalMove.y = vSpd;
                }
            }
            else {
                if (finalMove.y < vSpd) {
                    finalMove.y += vSpd * moveAcc * Time.deltaTime;
                }
                else {
                    finalMove.y = vSpd;
                }
            }
        }

        movement = new Vector3((camForward.x * finalMove.y) + (camRight.x * finalMove.x), 0f, (camForward.y * finalMove.y) + (camRight.y * finalMove.x));
        float magnitude = movement.magnitude;

        if (magnitude > 0) {
            lookRot = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, Angle(movement), 0), rotateSpeed * Time.deltaTime);
        }

        //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).length);

        if (!isGrounded) {
            if (isWalled){
                lookRot = Quaternion.LookRotation(-wallNormal, Vector3.up);
                if (upSpd < 0) {
                    currentGravity = -0.6f;
                    //upSpd = -wallSlideSpd;
                }
            }
            else {
                currentGravity = gravity;
            }
        }
        else {
            currentGravity = gravity;
        }
        upSpd += currentGravity* Time.deltaTime;

        transform.rotation = lookRot;
        movement.y = upSpd;
        isGrounded = false;
        isWalled = false;
        charController.Move(movement * Time.deltaTime * speed);

        Debug.Log("gnd:"+isGrounded+" wll:"+isWalled);
        if (Input.GetButtonDown("Jump")) {
            if (isGrounded) {
                upSpd = jumpSpd;
                anim.Play("Jump", 0);
            }
            else {
                if (isWalled) {
                    finalMove.y = -wallJumpSpd;
                    upSpd = jumpSpd;
                    //isWallJump = true;
                }
                else {
                    //isWallJump = false;
                }
            }
        }

        Debug.Log(upSpd);
    }

    void move() {
        hSpd = Input.GetAxis("Horizontal");
        vSpd = Input.GetAxis("Vertical");
        Vector2 camForward = new Vector2(camera.transform.forward.x, camera.transform.forward.z);
        Vector2 camRight = new Vector2(camera.transform.right.x, camera.transform.right.z);
#if NORMALIZED
        camForward.Normalize();
        camRight.Normalize();
#endif

        movement = new Vector3((camForward.x * vSpd) + (camRight.x * hSpd), 0f, (camForward.y * vSpd) + (camRight.y * hSpd));
        float magnitude = movement.magnitude;

        if (magnitude > 0) {
            lookRot = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, Angle(movement), 0), rotateSpeed * Time.deltaTime);
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

        //Debug.Log(anim.GetCurrentAnimatorStateInfo(0).length);

        if (!isGrounded) {
            if (isWalled && upSpd < 0) {
                currentGravity = 0;
                upSpd = wallSlideSpd;
                lookRot = Quaternion.LookRotation(-wallNormal, Vector3.up);
            }
            else {
                currentGravity = gravity;
            }
        }
        else {
            currentGravity = gravity;
        }
        upSpd += currentGravity * Time.deltaTime;

        transform.rotation = lookRot;

        Vector3 wallJumpDir = new Vector3(-wallNormal.x, 0, -wallNormal.z);
        Debug.Log("wallJump:" + isWallJump);
        if (isWallJump) {
            if (isWalled) {
                upSpd = wallJumpSpd;
                wallJumpVector = wallJumpDir * wallJumpSpd;
            }
            else {
                if (new Vector3(wallJumpVector.x, 0, wallJumpVector.z).magnitude > 0) {
                    wallJumpVector -= wallJumpDir;
                }
                else {
                    isWallJump = false;
                    wallJumpVector = Vector3.zero;
                }
            }
        }

        isGrounded = false;
        isWalled = false;
        charController.Move((new Vector3(transform.forward.x * magnitude, upSpd, transform.forward.z * magnitude) + wallJumpVector) * Time.deltaTime * speed);

        Debug.Log("gnd:" + isGrounded + " wll:" + isWalled);
        if (Input.GetButtonDown("Jump")) {
            if (isGrounded) {
                upSpd = jumpSpd;
                anim.Play("Jump", 0);
            }
            else {
                if (isWalled) {
                    isWallJump = true;
                }
                else {
                    isWallJump = false;
                }
            }
        }

        Debug.Log(upSpd);
    }

    private bool isWallJump = false;
    private Vector3 wallJumpVector = Vector3.zero;
    public float wallJumpSpd = 1;
    private Vector3 wallNormal;

    public Vector3[] parallelWallsPoint = new Vector3[2];
    public Vector3 backWallNormal;
    Quaternion nineDeg = Quaternion.Euler(0, 90, 0);
    Vector3 perpDir;
    Vector3 perpStart;
    public float paraWallRayLength = 4;
    void checkParaWall(float maxDistance) {

        if (hitedPreviousRay) {
            if (previousValidRayIndex < 0) {
                if (Physics.Raycast(discCastOrigin, -discRays[-previousValidRayIndex], out hit, maxDistance, mask.value)) {
                    if (hit.normal != otherNormal) {
                        otherNormal = hit.normal;
                        parallelPoints[0] = hit.point;
                        if (Physics.Raycast(hit.point, hit.normal, out hit, maxDistance * 2, mask.value)) {
                            float angleDiff = Vector3.Angle(otherNormal, hit.normal);
                            if (angleDiff > 175 && angleDiff < 185) {
                                parallelPoints[1] = hit.point;
#if DEBUG
                                Debug.DrawLine(parallelPoints[0], parallelPoints[1], Color.gray);
#endif
                            }
                            else {
                                hitedPreviousRay = false;
                                sendParaWallMessage(false);
#if DEBUG
                                Debug.Log("false on -3");
#endif
                            }
                        }
                        else {
                            hitedPreviousRay = false;
                            sendParaWallMessage(false);
#if DEBUG
                            Debug.Log("false on -2");
#endif
                        }
                    }

                }
                else {
                    hitedPreviousRay = false;
                    sendParaWallMessage(false);
#if DEBUG
                    Debug.Log("false on -1");
#endif
                }
            }
            else {
                if (Physics.Raycast(discCastOrigin, discRays[previousValidRayIndex], out hit, maxDistance, mask.value)) {
                    if (hit.normal != otherNormal) {
                        otherNormal = hit.normal;
                        parallelPoints[0] = hit.point;
                        if (Physics.Raycast(hit.point, hit.normal, out hit, maxDistance * 2, mask.value)) {
                            float angleDiff = Vector3.Angle(otherNormal, hit.normal);
                            if (angleDiff > 175 && angleDiff < 185) {
                                parallelPoints[1] = hit.point;
#if DEBUG
                                Debug.DrawLine(parallelPoints[0], parallelPoints[1], Color.black);
#endif
                            }
                            else {
                                hitedPreviousRay = false;
                                sendParaWallMessage(false);
#if DEBUG
                                Debug.Log("false on +3");
#endif
                            }
                        }
                        else {
                            hitedPreviousRay = false;
                            sendParaWallMessage(false);
#if DEBUG
                            Debug.Log("false on +2");
#endif
                        }
                    }
                }
                else {
                    hitedPreviousRay = false;
                    sendParaWallMessage(false);
#if DEBUG
                    Debug.Log("false on +1");
#endif
                }
            }
        }
        else {
            for (int a = 0; a < discCastNum; a++) {
#if DEBUG
                Debug.DrawLine(discCastOrigin, discCastOrigin + (discRays[a] * maxDistance), Color.green);
#endif

                if (Physics.Raycast(discCastOrigin, discRays[a], out hit, maxDistance, mask.value)) {
                    otherNormal = hit.normal;
                    parallelPoints[0] = hit.point;
#if DEBUG
                    Debug.DrawLine(hit.point, hit.point + (hit.normal * maxDistance * 2), Color.red);
#endif
                    if (Physics.Raycast(hit.point, hit.normal, out hit, maxDistance * 2, mask.value)) {
#if DEBUG
                        Debug.DrawLine(hit.point, hit.point + hit.normal, Color.blue);
#endif
                        float angleDiff = Vector3.Angle(otherNormal, hit.normal);
                        if (angleDiff > 175 && angleDiff < 185) {
                            parallelPoints[1] = hit.point;
                            hitedPreviousRay = true;
                            previousValidRayIndex = a;
                            calcParaWallDirection();
                            sendParaWallMessage(true);
                            break;
                        }
                    }
                }
                else {
#if DEBUG
                    Debug.DrawLine(discCastOrigin, discCastOrigin + (-discRays[a] * maxDistance), Color.yellow);
#endif
                    if (Physics.Raycast(discCastOrigin, -discRays[a], out hit, maxDistance, mask.value)) {
                        otherNormal = hit.normal;
                        parallelPoints[0] = hit.point;
#if DEBUG
                        Debug.DrawLine(hit.point, hit.point + (hit.normal * maxDistance * 2), Color.red);
#endif
                        if (Physics.Raycast(hit.point, hit.normal, out hit, maxDistance * 2, mask.value)) {
#if DEBUG
                            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.blue);
#endif
                            float angleDiff = Vector3.Angle(otherNormal, hit.normal);
                            if (angleDiff > 175 && angleDiff < 185) {
                                parallelPoints[1] = hit.point;
                                hitedPreviousRay = true;
                                previousValidRayIndex = -a;
                                calcParaWallDirection();
                                sendParaWallMessage(true);
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    void calcParaWallDirection() {
        parallelPoints[0].y = 0;
        parallelPoints[1].y = 0;
        camPlayerVector.y = 0;
        perpDir = (parallelPoints[0] - parallelPoints[1]);
        perpDir = nineDeg * perpDir;

        perpStart = (parallelPoints[0] + parallelPoints[1]) / 2;

        if (Vector3.Angle(camPlayerVector, perpDir) > Vector3.Angle(camPlayerVector, perpDir * -1)) {
            perpDir *= -1;
        }
    }

    bool hitedPreviousRay = false;
    RaycastHit hit;
    Vector3 otherNormal;
    int indexOne;
    int indexTwo;
    int previousValidRayIndex;
    Vector3[] parallelPoints = new Vector3[2];
    object[] paraWallDataPack = new object[3];
    void sendParaWallMessage( bool onPW) {
        paraWallDataPack[0] = onPW;
        if (onPW) {
            paraWallDataPack[1] = perpStart;
            paraWallDataPack[2] = perpDir;
        }
        camera.SendMessage("onParallelWalls", paraWallDataPack);
    }

    public static float Angle(Vector3 p_vector2) {
        if (p_vector2.x < 0) {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.z) * Mathf.Rad2Deg * -1);
        }
        else {
            return Mathf.Atan2(p_vector2.x, p_vector2.z) * Mathf.Rad2Deg;
        }
    }

    private IEnumerator Blinking() {
        while (true) {
            yield return new WaitForSeconds(UnityEngine.Random.Range(3, 6));
            anim.SetTrigger("blink");
#if DEBUG
            Debug.LogWarning("blink!!");
#endif
        }
    }

    float hitAngle;
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        hitAngle = Vector3.Angle(hit.normal, Vector3.up);
        if (hitAngle < groundAngleTolerance) {
            isGrounded = true;
            upSpd = 0;
            currentFloorNormal = hit.normal;
            //Debug.LogWarning("grounded!!");
        }

        if (hitAngle > 90-wallAngleTolerance && hitAngle < 90 + wallAngleTolerance) {
            isWalled = true;
            wallNormal = hit.normal;
            //Debug.LogWarning("onWall!!");
        }
    }
}

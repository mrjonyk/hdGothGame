 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelWallsSensor : MonoBehaviour {

    private bool isActivated = true;
    public LayerMask mask = 0;
    private CamController mainCamera;
    private CharacterController charController;
    public float detectDistance=4f;
    public float btwnWallsDistance=10.5f;
    private Vector3 discCastOrigin;
    public float discCastHeightFraction = 0.5f;
    private float discCastHeight;
    private bool paraWallFound = false;
    public int discCastNum = 8;
    private Vector3[] discRaysDirections;

    private Vector3 camPlayerVector;
    private Quaternion nineDegY = Quaternion.Euler(0, 90, 0);

    private RaycastHit hit;
    private Vector3 savedPoint;
    private Vector3 savedNormal;

    private int savedRayIndex;
    private int savedRaySign;

    void Start () {
        mainCamera = GameObject.Find("Main Camera").GetComponent<CamController>();
        charController = GetComponent<CharacterController>();
        discCastHeight = charController.height * discCastHeightFraction;
        discCastNum = discCastNum % 2 == 0 ? discCastNum / 2 : (discCastNum + 1) / 2;
        discRaysDirections = new Vector3[discCastNum];
        discCastOrigin = transform.position + (Vector3.up * discCastHeight);

        float degreeDif = (180 / discCastNum) * Mathf.Deg2Rad;
        float angle = 0;
        for (uint a = 0; a < discCastNum; a++) {
            angle = a * degreeDif;
            discRaysDirections[a] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        }

    }
	
	void Update () {
        if (isActivated) {
            camPlayerVector = mainCamera.transform.position - transform.position;
            discCastOrigin = transform.position + (Vector3.up * discCastHeight);
            checkParaWall();
        }
    }


    void checkParaWall() {

        if (paraWallFound) {
            if(!checkWall(discRaysDirections[savedRayIndex], savedRaySign)){
                paraWallFound = false;
                mainCamera.onParallelWalls(Vector3.zero,false);
            }
        }
        else {
            for (int a = 0; a < discCastNum; a++) {
                if (checkWall(discRaysDirections[a], 1)) {
                    paraWallFound = true;
                    savedRaySign = 1;
                    savedRayIndex = a;
                    mainCamera.onParallelWalls(calcParaWallDirection(savedPoint, hit.point),true);
                    break;
                }
                else {

                    if(checkWall(discRaysDirections[a], -1)) {
                        paraWallFound = true;
                        savedRaySign = -1;
                        savedRayIndex = a;
                        mainCamera.onParallelWalls(calcParaWallDirection(savedPoint, hit.point),true);
                        break;
                    }
                }
            }
        }
    }

    bool checkWall(Vector3 direction, int scale) {

#if DEBUG
        Debug.DrawLine(discCastOrigin, discCastOrigin + (direction * detectDistance * scale), Color.green);
#endif
        if (Physics.Raycast(discCastOrigin, direction * scale, out hit, detectDistance, mask.value)) {
            savedPoint = hit.point;
            savedNormal = hit.normal;
#if DEBUG
            Debug.DrawLine(hit.point, hit.point + (hit.normal * btwnWallsDistance), Color.red);
#endif
            if (Physics.Raycast(hit.point, hit.normal, out hit, btwnWallsDistance, mask.value)) {
#if DEBUG
                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.blue);
#endif
                float angleDiff = Vector3.Angle(savedNormal, hit.normal);
                if (angleDiff > 175 && angleDiff < 185) {
                    return true;
                }
            }
        }
        return false;
    }

    Vector3 calcParaWallDirection(Vector3 pointA, Vector3 pointB) {
        pointA.y = 0;
        pointB.y = 0;

        Vector3 toCameraDirection = nineDegY * (pointA - pointB);

        if (Vector3.Angle(camPlayerVector, toCameraDirection) > Vector3.Angle(camPlayerVector, toCameraDirection * -1)) {
            return toCameraDirection * -1;
        }
        return toCameraDirection;
    }

    void deactivate() {
        isActivated = false;
    }

    void activate() {
        isActivated = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {
    private GameObject player;
    private GameObject target;

    public Vector3 TargetPositionOffset = new Vector3(0,3.5f,0);
    // Use this for initialization

    // guardar lista de posiciones cada cierto tiempo

    private int listPosition = 0;
    Vector3[] posList = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
    private float lastSaved = 0;
    public float saveStateDelay = 0.1f;
    public float distanceToPlayer = 10;
    public float upDistance = 8;
    public float camSpeed = 1.5f;
    public float camRotSpeed = 50f;
    public float lookSpeed = 0.2f;
    private Vector3 ghostPosition;

    void Start () {
        player = GameObject.Find("edgarMCSkin");
        target = player;

        ghostPosition = target.transform.position - (target.transform.forward * distanceToPlayer) + (Vector3.up * upDistance);
    }

    // Update is called once per frame
    
    void Update () {
        if (onParaWalls) {
            ghostPosition = camMove(ghostPosition, target.transform.position+ TargetPositionOffset + (paraWallDir * distanceToPlayer),0);
            //Debug.DrawRay(target.transform.position, paraWallDir);
        }
        else {
            ghostPosition = camMove(ghostPosition, target.transform.position+ TargetPositionOffset, distanceToPlayer);
        }
        ghostPosition.y = Mathf.Lerp(ghostPosition.y, target.transform.position.y + upDistance, 0.1f);

        // rotate with stick
        ghostPosition = (Quaternion.AngleAxis(Input.GetAxis("rightHorizontal") * camRotSpeed * Time.deltaTime, Vector3.up) * (ghostPosition-target.transform.position))+ target.transform.position;

        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(((target.transform.position+ TargetPositionOffset) -transform.position).normalized), lookSpeed);

        transform.position = ghostPosition;
        
        RaycastHit hit;
        if (Physics.Raycast(target.transform.position+ TargetPositionOffset, transform.position - (target.transform.position+ TargetPositionOffset),out hit,Vector3.Distance(target.transform.position+ TargetPositionOffset, transform.position))) {
            //Debug.LogWarning("something between camear and player");
            transform.position = hit.point;
        }

    }
    Vector3 targPos;
    Vector3 camMove(Vector3 currentPosition,Vector3 targetPosition, float maxDistance) {
        targPos = targetPosition;
        //targPos.y += upDistance;
        targPos = Vector3.MoveTowards(currentPosition, targPos, (Vector3.Distance(currentPosition, targPos) - maxDistance) * camSpeed * Time.deltaTime);
        
        return targPos;
    }
    private bool onParaWalls = false;
    private Vector3 paraWallDir;
    private Vector3 paraWallStart;
    public void onParallelWalls(Vector3 fromWallsDirection, bool isOnParaWalls) {
        onParaWalls = isOnParaWalls;
        if (isOnParaWalls) {
            paraWallDir = fromWallsDirection.normalized;
        }
    }


    Vector3 nextCamPosition() {
        /*Vector3 targetBehind = player.transform.position - player.transform.forward * backDistance + Vector3.up * upDistance;
           if (Time.time > lastSaved + saveStateDelay) {
               lastSaved = Time.time;
               posList[listPosition] = player.transform.position;
               listPosition++;
               if (listPosition >= posList.Length) {
                   listPosition = 0;
               }
           }
           Vector3[] threePoints = new Vector3[3];

           for (int a = posList.Length-3; a < posList.Length; a++) {
               int currIndx = 0;

               if(listPosition + a >= posList.Length) {
                   currIndx = listPosition + a - posList.Length;
               }
               else {
                   currIndx = listPosition + a ; 
               }
               threePoints[a - (posList.Length - 3)] = posList[currIndx];
           }

           for (int a = 0; a < posList.Length-1; a++) {
               Debug.DrawLine(posList[a],posList[a+1]);
           }*/

        //float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot((threePoints[2] - threePoints[1]).normalized, (threePoints[1] - threePoints[0]).normalized));
        //Debug.Log(angle);
        //if(angle > 120) {
        // return Vector3.MoveTowards(transform.position, targetBehind, 5f * Vector3.Distance(transform.position, player.transform.position) * Time.deltaTime);
        // }else {
        //return Vector3.MoveTowards(transform.position, player.transform.position, 0.5f * (Vector3.Distance(transform.position, player.transform.position) - 20) * Time.deltaTime);
        //}
        return Vector3.zero;
    }
    public void changeTarget(GameObject newTarget) {
        target = newTarget;
    }
    public void resetTarget() {
        target = player;
    }
}

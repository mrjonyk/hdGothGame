using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {


    // position
    public Vector3 TargetPositionOffset = new Vector3(0,3.5f,0);
    public float distanceToPlayer = 10;
    public float upDistance = 8;
    private Vector3 ghostPosition;
    private float lastY = 0f;
    private Vector3 paraWallDir;

    // movement
    public float camSpeed = 1.5f;
    public float camRotSpeed = 50f;
    public float lookSpeed = 0.2f;

    // mocelaneous objects
    private GameObject player;
    private GameObject target;
    private bool onParaWalls = false;
    

    void Start () {
        player = GameObject.Find("edgarMCSkin");
        target = player;

        ghostPosition = target.transform.position - (target.transform.forward * distanceToPlayer) + (Vector3.up * upDistance);
        lastY = ghostPosition.y;
    }

    // Update is called once per frame
    
    void Update () {

        Vector3 curentTargetPlane = target.transform.position;
        curentTargetPlane.y = 0;

        Vector3 currentGhostPlane = ghostPosition;
        currentGhostPlane.y = 0;

        if (onParaWalls) {
            ghostPosition = Vector3.MoveTowards(currentGhostPlane, curentTargetPlane + (paraWallDir * distanceToPlayer), Vector3.Distance(currentGhostPlane, curentTargetPlane + (paraWallDir * distanceToPlayer)) * camSpeed * Time.deltaTime);

            Debug.DrawRay(target.transform.position, paraWallDir,Color.black);
        }
        else {
            ghostPosition = Vector3.MoveTowards(currentGhostPlane,curentTargetPlane,(Vector3.Distance(currentGhostPlane, curentTargetPlane)-distanceToPlayer) * camSpeed * Time.deltaTime);
        }

        lastY =  Mathf.Lerp(lastY, target.transform.position.y + TargetPositionOffset.y + upDistance, 0.1f);

        ghostPosition.y = lastY;
        //ghostPosition.y = Mathf.Lerp(ghostPosition.y, target.transform.position.y + TargetPositionOffset.y + upDistance, 0.1f);

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

    public void onParallelWalls(Vector3 fromWallsDirection, bool isOnParaWalls) {
        onParaWalls = isOnParaWalls;
        if (isOnParaWalls) {
            paraWallDir = fromWallsDirection.normalized;
        }
    }

    public void changeTarget(GameObject newTarget) {
        target = newTarget;
    }

    public void resetTarget() {
        target = player;
    }
}

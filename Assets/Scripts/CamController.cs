using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {

    private GameObject player;
    // Use this for initialization

    // guardar lista de posiciones cada cierto tiempo

    private int listPosition = 0;
    Vector3[] posList = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
    private float lastSaved = 0;
    public float saveStateDelay = 0.1f;
    public float distanceToPlayer = 10;
    public float upDistance = 10;
    public float camSpeed = 1.5f;

    void Start () {
        player = GameObject.Find("edgarMCSkin");
        transform.position = player.transform.position - player.transform.forward * distanceToPlayer + Vector3.up * upDistance;
    }

    Vector3 nextPosition;
    // Update is called once per frame
    void Update () {
        if (onParaWalls) {
            camMove(player.transform.position + (paraWallDir * distanceToPlayer),0);
            Debug.DrawRay(player.transform.position, paraWallDir);
        }
        else {
            camMove(player.transform.position, distanceToPlayer);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation((player.transform.position-transform.position).normalized),0.6f);

    }
    Vector3 targetPosition;
    void camMove(Vector3 targetPosition, float maxDistance) {

        targetPosition.y = player.transform.position.y + upDistance;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, (Vector3.Distance(transform.position, targetPosition) - maxDistance) * camSpeed * Time.deltaTime);
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

}

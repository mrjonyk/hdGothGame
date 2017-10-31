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
    public float backDistance = 10;
    public float upDistance = 10;

	void Start () {
        player = GameObject.Find("edgarMCSkin");
        transform.position = player.transform.position - player.transform.forward * backDistance + Vector3.up * upDistance;
    }

    Vector3 nextPosition;
    // Update is called once per frame
    void Update () {
        if (onParaWalls) {
            camMove2();
        }
        else {
            camMove1();
        }

        transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation((player.transform.position-transform.position).normalized),0.6f);

    }
    void camMove1() {
        nextPosition = Vector3.MoveTowards(transform.position, player.transform.position, (Vector3.Distance(transform.position, player.transform.position) - backDistance * 1.5f) * Time.deltaTime);
        nextPosition.y = upDistance;
        transform.position = nextPosition;
    }

    void camMove2() {

    }
    private bool onParaWalls = false;
    private Vector3 paraWallDir;
    private Vector3 paraWallStart;
    void onParallelWalls(object[] dataPack) {
        onParaWalls = (bool)dataPack[0];
        if (onParaWalls) {
            paraWallStart = (Vector3)dataPack[1];
            paraWallDir = (Vector3)dataPack[2];
        }
    }


    Vector3 nextCamPosition() {
        Vector3 targetBehind = player.transform.position - player.transform.forward * backDistance + Vector3.up * upDistance;
        /*   if (Time.time > lastSaved + saveStateDelay) {
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
        return Vector3.MoveTowards(transform.position, player.transform.position, 0.5f * (Vector3.Distance(transform.position, player.transform.position) - 20) * Time.deltaTime);
        //}
        //return Vector3.zero;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour {

    private BasicMovement playerMove;
    // Use this for initialization
    void Start () {
        playerMove = GetComponent<BasicMovement>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!playerMove.isGrounded() && playerMove.isWalled()) {

            playerMove.lockLookDirection(Quaternion.LookRotation(-playerMove.getWallNormal(), Vector3.up));

        }
        else {
            playerMove.freeLookDirection();
        }
	}
}

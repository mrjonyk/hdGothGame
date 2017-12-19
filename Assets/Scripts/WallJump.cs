using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour {

    private string JumpKey = "Fire1";
    private BasicMovement playerMove;
    public float jumpBackForce = 25f;
    public float jumpUpForce = 20f;
    private bool isDirLock = false;
    private Vector3 jumpDirection;
    public float friction = 20;


    // Use this for initialization
    void Start () {
        playerMove = GetComponent<BasicMovement>();
	}
	
	// Update is called once per frame
	void Update () {

		if(!playerMove.isGrounded() && playerMove.isWalled()) {

            if (Input.GetButtonDown(JumpKey)) {
                jumpDirection= playerMove.getWallNormal()* jumpBackForce + Vector3.up* jumpUpForce;
                playerMove.addForce(jumpDirection);

            }
            else {
                if(playerMove.isFalling()) 
                playerMove.addForce(Vector3.up * friction * Time.deltaTime);
            }

            if (!isDirLock) {
                isDirLock = true;
                playerMove.lockLookDirection(Quaternion.LookRotation(-playerMove.getWallNormal(), Vector3.up));
            }
        }
        else {
            if (isDirLock) {
                isDirLock = false;
                playerMove.freeLookDirection();
            }
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {
    private bool isActivated = true;
    private string key = "Fire3";
    private bool active = false;
    private GameObject shield;
    public float forwardDistance = 1.7f;
    public float upDistance = 2.8f;

    void Start () {
        shield = Instantiate(Resources.Load<GameObject>("shield"), transform.position + (transform.forward * forwardDistance)+(Vector3.up*upDistance),Quaternion.LookRotation(-transform.forward),transform);
        shield.SetActive(false);
    }
	
	void Update () {
        if (isActivated) {
            if (active) {
                if (Input.GetButtonDown(key)) {
                    shield.SetActive(false);
                    active = false;
                }
            }
            else {
                if (Input.GetButtonDown(key)) {
                    shield.SetActive(true);
                    active = true;
                }
            }
        }
	}

    void deactivate() {
        isActivated = false;
    }

    void activate() {
        isActivated = true;
    }
}

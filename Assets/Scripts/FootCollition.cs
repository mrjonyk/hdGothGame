using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootCollition : MonoBehaviour {

    SimioController playerController;
	// Use this for initialization
	void Start () {
        playerController = GetComponentInParent<SimioController>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision) {

        Debug.Log(collision.contacts[0].normal);
        //foreach (ContactPoint contact in collision.contacts) {
        //    print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
        //    Debug.DrawRay(contact.point, contact.normal, Color.white);
        //}
    }
}

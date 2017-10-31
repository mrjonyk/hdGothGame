using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingCube : MonoBehaviour {

    private Rigidbody rb;
    public float speed = 1f;
    public float distance = 4f;
    private Vector3 start;
	// Use this for initialization
	void Start () {
        start = transform.position;
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(start, transform.position) > distance)
            speed = -speed;
        rb.velocity = new Vector3(speed,0,0);
	}
}

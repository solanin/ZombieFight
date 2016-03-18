﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{

    GameObject self;
    Rigidbody rigidBody;
    bool isJumping;
    int direction; // -1 left, 1 right

    // Use this for initialization
    void Start()
    {
        self = GameObject.FindGameObjectWithTag("Player");
        rigidBody = self.GetComponent<Rigidbody>();
        direction = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            rigidBody.velocity = new Vector3(-10.0f, rigidBody.velocity.y);
            direction = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            rigidBody.velocity = new Vector3(10.0f, rigidBody.velocity.y);
            direction = 1;
        }
        if (Input.GetKeyDown(KeyCode.W) && !isJumping)
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 60.0f);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            fireBullet();
        }

		// Screen Wrap
		if (transform.position.x > 50f) {
			transform.position = new Vector3(-50f, 0f, 0f);
		} else if (transform.position.x < -50f) {
			transform.position = new Vector3(50f, 0f, 0f);	
		} 
    }

    void fireBullet()
    {
        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.tag = "Bullet";
        bullet.transform.localScale = new Vector3(.1f, .1f, .1f);
        bullet.transform.position = this.transform.position + new Vector3(direction,0,0);
        bullet.AddComponent<Rigidbody>();
        bullet.GetComponent<Rigidbody>().velocity = new Vector3(30.0f * direction, 0.0f, 0.0f);
        bullet.GetComponent<Rigidbody>().useGravity = false;
        Destroy(bullet, 1.50f);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isJumping = false;
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Ground")
        {
            isJumping = true;
        }
    }
}

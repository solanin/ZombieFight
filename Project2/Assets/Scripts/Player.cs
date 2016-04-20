﻿using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    GameObject bg;
    GameObject self;
    Rigidbody rigidBody;
    bool isJumping;

    int direction = 1;
	public int Direction { get { return direction; } }

    private bool shootUp = false;
	public bool ShootUp { get { return shootUp; } }
	private bool shootRight = false;
	public bool ShootRight { get { return shootRight; } }
	private bool shootLeft = false;
	public bool ShootLeft { get { return shootLeft; } }

	private Health health;
	private GameManager gm;

    //list of bullets remaining
    float[] bullets = new float[4] { -1.0f, 200.0f, 10.0f, 10.0f };

    //index of currently equipped weapon
    int currentWeapon = 0;

    public GameObject pistolBulletPrefab = null;
    public GameObject machineGunBulletPrefab = null;
    public GameObject laserPrefab = null;
	public GameObject fireballPrefab = null;

    public int fireballDamage = 10;
    public int pistolDamage = 2;
    public int machineGunDamage = 2;
    public int laserDamage = 10;

    float bulletCool = 0.15f;
    bool laser = false;
	public bool Laser { get { return laser; } }

    bool immune = false;
    bool visible = true;
    float flashTimer = 0.1f;
    float playerFlashTime = 1.0f;

    void Start()
    {
        self = GameObject.FindGameObjectWithTag("Player");
        bg = GameObject.Find("BG");
        rigidBody = self.GetComponent<Rigidbody>();
        direction = 1;

        health = GetComponent<Health>();
		gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    void Update()
    {
        if (bulletCool < 0.15f) bulletCool += Time.deltaTime;
        if (immune)
        {
            playerFlashTime -= Time.deltaTime;
            Flash();
            if (playerFlashTime <= 0.0f)
            {
                immune = false;
                playerFlashTime = 1.0f;
                flashTimer = 0.1f;
                GetComponentInChildren<MeshRenderer>().enabled = true;
                visible = true;
            }
        }

		if (!health.isDead()) {
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
	            isJumping = true;
	            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 60.0f);

                Physics.IgnoreLayerCollision(8, 12, true);
            }

	        if (Input.GetKey(KeyCode.Space))
	        {
	            fireBullet();
	        }
	        if (Input.GetKeyUp(KeyCode.Space))
	        {
	            fireShots();
	        }
	        if (Input.GetKeyUp(KeyCode.R))
	        {
                if (laser) { laser = false; }
	            currentWeapon++;
	            if (currentWeapon > 3)
	            {
	                currentWeapon = 0;
	            }

	        }
	        if (Input.GetKeyUp(KeyCode.E))
	        {
                if (laser) { laser = false; }
                currentWeapon--;
	            if (currentWeapon < 0)
	            {
	                currentWeapon = 3;
				}
				gm.UpdateUI(currentWeapon, bullets);
	        }

	        //shooting direction
	        if (Input.GetKeyDown(KeyCode.I))
	        {
	            shootUp = true;
	        }

	        if (Input.GetKeyUp(KeyCode.I))
	        {
	            shootUp = false;
	        }

	        if (Input.GetKeyDown(KeyCode.J))
	        {
	            shootLeft = true;
	        }

	        if (Input.GetKeyUp(KeyCode.J))
	        {
	            shootLeft = false;
	        }

	        if (Input.GetKeyDown(KeyCode.L))
	        {
	            shootRight = true;
	        }

	        if (Input.GetKeyUp(KeyCode.L))
	        {
	            shootRight = false;
	        }
		}

        // Screen Wrap
        if (transform.position.x > (bg.transform.position.x + 50f))
        {
            bg.transform.position = new Vector3(bg.transform.position.x + 100f, bg.transform.position.y, bg.transform.position.z);
        }
        else if (transform.position.x < (bg.transform.position.x - 50f))
        {
            bg.transform.position = new Vector3(bg.transform.position.x - 100f, bg.transform.position.y, bg.transform.position.z);
        }

        //laser ammo check
        if (laser)
        {
            bullets[2] -= Time.deltaTime;
            if (bullets[2] <= 0.0f)
            {
                laser = false;
                bullets[2] = 0.0f;
			}
			gm.UpdateAmo(bullets[2]);
        }

        if (isJumping)
        {
            if (rigidBody.velocity.y < 0.0f)
            {
                Physics.IgnoreLayerCollision(8, 12, false);
            }
            
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1.2f) && rigidBody.velocity.y <= 0.0f)
            {
                if (hit.distance <= 1.01f)
                {
                    isJumping = false;
                }
            }
        }
    }


    void fireBullet()
    {
        switch (currentWeapon)
        {
            default:
                break;
            case 1:
                if (bulletCool >= 0.15f && bullets[1] > 0)
                {
                    Instantiate(machineGunBulletPrefab, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                    bulletCool = 0.0f;
					bullets[1] -= 4;
					gm.UpdateAmo(bullets[1]);
				}
                break;
            case 2:
                if (!laser && bullets[2] > 0.0f)
                {
                    Instantiate(laserPrefab, new Vector3(transform.position.x, transform.position.y, 0.5f), Quaternion.identity);
					laser = true;
					gm.UpdateAmo(bullets[2]);
				}
                break;
        }
    }

    void fireShots()
    {
        switch (currentWeapon)
        {
            case 0:
                Instantiate(pistolBulletPrefab, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                break;
            case 2:
                laser = false;
                break;
            case 3:
                if (bullets[3] > 0)
                {
                    Instantiate(fireballPrefab, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
					bullets[3] -= 1;
					gm.UpdateAmo(bullets[3]);
                }
                break;
            default:
                break;
        }
    }

    void Flash()
    {
        flashTimer -= Time.deltaTime;
        if (flashTimer <= 0.0f)
        {
            if (visible)
            {
                GetComponentInChildren<MeshRenderer>().enabled = false;
                visible = false;
            }
            else
            {
                GetComponentInChildren<MeshRenderer>().enabled = true;
                visible = true;
            }
            flashTimer = 0.1f;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Enemy" && !immune && !health.isDead())
        {
            immune = true;
            health.takeDamage(5.0f);
            if (health.isDead())
            {
                gm.EndGame();
            }
        }
        else if (col.gameObject.tag == "HealthDrop")
        {
            health.gainHealth(10.0f);
            Destroy(col.gameObject);
        }
        else if (col.gameObject.tag == "MachineDrop")
        {
            bullets[1] += 100.0f;
            if (bullets[1] > 200.0f)
                bullets[1] = 200.0f;
            if (currentWeapon == 1)
				gm.UpdateAmo(bullets[1]);

            Destroy(col.gameObject);
        }
        else if (col.gameObject.tag == "LaserDrop")
        {
            bullets[2] += 5.0f;
            if (bullets[2] > 10.0f)
            bullets[2] = 10.0f;
            if (currentWeapon == 2)
				gm.UpdateAmo(bullets[2]);
            Destroy(col.gameObject);
        }
        else if (col.gameObject.tag == "FireballDrop")
        {
            bullets[3] += 5.0f;
            if (bullets[3] > 10.0f)
                bullets[3] = 10.0f;
            if (currentWeapon == 3)
                gm.UpdateAmo(bullets[3]);
            Destroy(col.gameObject);
        }

    }

    void OnCollisionStay(Collision col)
    {
		if (col.gameObject.tag == "Enemy" && !immune && !health.isDead())
        {
            immune = true;
            health.takeDamage(5.0f);
			if (health.isDead())
            {
				gm.EndGame();
            }
        }
    }

    
}


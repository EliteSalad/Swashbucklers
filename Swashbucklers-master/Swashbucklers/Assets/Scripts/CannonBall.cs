using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour {

    //CannonBall class which is attached to the player's instances of the cannon ball prefab

    //Time the local cannon ball expires
    public float ExpiryTime;
    //Time the local cannon ball was spawned
    private float StartTime;
    //The amount of damage the cannon ball will deal on impact
    private float DamageAmount;

	// Use this for initialization
	void Start () {
        //Set the start time of the cannon ball to the current game time
        StartTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time >= StartTime + ExpiryTime)
        {
            //If the cannon ball has expired
            //Play the splash sound effect
            GetComponent<AudioSource>().Play();
            //Destroy the cannon ball
            Destroy(gameObject);
        }
	}

    public void LoadDamage(float Damage)
    {
        //Function to load damage into the cannon ball from the player when spawned
        DamageAmount = Damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            //If the cannon ball collided with an enemy
            //Run the enemy's take damage function, giving it this instance's damage amount
            other.gameObject.GetComponent<EnemyAI>().TakeDamage(DamageAmount);
            //Destroy the cannon ball
            Destroy(gameObject);
        }
    }
}

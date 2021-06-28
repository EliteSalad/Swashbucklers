using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallEnemy : MonoBehaviour
{
    //CannonBall class which is attached to the enemy's instances of the cannon ball prefab

    //Time the local cannon ball expires
    public float eExpiryTime;
    //Time the local cannon ball was spawned
    private float eStartTime;
    //The amount of damage the cannon ball will deal on impact
    private float DamageAmount;

    // Use this for initialization
    void Start()
    {
        //Set the start time of the cannon ball to the current game time
        eStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= eStartTime + eExpiryTime)
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
        //Function to load damage into the cannon ball from the enemy when spawned
        DamageAmount = Damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //If the cannon ball collided with the player
            //Run the player's take damage function, giving it this instance's damage amount
            GameObject.Find("Player").GetComponent<Combat>().TakeDamage(DamageAmount);
            //Destroy the cannon ball
            Destroy(gameObject);
        }
    }
}

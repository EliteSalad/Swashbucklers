using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCrate : MonoBehaviour {

    //Script attached to the spawned loot object when an enemy has died

    //Variable to store the type of loot the crate is holding
    public string LootType;

    private void Awake()
    {
        //Start the countdown to sink the cargo in 20 seconds of it being created
        Invoke("Sink", 20.0f);
    }
    private void FixedUpdate()
    {
        //Get the distance between this instance of the cargo and the player
        float dist = Vector3.Distance(transform.position, GameObject.Find("Player").transform.position);
        if (dist <= GameObject.Find("Player").GetComponent<Combat>().LootPullDistance)
        {
            //If the distance is within the loot pulling distance defined in the player's combat script
            //Move the cargo towards the player
            transform.position = Vector3.MoveTowards(transform.position, GameObject.Find("Player").transform.position, 0.1f);
        }
    }
    void Sink()
    {
        //After 20 seconds of not being collected enabled the cargo's gravity
        GetComponent<Rigidbody>().useGravity = true;
        //Begin the countdown to destroy the gameobject
        Invoke("DestroyCrate", 5.0f);
    }
    void DestroyCrate()
    {
        //If the countdown of 5 seconds reached 0 destroy the gameobject
        Destroy(gameObject);
    }
    public string GiveType()
    {
        //Return the loot type of this instance of the loot
        return LootType;
    }
    public void TakeType(string LT)
    {
        //Give this instance of the loot crate a loot type
        LootType = LT;
    }
}

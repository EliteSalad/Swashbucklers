using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

    //EnemyAI Script which is attached to each enemy spawned and controls the movement, stats and attacks of the enemy AI
    
    //Local enemy stats:
    private float HP; //Enemy HP
    private float MHP; //Enemy Max HP
    private float Quality; //Enemy Quality
    private float CannonBallDamage; //Enemy Cannon Ball Damage
    private float turn_Speed; //Enemy turning speed

    //Bool to check whether the local enemy is dead or not
    public bool Dead;

    //Variables to control the firing of the enemy cannons (only one cooldown because they only first in one direction)
    private bool CannonCD;
    private float CannonCDTime;
    private float CannonBallSpeed;
    
    //Rigidbody's which contain the prefab to spawn on either death or attack
    public Rigidbody CannonBall;
    public Rigidbody Loot;

    //Local nav mesh agent variable
    private NavMeshAgent nav;

    //Misc variables:
    private Quaternion rotation;

    private void Awake()
    {
        //Initialise the nav mesh agent
        nav = GetComponent<NavMeshAgent>();
        //Store the spawn location for use later
        Vector3 SpawnLocation = transform.position;
        //More nav mesh agent initialisation (used to ensure the enemy is flat against the baked nav area)
        nav.Warp(SpawnLocation);
        switch (transform.name)
        {
            //Initialise the enemy ship's stats based on its name
            case "Sloop(Clone)":
                HP = 25.0f;
                CannonBallDamage = 15.0f;
                Quality = 0.8f;
                turn_Speed = 1.0f;
                break;
            case "Caravel(Clone)":
                HP = 50.0f;
                CannonBallDamage = 30.0f;
                Quality = 0.75f;
                turn_Speed = 1.2f;
                break;
            case "Brigantine(Clone)":
                HP = 90.0f;
                CannonBallDamage = 50.0f;
                Quality = 0.7f;
                turn_Speed = 1.5f;
                break;
            default:
                break;
        }
        //Set the max hp to be the current hp
        MHP = HP;
        //Set the nav mesh agent's speed to be equal to the quality
        nav.speed = Quality;
        //Set the destination preliminarily to the player's position
        nav.SetDestination(GameObject.Find("Player").transform.position);
        //Set the dead boolean to false
        Dead = false;
        //Store the spawned enemies name 3d text rotation
        rotation = gameObject.transform.GetChild(0).transform.rotation;
        //Set the cannon ball speed
        CannonBallSpeed = 10.0f;
    }
    // Update is called once per frame
    void Update ()
    {
        if (Time.time >= CannonCDTime)
        {
            //Set the cannon ball cooldown to false if the cooldown has expired
            CannonCD = false;
        }
        Vector3 pos;
        //Set the enemy's 3d text tooltip to be equal to its current health
        gameObject.transform.GetChild(0).GetComponent<TextMesh>().text = transform.name + "\nHP: " + Mathf.Round(HP) + " / " + Mathf.Round(MHP);
        //Keep the rotation of the 3d text tooltip equal to the original rotation
        gameObject.transform.GetChild(0).transform.rotation = rotation;
        //Get the current position of the enemy ship
        pos = transform.position;
        pos.y += 1;
        //Set the enemies name and health tooltip to be off-centre of the position of the ship itself
        gameObject.transform.GetChild(0).transform.position = pos;
        //Set the colour of the tooltip to be gray with slight transparency
        gameObject.transform.GetChild(0).GetComponent<TextMesh>().color = new Color32(114, 114, 114, 138);
        //Get the distance between the enemy ship and the player
        float dist = Vector3.Distance(transform.position, GameObject.Find("Player").transform.position);
        if (dist <= 10.0f && !Dead)
        {
            //If the position is within the stopping distance of the nav mesh agent and the enemy isnt dead
            nav.enabled = false;
            //Get the relative position of the direction of the player
            Vector3 lookPos = GameObject.Find("Player").transform.position - transform.position;
            lookPos.y = 0;
            //Get the rotation of the players position compared to the enemy
            Quaternion grotation = Quaternion.LookRotation(lookPos);
            //Make the new rotation 90 degrees + on the y axis
            grotation *= Quaternion.Euler(0, 90, 0);
            //Rotate the enemy ship to face the player
            transform.rotation = Quaternion.Slerp(transform.rotation, grotation, Time.deltaTime * turn_Speed);
            //Fire
            if (!CannonCD)
            {
                //If the enemy is able to fire its cannon
                Debug.Log("Fire! Left");
                //Set the ball clone rigid body to a new instance of the cannonball prefab
                Rigidbody BallClone1 = Instantiate(CannonBall, transform.position, transform.rotation);
                //Give the new projectile a velocity 
                BallClone1.velocity = -transform.right * CannonBallSpeed;
                //Give the new projectile a damage based on this instance of the enemies stats
                BallClone1.GetComponent<CannonBallEnemy>().LoadDamage
                    ((CannonBallDamage + (CannonBallDamage * Random.Range(-0.3f, 0.03f))) * Quality);
                //Set the cooldown of the cannon to true
                CannonCD = true;
                //Set the cooldown time to now plus 3 seconds
                CannonCDTime = Time.time + 3;
            }
        }
        else if(!Dead)
        {
            //If the enemy is not within the shooting range and isnt dead enable the nav mesh agent to move the enemy to the player
            nav.enabled = true;
            //Make the enemy move to the player using the nav mesh agent
            nav.SetDestination(GameObject.Find("Player").transform.position);
        }
    }

    public void TakeDamage(float Amount)
    {
        //This function is activated when the player collides with an enemy cannon ball        
        
        //Minus the damage amount from the current enemy hp
        HP -= Amount;
       
        //Used for debugging purposes
        Debug.Log("You hit.. " + transform.name + " for " + Amount + " damage.");

        //Play the cannon ball hit sound effect
        gameObject.transform.GetChild(1).GetComponent<AudioSource>().Play();
        if (HP <= 0 && !Dead)
        {
            //If the enemy hit points is less than or equal to 0 and the enemy isn't currently dead
            //Set the dead boolean to true
            Dead = true;
            //Play the sinking sound effect
            GetComponent<AudioSource>().Play();
            //Disable the 3d text tooltip
            gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            //Get the current rotation of the enemy ship
            Vector3 tipped = transform.eulerAngles;
            tipped.z += 180.0f;
            //Tip the ship upside down
            transform.eulerAngles = tipped;
            //Disable the enemies collided when sunk
            GetComponent<BoxCollider>().enabled = false;
            //Initialise temporary variables used for loot spawning
            int randLootMin = 0;
            int randLootMax = 0;
            int randLootAmount = 0;
            switch (transform.name)
            {
                //Set the maximum amount of loot depending on what type of enemy is spawning it and add gold to the players cargo
                case "Sloop(Clone)":
                    randLootMin = 0;
                    randLootMax = 2;
                    GameObject.Find("Player").GetComponent<Combat>().Gold =
                        GameObject.Find("Player").GetComponent<Combat>().Gold + (int)Random.Range(2, 8);
                    break;
                case "Caravel(Clone)":
                    randLootMin = 1;
                    randLootMax = 4;
                    GameObject.Find("Player").GetComponent<Combat>().Gold =
                        GameObject.Find("Player").GetComponent<Combat>().Gold + (int)Random.Range(4, 15);
                    break;
                case "Brigantine(Clone)":
                    randLootMin = 3;
                    randLootMax = 8;
                    GameObject.Find("Player").GetComponent<Combat>().Gold =
                        GameObject.Find("Player").GetComponent<Combat>().Gold + (int)Random.Range(8, 25);
                    break;
                default:
                    break;
            }
            //Begin spawning random loot amounts
            float randLoot = 0.0f;
            randLootAmount = Random.Range(randLootMin, randLootMax);
            for (int i = 0; i <= randLootAmount; i++)
            {
                //Get the current position of the enemy ship
                Vector3 vlootPos = transform.position;
                //Add a random amount to the x and y of the enemy ship
                vlootPos.x += Random.Range(-5.0f, 5.0f);
                vlootPos.z += Random.Range(-5.0f, 5.0f);
                //Create a new instance of the loot prefab
                Rigidbody newLoot = Instantiate(Loot, vlootPos, transform.rotation);
                switch (transform.name)
                {
                    //Give the new instance a random type based on their drop rates and enemy ship type
                    case "Sloop(Clone)":
                        randLoot = Random.Range(0.0f, 100.0f);
                        if (randLoot >= 0.0f && randLoot <= 25.0f) //Grain 25%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Grain");
                        }
                        else if (randLoot > 25.0f && randLoot <= 47.0f) //Fish 22%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Fish");
                        }
                        else if (randLoot > 47.0f && randLoot <= 65.0f) //Oil 18%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Oil");
                        }
                        else if (randLoot > 65.0f && randLoot <= 78.0f) //Wood 13%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Wood");
                        }
                        else if (randLoot > 78.0f && randLoot <= 88.0f) //Brick 10%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Brick");
                        }
                        else if (randLoot > 88.0f && randLoot <= 96.0f) //Iron 8%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Iron");
                        }
                        else if (randLoot > 96.0f && randLoot <= 100.0f) //Rum 4%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Rum");
                        }
                        break;
                    case "Caravel(Clone)":
                        randLoot = Random.Range(0.0f, 100.0f);
                        if (randLoot >= 0.0f && randLoot <= 22.0f) //Grain 22%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Grain");
                        }
                        else if (randLoot > 22.0f && randLoot <= 41.0f) //Fish 19%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Fish");
                        }
                        else if (randLoot > 41.0f && randLoot <= 56.0f) //Oil 15%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Oil");
                        }
                        else if (randLoot > 56.0f && randLoot <= 68.0f) //Wood 12%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Wood");
                        }
                        else if (randLoot > 68.0f && randLoot <= 79.0f) //Brick 11%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Brick");
                        }
                        else if (randLoot > 79.0f && randLoot <= 88.0f) //Iron 9%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Iron");
                        }
                        else if (randLoot > 88.0f && randLoot <= 94.0f) //Rum 6%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Rum");
                        }
                        else if (randLoot > 94.0f && randLoot <= 98.0f) //Silk 4%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Silk");
                        }
                        else if (randLoot > 98.0f && randLoot <= 100.0f) //Silverware 2%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Silverware");
                        }
                        break;
                    case "Brigantine(Clone)":
                        randLoot = Random.Range(0.0f, 100.0f);
                        if (randLoot >= 0.0f && randLoot <= 18.0f) //Grain 18%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Grain");
                        }
                        else if (randLoot > 18.0f && randLoot <= 34.0f) //Fish 16%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Fish");
                        }
                        else if (randLoot > 34.0f && randLoot <= 47.0f) //Oil 13%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Oil");
                        }
                        else if (randLoot > 47.0f && randLoot <= 59.0f) //Wood 12%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Wood");
                        }
                        else if (randLoot > 59.0f && randLoot <= 70.0f) //Brick 11%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Brick");
                        }
                        else if (randLoot > 70.0f && randLoot <= 80.0f) //Iron 10%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Iron");
                        }
                        else if (randLoot > 80.0f && randLoot <= 88.0f) //Rum 8%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Rum");
                        }
                        else if (randLoot > 88.0f && randLoot <= 93.0f) //Silk 5%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Silk");
                        }
                        else if (randLoot > 93.0f && randLoot <= 97.0f) //Silverware 4%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Silverware");
                        }
                        else if (randLoot > 97.0f && randLoot <= 100.0f) //Emerald 3%
                        {
                            newLoot.GetComponent<LootCrate>().TakeType("Silverware");
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

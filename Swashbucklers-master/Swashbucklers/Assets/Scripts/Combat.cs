using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class Combat : MonoBehaviour {

    //Combat script which controls everything related to the player as well as management of spawning and other scene elements

    //Misc player/camera variables to control movement:
    public Rigidbody shipRigid;
    private Quaternion rotation;
    private Vector3 pos;
    //Camera GameObject:
    private GameObject cam;

    //Various UI variables:
    private Text HHP;
    private Text SHP;
    private Text Cargo;
    private GameObject button;

    //Enemy Spawning Variables:
    public Rigidbody CannonBall;
    public Rigidbody Sloop;
    public Rigidbody Caravel;
    public Rigidbody Brigantine;
    private Rigidbody Spawned;

    //Sound effect variables:
    public AudioSource CollisionAudio;
    public AudioSource LootAudio;

    //Ship's Cannon Variables:
    private bool CannonCD1;
    private bool CannonCD2;
    private float CannonCD1Time;
    private float CannonCD2Time;
    private float CannonBallDamage;
    public float CannonBallSpeed;

    //Ship's Collision timing variables:
    private bool canCollide = true;
    private float canCollideTime = 0.0f;

    //Game-End variables:
    private bool GameOver = false;
    private float GameOverTime = 0.0f;
    private float GameEndTime = 0.0f;

    //Ship's Current Inventory items:
    public int Gold = 0;
    private int Grain = 0;
    private int Fish = 0;
    private int Oil = 0;
    private int Wood = 0;
    private int Brick = 0;
    private int Iron = 0;
    private int Rum = 0;
    private int Silk = 0;
    private int Silverware = 0;
    private int Emerald = 0;
    //Current amount of cargo:
    private int currentCargo = 0;

    //Ship's Stats:
    private float Quality;
    private float turn_speed;
    public float speed;
    public float LootPullDistance;
    float mTurn = 0.0f;

    // Use this for initialization
    void Start () {
        //Set both the left and right cannon's cooldown to ready
        CannonCD1 = false;
        CannonCD2 = false;
    }
    private void OnApplicationQuit()
    {
        //When the player closes the game, delete all PlayerPrefs
        PlayerPrefs.DeleteAll();
    }
    private void FixedUpdate()
    {
        //Get the amount the player has moved the analog stick from the centre
        mTurn = CrossPlatformInputManager.GetAxis("Horizontal");
        Debug.Log(mTurn);
        //Turn the player ships rigid body using the above joystick amount using torque to simulate an actual ship
        shipRigid.AddRelativeTorque(new Vector3(0, (mTurn * (turn_speed + 15) * Time.deltaTime), 0), ForceMode.Force);
    }
    private void Awake()
    {
        if (CannonBallDamage != PlayerPrefs.GetFloat("fDamage"))
        {
            //Check if the Cannon ball damage has been initialised, if not initialise it
            CannonBallDamage = PlayerPrefs.GetInt("fDamage");
        }
        if (Quality != PlayerPrefs.GetInt("fQuality"))
        {
            //Check if the Quality has been initialised, if not initialise it
            Quality = PlayerPrefs.GetInt("fQuality");
        }
        //Set the Players sail hp to the max sail hp
        PlayerPrefs.SetFloat("SHP", PlayerPrefs.GetFloat("MSHP"));
        //Set the Players hull hp to the max hull hp
        PlayerPrefs.SetFloat("HHP", PlayerPrefs.GetFloat("MHHP"));

        //Set locale variables to the playerprefs variables for the ships stats
        turn_speed = PlayerPrefs.GetFloat("Turn");
        CannonBallDamage = PlayerPrefs.GetFloat("fDamage");
        Cargo = GameObject.Find("Cargo").GetComponent<Text>();
        Quality = PlayerPrefs.GetFloat("fQuality");
        //Get the camera object
        cam = GameObject.Find("Main Camera");
        rotation = cam.GetComponent<Rigidbody>().transform.rotation;
        //Set the UI elements to show current hull and sail hp
        HHP = GameObject.Find("Hull Health").GetComponent<Text>();
        SHP = GameObject.Find("Sail Health").GetComponent<Text>();
        //Used for testing purposes
        Debug.Log("Quality: " + Quality + ", Turn Speed: " + turn_speed + ", Damage: " + CannonBallDamage + ", Max Hull HP: " + PlayerPrefs.GetFloat("MHHP") +
            ", Hull HP: " + PlayerPrefs.GetFloat("HHP") + ", Max Sail HP: " + PlayerPrefs.GetFloat("MSHP") + ", Sail HP: " + PlayerPrefs.GetFloat("SHP"));
        //Used for android testing purposes to track stats without a debugger
        GameObject.Find("Test").GetComponent<Text>().text = ("Quality: " + PlayerPrefs.GetFloat("fQuality") + ",\n Turn Speed: " + PlayerPrefs.GetFloat("Turn") + ",\n Damage: " + PlayerPrefs.GetFloat("fDamage") + ",\n Max Hull HP: " + PlayerPrefs.GetFloat("MHHP") +
            ",\n Hull HP: " + PlayerPrefs.GetFloat("HHP") + ",\n Max Sail HP: " + PlayerPrefs.GetFloat("MSHP") + ",\n Sail HP: " + PlayerPrefs.GetFloat("SHP"));
        //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //          Enemy Spawning
        //Get the random percentage which defines how many enemies will spawn
        float SpawnPerc = Random.Range(0.0f, 100.0f);
        if (SpawnPerc > 0.0f && SpawnPerc <= 60.0f)
        {
            //If the spawn percentage is 60%
            //Set the random amounts of individual spawns for this percentage
            float NSloop = Mathf.Round(Random.Range(2, 4));
            float NCaravel = Mathf.Round(Random.Range(0, 1));
            Vector3 SpawnPos;
            for (int i = 1; i <= NSloop; i++)
            {
                //Cycle through the amount of Sloops to spawn dependant on the random numbers generated above
                //Set the spawn position to the centre (where the player spawns)
                SpawnPos = transform.position;
                //Set x to be a random number between 5 and 20 positive or negative
                SpawnPos.x += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                SpawnPos.z += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                //Set the Spawned variable to be equal to the new ship
                Spawned = Instantiate(Sloop, SpawnPos, transform.rotation);
            }
            for (int i = 1; i <= NCaravel; i++)
            {
                //Cycle through the amount of Caravels to spawn dependant on the random numbers generated above
                //Set the spawn position to the centre (where the player spawns)
                SpawnPos = transform.position;
                //Set x to be a random number between 5 and 20 positive or negative
                SpawnPos.x += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                SpawnPos.z += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                //Set the Spawned variable to be equal to the new ship
                Spawned = Instantiate(Caravel, SpawnPos, transform.rotation);
            }
            //For debugging purposes
            Debug.Log("Spawned 60% chance (" + NSloop + " Sloops, " + NCaravel + " Caravels)");
        }
        else if (SpawnPerc > 60.0f && SpawnPerc <= 90.0f)
        {
            float NSloop = Mathf.Round(Random.Range(2, 6));
            float NCaravel = Mathf.Round(Random.Range(1, 3));
            float NBrigantine = Mathf.Round(Random.Range(0, 1));
            Vector3 SpawnPos;
            for (int i = 1; i <= NSloop; i++)
            {
                //Cycle through the amount of Sloops to spawn dependant on the random numbers generated above
                //Set the spawn position to the centre (where the player spawns)
                SpawnPos = transform.position;
                //Set x to be a random number between 5 and 20 positive or negative
                SpawnPos.x += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                SpawnPos.z += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                //Set the Spawned variable to be equal to the new ship
                Spawned = Instantiate(Sloop, SpawnPos, transform.rotation);
            }
            for (int i = 1; i <= NCaravel; i++)
            {
                //Cycle through the amount of Caravels to spawn dependant on the random numbers generated above
                //Set the spawn position to the centre (where the player spawns)
                SpawnPos = transform.position;
                //Set x to be a random number between 5 and 20 positive or negative
                SpawnPos.x += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                SpawnPos.z += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                //Set the Spawned variable to be equal to the new ship
                Spawned = Instantiate(Caravel, SpawnPos, transform.rotation);
            }
            for (int i = 1; i <= NBrigantine; i++)
            {
                //Cycle through the amount of Brigantine to spawn dependant on the random numbers generated above
                //Set the spawn position to the centre (where the player spawns)
                SpawnPos = transform.position;
                //Set x to be a random number between 5 and 20 positive or negative
                SpawnPos.x += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                SpawnPos.z += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                //Set the Spawned variable to be equal to the new ship
                Spawned = Instantiate(Brigantine, SpawnPos, transform.rotation);
            }
            //For debugging purposes
            Debug.Log("Spawned 30% chance (" + NSloop + " Sloops, " + NCaravel + " Caravels, " + NBrigantine + " Brigantines)");
        }
        else if (SpawnPerc > 90.0f && SpawnPerc <= 100.0f)
        {
            float NSloop = 7;
            float NCaravel = Mathf.Round(Random.Range(2, 6));
            float NBrigantine = 2;
            Vector3 SpawnPos;
            SpawnPos.y = 0.01000637f;
            for (int i = 1; i <= NSloop; i++)
            {
                //Cycle through the amount of Sloops to spawn dependant on the random numbers generated above
                //Set the spawn position to the centre (where the player spawns)
                SpawnPos = transform.position;
                //Set x to be a random number between 5 and 20 positive or negative
                SpawnPos.x += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                SpawnPos.z += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                //Set the Spawned variable to be equal to the new ship
                Spawned = Instantiate(Sloop, SpawnPos, transform.rotation);
            }
            for (int i = 1; i <= NCaravel; i++)
            {
                //Cycle through the amount of Caravels to spawn dependant on the random numbers generated above
                //Set the spawn position to the centre (where the player spawns)
                SpawnPos = transform.position;
                //Set x to be a random number between 5 and 20 positive or negative
                SpawnPos.x += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                SpawnPos.z += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                //Set the Spawned variable to be equal to the new ship
                Spawned = Instantiate(Caravel, SpawnPos, transform.rotation);
            }
            for (int i = 1; i <= NBrigantine; i++)
            {
                //Cycle through the amount of Brigantine to spawn dependant on the random numbers generated above
                //Set the spawn position to the centre (where the player spawns)
                SpawnPos = transform.position;
                //Set x to be a random number between 5 and 20 positive or negative
                SpawnPos.x += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                SpawnPos.z += (Mathf.Round(Random.Range(0.0f, 100.0f)) <= 50.0f ? Random.Range(5.0f, 20.0f) : Random.Range(-5.0f, -20.0f));
                //Set the Spawned variable to be equal to the new ship
                Spawned = Instantiate(Brigantine, SpawnPos, transform.rotation);
            }
            //For debugging purposes
            Debug.Log("Spawned 10% chance (" + NSloop + " Sloops, " + NCaravel + " Caravels, " + NBrigantine + " Brigantines)");
        }
    }

    // Update is called once per frame
    void Update ()
    {
        //UI Meter upgrade modifications:
        GameObject.Find("uHull").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Hull").ToString();
        GameObject.Find("uDamage").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Damage").ToString();
        GameObject.Find("uSails").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Sails").ToString();
        GameObject.Find("uStorage").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Storage").ToString();
        GameObject.Find("uQuality").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Quality").ToString();

        //Set the in-game slider to the upgrade level of each item
        GameObject.Find("uHull").transform.GetChild(1).GetComponent<Slider>().value = PlayerPrefs.GetInt("Hull");
        GameObject.Find("uDamage").transform.GetChild(1).GetComponent<Slider>().value = PlayerPrefs.GetInt("Damage");
        GameObject.Find("uSails").transform.GetChild(1).GetComponent<Slider>().value = PlayerPrefs.GetInt("Sails");
        GameObject.Find("uStorage").transform.GetChild(1).GetComponent<Slider>().value = PlayerPrefs.GetInt("Storage");
        GameObject.Find("uQuality").transform.GetChild(1).GetComponent<Slider>().value = PlayerPrefs.GetInt("Quality");

        if (Time.time >= canCollideTime)
        {
            //If the ship crash cooldown is finished set the boolean to true (allowed to take damage from collisions)
            canCollide = true;
        }
        if (PlayerPrefs.GetFloat("HHP") <= 0.0f)
        {
            //Check to see if the player has died
            //and if so set the hull hp to 0 to avoid going into negatives
            PlayerPrefs.SetFloat("HHP", 0.0f);
            //Display the lose text on the ui
            GameObject.Find("Died").GetComponent<Text>().enabled = true;
            //Call the return to management scene function after 2 seconds
            Invoke("ManagementLose", 2.0f);

        }
        //Add a constant force to the player based on the speed and sail health
        GetComponent<Rigidbody>().AddRelativeForce(((Vector3.forward * speed * Time.deltaTime) * (PlayerPrefs.GetFloat("SHP") / 100)), ForceMode.Acceleration);
        //Set the rotation of the camera to static to ensure it doesn't follow the rotation of the player
        cam.GetComponent<Rigidbody>().transform.rotation = rotation;
        //Get the position of the player and make it hover above the player by 50 units
        pos = GetComponent<Rigidbody>().transform.position;
        pos.y += 50;
        cam.transform.position = pos;
        //Set the UI's hull and sail HP to be equal to the player prefs relevant values
        HHP.text = "Hull Health: " + Mathf.Round(PlayerPrefs.GetFloat ("HHP")) + " / " + Mathf.Round(PlayerPrefs.GetFloat("MHHP"));
        SHP.text = "Sail Health: " + Mathf.Round(PlayerPrefs.GetFloat ("SHP")) + " / " + Mathf.Round(PlayerPrefs.GetFloat("MSHP"));
        //Set the cargo UI text to be equal to the relevant player prefs values
        Cargo.text = "Cargo: " + currentCargo + " / " + PlayerPrefs.GetInt("Cargo");
        if (Time.time >= CannonCD1Time)
        {
            //Check if the first cannon has cooled down, if so set the boolean to false to allow firing again
            CannonCD1 = false;
            //Set the UI text to show that the cannon is ready to fire
            GameObject.Find("CannonCD1").GetComponent<Text>().text = "Cooldown: Ready";
        }
        else
        {
            //If the cannon is still cooling down display the amount of time that is left
            GameObject.Find("CannonCD1").GetComponent<Text>().text = "Cooldown: " + (Mathf.Round(Time.time - CannonCD1Time) * -1) + "s";
        }
        if (Time.time >= CannonCD2Time)
        {
            //Check if the second cannon has cooled down, if so set the boolean to false to allow firing again
            CannonCD2 = false;
            //Set the UI text to show that the cannon is ready to fire
            GameObject.Find("CannonCD2").GetComponent<Text>().text = "Cooldown: Ready";
        }
        else
        {
            //If the cannon is still cooling down display the amount of time that is left
            GameObject.Find("CannonCD2").GetComponent<Text>().text = "Cooldown: " + (Mathf.Round(Time.time - CannonCD2Time) * -1) + "s";
        }
        if (!GameOver)
        {
            //If the gameover bool is not true cycle through all of the objects with the Enemy tag and check if their corresponding boolean for being dead is set to false
            var objects = GameObject.FindGameObjectsWithTag("Enemy");
            var objectCount = objects.Length;
            bool allDead = true;
            foreach (var obj in objects)
            {
                if (!obj.GetComponent<EnemyAI>().Dead)
                {
                    allDead = false;
                }
            }
            if (allDead)
            {
                //If all of the enemies in the scene are dead, start the countdown to return to the main menu
                GameOver = true;
                GameOverTime = Time.time;
                GameEndTime = GameOverTime + 10.0f;
                GameObject.Find("Win").GetComponent<Text>().enabled = true;
                //End the game after 10 seconds
                Invoke("ManagementWin", 10.0f);
            }
        }
        else
        {
            //If the Game is over and the player hasn't died display the return to base after 10 seconds ui textbox
            GameObject.Find("Win").GetComponent<Text>().text = "All Enemies Dead!\nReturning to base in.. "
                + Mathf.Round((Time.time - GameEndTime) * -1).ToString();
        }
	}

    //Function to fire the right cannon
    public void rFire()
    {
        if (!CannonCD2)
        {
            //If the right cannon is not on cooldown
            Vector3 Arc;
            Debug.Log("Fire! Right");
            //Spawn the first cannon ball
            Rigidbody BallClone1 = Instantiate(CannonBall, transform.position, transform.rotation);
            //Give the cannon ball a velocity to the right
            BallClone1.velocity = transform.right * CannonBallSpeed;
            Arc = transform.right;
            Arc.z += 0.45f;
            //Spawn the second cannonball
            Rigidbody BallClone2 = Instantiate(CannonBall, transform.position, transform.rotation);
            //Give the cannon ball a velocity to the right adding a 45 degree angle
            BallClone2.velocity = (Arc) * CannonBallSpeed;
            Arc = transform.right;
            Arc.z -= 0.45f;
            //Spawn the third cannonball
            Rigidbody BallClone3 = Instantiate(CannonBall, transform.position, transform.rotation);
            //Give the cannon ball a velocity to the right minusing a 45 degree angle
            BallClone3.velocity = (Arc) * CannonBallSpeed;
            //Set the cannonball fired boolean to true
            CannonCD2 = true;
            //Set the cooldown time to now plus 3 seconds
            CannonCD2Time = Time.time + 3;
            //Play the fire sound effect
            gameObject.transform.GetChild(1).GetComponent<AudioSource>().Play();
            //Give the cannonballs damage based on the formula supplied
            BallClone1.GetComponent<CannonBall>().LoadDamage
                ((CannonBallDamage + (CannonBallDamage * Random.Range(-0.3f, 0.03f))) * Quality);
            BallClone2.GetComponent<CannonBall>().LoadDamage
                ((CannonBallDamage + (CannonBallDamage * Random.Range(-0.3f, 0.03f))) * Quality);
            BallClone3.GetComponent<CannonBall>().LoadDamage
                ((CannonBallDamage + (CannonBallDamage * Random.Range(-0.3f, 0.03f))) * Quality);
        }
    }

    //Function to fire the left cannon
    public void lFire()
    {
        if (!CannonCD1)
        {
            //If the left cannon is not on cooldown
            Vector3 Arc;
            Debug.Log("Fire! Left");
            //Spawn the first cannon ball
            Rigidbody BallClone1 = Instantiate(CannonBall, transform.position, transform.rotation);
            //Give the cannon ball a velocity to the right
            BallClone1.velocity = -transform.right * CannonBallSpeed;
            Arc = -transform.right;
            Arc.z += 0.45f;
            //Spawn the second cannonball
            Rigidbody BallClone2 = Instantiate(CannonBall, transform.position, transform.rotation);
            BallClone2.velocity = (Arc) * CannonBallSpeed;
            Arc = -transform.right;
            Arc.z -= 0.45f;
            //Spawn the third cannonball
            Rigidbody BallClone3 = Instantiate(CannonBall, transform.position, transform.rotation);
            //Give the cannon ball a velocity to the right minusing a 45 degree angle
            BallClone3.velocity = (Arc) * CannonBallSpeed;
            //Set the cannonball fired boolean to true
            CannonCD1 = true;
            //Set the cooldown time to now plus 3 seconds
            CannonCD1Time = Time.time + 3;
            //Play the fire sound effect
            gameObject.transform.GetChild(2).GetComponent<AudioSource>().Play();
            //Give the cannonballs damage based on the formula supplied
            BallClone1.GetComponent<CannonBall>().LoadDamage
                ((CannonBallDamage + (CannonBallDamage * Random.Range(-0.3f, 0.03f))) * Quality);
            BallClone2.GetComponent<CannonBall>().LoadDamage
                ((CannonBallDamage + (CannonBallDamage * Random.Range(-0.3f, 0.03f))) * Quality);
            BallClone3.GetComponent<CannonBall>().LoadDamage
                ((CannonBallDamage + (CannonBallDamage * Random.Range(-0.3f, 0.03f))) * Quality);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && canCollide)
        {
            //If the player collided with an enemy
            Debug.Log("Crash!");
            //Minus half of the max hull hp from the current hp
            PlayerPrefs.SetFloat("HHP",PlayerPrefs.GetFloat("HHP") - (PlayerPrefs.GetFloat("MHHP") / 2));
            //Play the collision sound effect
            CollisionAudio.Play();
            //Toggle the collide boolean
            canCollide = false;
            //Set the cooldown float
            canCollideTime = Time.time + 10.0f;
        }
        else if (other.CompareTag("Loot"))
        {
            //If the player collided with loot
            //Play the loot audio clip
            LootAudio.Play();
            if (currentCargo < PlayerPrefs.GetInt("Cargo"))
            {
                //If there is space enough for cargo
                Debug.Log("Looted!");
                //Get the loot type from the object the player collided with
                string LootType = other.GetComponent<LootCrate>().GiveType();
                //Switch to find the loot type and add to the local loot variable counter
                switch (LootType)
                {
                    case "Grain":
                        ++Grain;
                        break;
                    case "Fish":
                        ++Fish;
                        break;
                    case "Oil":
                        ++Oil;
                        break;
                    case "Wood":
                        ++Wood;
                        break;
                    case "Brick":
                        ++Brick;
                        break;
                    case "Iron":
                        ++Iron;
                        break;
                    case "Rum":
                        ++Rum;
                        break;
                    case "Silk":
                        ++Silk;
                        break;
                    case "Silverware":
                        ++Silverware;
                        break;
                    case "Emerald":
                        ++Emerald;
                        break;
                    default:
                        break;
                }
                //Add one to the cargo amount
                ++currentCargo;
                //Destroy the cargo item
                Destroy(other.gameObject);
            }
            else
            {
                //For debugging purposes display that there is no room for the cargo looted
                Debug.Log("Cargo Full!");
            }
        }
    }
    public void TakeDamage(float Amount)
    {
        //Called by enemy cannon balls when they collide with the players ship
        if (Random.Range(0.0f, 100.0f) <= 20.0f)
        {
            //If the random number generated is 20% then apply damage to the sail health instead of the hull hp
            PlayerPrefs.SetFloat("SHP", PlayerPrefs.GetFloat("SHP") - Amount);
        }
        else
        {
            //if it's 80% then apply to the hull hp
            PlayerPrefs.SetFloat("HHP", PlayerPrefs.GetFloat("HHP") - Amount);
        }
        //Play the take damage soundclip from the correct child
        gameObject.transform.GetChild(3).GetComponent<AudioSource>().Play();
        //Debug to display the amount the player was damaged for
        Debug.Log("You were hit.. for " + Amount + " damage.");
        if (PlayerPrefs.GetFloat("HHP") <= 0.0f)
        { 
            //If the health is less than or equal to 0 the player died
            //Set the hp to 0 to prevent it from going below 0
            PlayerPrefs.SetFloat("HHP", 0.0f);
            //Show the failed ui text component to signify failure
            GameObject.Find("Died").GetComponent<Text>().enabled = true;
            //Move to the management menu after 2 seconds
            Invoke("ManagementLose", 2.0f);
        }
        if (PlayerPrefs.GetFloat("SHP") <= 0.0f)
        {
            //Prevent the sails hp from going below 0
            PlayerPrefs.SetFloat("SHP", 0.0f);
        }
    }
    void ManagementLose()
    {
        //Called when the player dies
        //Set all local cargo items to 0 (delete them)
        Grain = 0;
        Fish = 0;
        Oil = 0;
        Wood = 0;
        Brick = 0;
        Iron = 0;
        Rum = 0;
        Silk = 0;
        Silverware = 0;
        Emerald = 0;
        //Remove 25% from the current gold amount
        float currGold = PlayerPrefs.GetInt("Gold") * 0.75f;
        //Set gold amount to new value
        PlayerPrefs.SetInt("Gold", (int)Mathf.Round(currGold));
        //Load the management scene
        SceneManager.LoadScene(0);
    }
    void ManagementWin()
    {
        //Call when the player wins
        //Set all relevant playerpref values to the local cargo (save them)
        PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + Gold);
        PlayerPrefs.SetInt("Grain", PlayerPrefs.GetInt("Grain") + Grain);
        PlayerPrefs.SetInt("Fish", PlayerPrefs.GetInt("Fish") + Fish);
        PlayerPrefs.SetInt("Oil", PlayerPrefs.GetInt("Oil") + Oil);
        PlayerPrefs.SetInt("Wood", PlayerPrefs.GetInt("Wood") + Wood);
        PlayerPrefs.SetInt("Brick", PlayerPrefs.GetInt("Brick") + Brick);
        PlayerPrefs.SetInt("Iron", PlayerPrefs.GetInt("Iron") + Iron);
        PlayerPrefs.SetInt("Rum", PlayerPrefs.GetInt("Rum") + Rum);
        PlayerPrefs.SetInt("Silk", PlayerPrefs.GetInt("Silk") + Silk);
        PlayerPrefs.SetInt("Silverware", PlayerPrefs.GetInt("Silverware") + Silverware);
        PlayerPrefs.SetInt("Emerald", PlayerPrefs.GetInt("Emerald") + Emerald);
        //Load the management scene
        SceneManager.LoadScene(0);
    }
}

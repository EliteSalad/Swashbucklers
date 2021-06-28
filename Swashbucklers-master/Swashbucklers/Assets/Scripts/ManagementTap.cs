using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class ManagementTap : MonoBehaviour {

    //Main Management controller script (Attached to main camera object)

    //Various items relating to the in-game UI (In-world and Canvas)
        //Canvas Object(s):
    public GameObject UI; //UI Canvas objects (Used to hide and display when clicking on the below world objects
        //In-world Objects:
    public Text UITitle; //Title 3D scene text object
    public GameObject Combat; //In-game object for the Combat UI function
    public GameObject Shipyard; //In-game object for the Shipyard UI function
    public GameObject Market; //In-game object for the Market UI function

    //Player AI-controlled ship variables:
    private NavMeshAgent nav; //Nav Mesh agent variable for the non-controlled player AI object
    public string currLocation; //Current location of the above player AI object
    //These booleans are used to check to see if the ship in the menu has made it to the first checkpoint along the path to each destination:
    private bool mVia = false; //Boolean used for the play AI route (Market route)
    private bool sVia = false; //Boolean used for the play AI route (Shipyard route)
    public bool cTar = false; //Boolean used for the play AI route (Starting combat route)

    private void Awake()
    {
        //Initialise variables:
        nav = GameObject.Find("MenuPlayer").GetComponent<NavMeshAgent>();
        //First initialisation of PlayerPref variables:
        if(!PlayerPrefs.HasKey("fDamage")) PlayerPrefs.SetFloat("fDamage", 20.0f);
        if(!PlayerPrefs.HasKey("fQuality")) PlayerPrefs.SetFloat("fQuality", 0.95f);
        //Set the current location of the AI-controlled player ship to the start:
        currLocation = "Start";
    }
    private void OnApplicationQuit()
    {
        //When closing the application delete all player save-data:
        PlayerPrefs.DeleteAll();
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod()
    {
        //When loading the scene check the PlayerPref settings and initialise them if necessary
        if(!PlayerPrefs.HasKey("MHHP")) PlayerPrefs.SetFloat("MHHP", 50.0f); //Max Hull-HP
        if(!PlayerPrefs.HasKey("MSHP")) PlayerPrefs.SetFloat("MSHP", 100.0f); //Max Sail-HP
        if(!PlayerPrefs.HasKey("Turn")) PlayerPrefs.SetFloat("Turn", 1.1f); //Turn speed of in-game shit (Combat scene)
        if(!PlayerPrefs.HasKey("fDamage")) PlayerPrefs.SetFloat("fDamage", 20.0f); //Cannon-ball base damage
        if(!PlayerPrefs.HasKey("fQuality")) PlayerPrefs.SetFloat("fQuality", 0.95f); //Combat-scene player Quality
        if(!PlayerPrefs.HasKey("Cargo")) PlayerPrefs.SetInt("Cargo", 10); //Combat-scene Cargo max capacity

        //Shipyard Upgrade PlayerPref Variables:
        if(!PlayerPrefs.HasKey("Sails")) PlayerPrefs.SetInt("Sails", 1); 
        if(!PlayerPrefs.HasKey("Damage")) PlayerPrefs.SetInt("Damage", 1);
        if(!PlayerPrefs.HasKey("Hull")) PlayerPrefs.SetInt("Hull", 1);
        if(!PlayerPrefs.HasKey("Storage")) PlayerPrefs.SetInt("Storage", 1);
        if(!PlayerPrefs.HasKey("Quality")) PlayerPrefs.SetInt("Quality", 1);

        //Initialise all Inventory Items to 0
        if(!PlayerPrefs.HasKey("Gold")) PlayerPrefs.SetInt("Gold", 0);
        if(!PlayerPrefs.HasKey("Grain")) PlayerPrefs.SetInt("Grain", 0);
        if(!PlayerPrefs.HasKey("Fish")) PlayerPrefs.SetInt("Fish", 0);
        if(!PlayerPrefs.HasKey("Oil")) PlayerPrefs.SetInt("Oil", 0);
        if(!PlayerPrefs.HasKey("Wood")) PlayerPrefs.SetInt("Wood", 0);
        if(!PlayerPrefs.HasKey("Brick")) PlayerPrefs.SetInt("Brick", 0);
        if(!PlayerPrefs.HasKey("Iron")) PlayerPrefs.SetInt("Iron", 0);
        if(!PlayerPrefs.HasKey("Rum")) PlayerPrefs.SetInt("Rum", 0);
        if(!PlayerPrefs.HasKey("Silk")) PlayerPrefs.SetInt("Silk", 0);
        if(!PlayerPrefs.HasKey("Silverware")) PlayerPrefs.SetInt("Silverware", 0);
        if(!PlayerPrefs.HasKey("Emerald")) PlayerPrefs.SetInt("Emerald", 0);
        
        //Save all created PlayerPref variables:
        PlayerPrefs.Save();
    }
    private void Start()
    {
        //On starting the scene set the player's Hull HP and Sail HP to be their Max values
        PlayerPrefs.SetFloat("HHP", PlayerPrefs.GetFloat("MHHP"));
        PlayerPrefs.SetFloat("SHP", PlayerPrefs.GetFloat("MSHP"));
        PlayerPrefs.Save();
    }
    // Update is called once per frame
    void Update ()
    {
        //Test is primarily used to debug on android and displays all necessary variables on the Canvas at run-time which effect gameplay. (Currently in-active in both scenes)
        GameObject.Find("Test").GetComponent<Text>().text = ("Quality: " + PlayerPrefs.GetFloat("fQuality") + ",\n Turn Speed: " + PlayerPrefs.GetFloat("Turn") + ",\n Damage: " + PlayerPrefs.GetFloat("fDamage") + ",\n Max Hull HP: " + PlayerPrefs.GetFloat("MHHP") +
            ",\n Hull HP: " + PlayerPrefs.GetFloat("HHP") + ",\n Max Sail HP: " + PlayerPrefs.GetFloat("MSHP") + ",\n Sail HP: " + PlayerPrefs.GetFloat("SHP"));
        //Update the gold amount displayed to the player in the management scene
        GameObject.Find("Gold").GetComponent<Text>().text = PlayerPrefs.GetInt("Gold") + " Gold";
        //The following IF statements query whether the UI is active (displayed) as well as the sub-category and if so update the UI text:
            //Update Market UI text with relevant PlayerPref variables:
        if (UI.activeInHierarchy == true && Market.activeInHierarchy == true)
        {
            //Display the amount of loot in the ships cargo
            GameObject.Find("Grain").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Grain").ToString();
            GameObject.Find("Fish").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Fish").ToString();
            GameObject.Find("Oil").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Oil").ToString();
            GameObject.Find("Wood").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Wood").ToString();
            GameObject.Find("Brick").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Brick").ToString();
            GameObject.Find("Iron").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Iron").ToString();
            GameObject.Find("Rum").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Rum").ToString();
            GameObject.Find("Silk").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Silk").ToString();
            GameObject.Find("Silverware").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Silverware").ToString();
            GameObject.Find("Emerald").transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetInt("Emerald").ToString();
        }
            //Update Shipyard UI text with relevant PlayerPref variables:
        if (UI.activeInHierarchy == true && Shipyard.activeInHierarchy == true)
        {
            //Display the current upgrade level for each category
            GameObject.Find("Sails").transform.GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetInt("Sails").ToString() + " / 10";
            GameObject.Find("CBDamage").transform.GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetInt("Damage").ToString() + " / 10";
            GameObject.Find("Hull").transform.GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetInt("Hull").ToString() + " / 10";
            GameObject.Find("Storage").transform.GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetInt("Storage").ToString() + " / 10";
            GameObject.Find("Quality").transform.GetChild(2).GetComponent<Text>().text = PlayerPrefs.GetInt("Quality").ToString() + " / 10";

            //Display cost of upgrade for each upgrade category
            GameObject.Find("Sails").transform.GetChild(4).GetComponent<Text>().text = 
                PlayerPrefs.GetInt("Sails") < 10 ? (PlayerPrefs.GetInt("Sails") * 5).ToString() + "g" : "MAX";

            GameObject.Find("CBDamage").transform.GetChild(4).GetComponent<Text>().text =
                PlayerPrefs.GetInt("Damage") < 10 ? (PlayerPrefs.GetInt("Damage") * 6).ToString() + "g" : "MAX";

            GameObject.Find("Hull").transform.GetChild(4).GetComponent<Text>().text =
                PlayerPrefs.GetInt("Hull") < 10 ? (PlayerPrefs.GetInt("Hull") * 7).ToString() + "g" : "MAX";

            GameObject.Find("Storage").transform.GetChild(4).GetComponent<Text>().text =
                PlayerPrefs.GetInt("Storage") < 10 ? (PlayerPrefs.GetInt("Storage") * 8).ToString() + "g" : "MAX";

            GameObject.Find("Quality").transform.GetChild(4).GetComponent<Text>().text =
                PlayerPrefs.GetInt("Quality") < 10 ? (PlayerPrefs.GetInt("Quality") * 9).ToString() + "g" : "MAX";

        }
        //Move menu AI-controlled ship:
        if (cTar) //If the Sea Raid is about to begin
        {
            //Set the destination to be the SeaRaid waypoint, placed in the scene
            nav.SetDestination(GameObject.Find("SeaRaid").transform.position);
        }
        else //If the Sea Raid is not starting
        {
            if (currLocation == "Start")
            {
                //If the current location of the UI ship is the start set its destination to the start
                nav.SetDestination(GameObject.Find("StartLocation").transform.position);
                //Set both booleans to false
                mVia = false;
                sVia = false;
            }
            else if (currLocation == "Market1") 
            {
                //If the location is the market then check whether the ship is close to the way-point and if so set the boolean to true
                if (Vector3.Distance(GameObject.Find("MenuPlayer").transform.position, GameObject.Find("MarketViaLocation").transform.position) < 5.0f)
                {
                    mVia = true;
                }
                if (!mVia) 
                {
                    //If the ship hasn't reached the waypoint go to it
                    nav.SetDestination(GameObject.Find("MarketViaLocation").transform.position);
                }
                else
                {
                    //Else if the ship has reached the waypoint, dock at the Market
                    nav.SetDestination(GameObject.Find("MarketLocation").transform.position);
                }
            }
            else if (currLocation == "Shipyard1")
            {
                //If the location is the Shipyard then check whether the ship is close to the way-point and if so set the boolean to true
                if (Vector3.Distance(GameObject.Find("MenuPlayer").transform.position, GameObject.Find("ShipyardViaLocation").transform.position) < 5.0f)
                {
                    mVia = true;
                }
                if (!mVia)
                {
                    //If the ship hasn't reached the waypoint go to it
                    nav.SetDestination(GameObject.Find("ShipyardViaLocation").transform.position);
                }
                else
                {
                    //Else if the ship has reached the waypoint, dock at the Shipyard
                    nav.SetDestination(GameObject.Find("ShipyardLocation").transform.position);
                }
            }
        }
        //PC Input:
        if (Input.GetMouseButtonDown(0)) //If the mouse is clicked
        {
            //Initialise raycast variables
            RaycastHit hit;
            Ray mousePosition;
            //Set the mouse position ray to be equal to the mouse position on the screen as a raycast location
            mousePosition = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mousePosition, out hit) && UI.activeInHierarchy == false) //Fire the raycast if the menu isn't open
            {
                if (hit.transform.tag == "Market" || hit.transform.tag == "Shipyard" || hit.transform.tag == "Combat")
                {
                    //If the hit object from the raycast is either the market, shipyard or combat then
                    GameObject.Find("UIManager").GetComponent<UIFunctions>().ClickObj(); //Play UI clicking sound effect
                    //Set the UI Parent GameObject to be visible
                    UI.SetActive(true);
                    //Set all of the child UI sub-items to be hidden
                    Combat.SetActive(false);
                    Shipyard.SetActive(false);
                    Market.SetActive(false);
                }
                if (hit.transform.tag == "Market")
                {
                    //If player clicked the market
                    Debug.Log("Market");
                    UITitle.text = "Market";
                    //Set the AI-controlled player ship to the Market
                    currLocation = "Market1";
                    //Display the market UI sub-item
                    Market.SetActive(true);
                }
                else if (hit.transform.tag == "Shipyard")
                {
                    //If player clicked shipyard
                    Debug.Log("Shipyard");
                    UITitle.text = "Shipyard";
                    //Set the AI-controlled player ship to the Shipyard
                    currLocation = "Shipyard1";
                    //Display the Shipyard UI sub-item
                    Shipyard.SetActive(true);
                }
                else if (hit.transform.tag == "Combat")
                {
                    //If the player clicked combat
                    Debug.Log("Combat");
                    UITitle.text = "Combat";
                    //Set the AI-controlled player ship to the Combat area
                    currLocation = "Combat";
                    //Display the Combat UI sub-item
                    Combat.SetActive(true);
                }
            }
            else if (UI.activeInHierarchy == false)
            {
                //Return the AI controlled ship to the start if nothing was selected
                currLocation = "Start";
            }
        }
        //Android Input:
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            //Initialise raycast variables
            RaycastHit hit;
            Ray mousePosition;
            //Set the mouse position ray to be equal to the touch position on the screen as a raycast location
            mousePosition = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);

            if (Physics.Raycast(mousePosition, out hit) && UI.activeInHierarchy == false) //Fire the raycast if the menu isn't open
            {
                if (hit.transform.tag == "Market" || hit.transform.tag == "Shipyard" || hit.transform.tag == "Combat")
                {
                    //If the hit object from the raycast is either the market, shipyard or combat then
                    GameObject.Find("UIManager").GetComponent<UIFunctions>().ClickObj(); //Play UI clicking sound effect
                    //Set the UI Parent GameObject to be visible
                    UI.SetActive(true);
                    //Set all of the child UI sub-items to be hidden
                    Combat.SetActive(false);
                    Shipyard.SetActive(false);
                    Market.SetActive(false);
                }
                if (hit.transform.tag == "Market")
                {
                    //If player clicked the market
                    Debug.Log("Market");
                    UITitle.text = "Market";
                    //Set the AI-controlled player ship to the Market
                    currLocation = "Market1";
                    //Display the market UI sub-item
                    Market.SetActive(true);
                }
                else if (hit.transform.tag == "Shipyard")
                {
                    //If player clicked shipyard
                    Debug.Log("Shipyard");
                    UITitle.text = "Shipyard";
                    //Set the AI-controlled player ship to the Shipyard
                    currLocation = "Shipyard1";
                    //Display the Shipyard UI sub-item
                    Shipyard.SetActive(true);
                }
                else if (hit.transform.tag == "Combat")
                {
                    //If the player clicked combat
                    Debug.Log("Combat");
                    UITitle.text = "Combat";
                    //Set the AI-controlled player ship to the Combat area
                    currLocation = "Combat";
                    //Display the Combat UI sub-item
                    Combat.SetActive(true);
                }
            }
            else if (UI.activeInHierarchy == false)
            {
                //Return the AI controlled ship to the start if nothing was selected
                currLocation = "Start";
            }
        }
    }
}

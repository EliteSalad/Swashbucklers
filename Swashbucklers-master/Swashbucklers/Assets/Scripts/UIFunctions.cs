using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AI;

public class UIFunctions : MonoBehaviour {

    //UI Button controls and Misc UI control functions (Attached to UI Manager GameObject)

    //Sound effect objects:
    public AudioSource UpgradeStuff;
    public AudioSource SellStuff;
    public AudioSource Click;
    public AudioSource CloseW;

    //Canvas objects:
    private GameObject UI;
    private GameObject Music;
    private GameObject MM;

    //Floats to store & display the combat scene loading countdown
    private float CurrTime = 0.0f;
    private float StartTime = 0.0f;
    //Boolean to check if the game is starting (combat scene being loaded)
    private bool Starting = false;
	
	// Update is called once per frame
	void Update () {
        if (Starting)
        {
            //If the Combat scene is about to be loaded display the countdown to it on the relevant UI element:
            GameObject.Find("xCombat").transform.GetChild(2).GetComponent<Text>().text = "Starting Combat in.. " + Mathf.Round(StartTime - Time.time).ToString();
        }
	}
    void Start()
    {
        //Initialise game-object variable on start:
        MM = GameObject.Find("EventSystem");
        Music = MM.transform.GetChild(0).gameObject;
    }
    //Button Controls:

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //          Miscellaneous button controls:
    public void CloseUI()
    {
        //Function which closes the UI if it's open
        //Play the close window sound effect
        CloseW.Play();
        UI = GameObject.Find("UI");
        Debug.Log("UI Closed");
        //If the UI has been closed set the current location of the AI-controlled player ship to the start
        GameObject.Find("Main Camera").GetComponent<ManagementTap>().currLocation = "Start";
        //Hide the UI:
        UI.SetActive(false);
    }
    public void StartCombat()
    {
        //Function to start combat (Activated when player clicks yes to starting combat)
        //Play generic click UI element click sound
        Click.Play();
        //De-activate the buttons on the UI (Yes / No buttons)
        GameObject.Find("xCombat").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("xCombat").transform.GetChild(1).gameObject.SetActive(false);
        //Set the countdown text initially (TBC by update in this class (above))
        GameObject.Find("xCombat").transform.GetChild(2).GetComponent<Text>().text = "Starting Combat in.. 5";
        //Set the starting boolean to true
        Starting = true;
        //Set the current time variable to be the time since the game started.
        CurrTime = Time.time;
        //Set the start time variable to be the time since the game started + 5 seconds. (primarily to allow the AI-controlled player ship to move upwards and give the player some time to prepare for battle)
        StartTime = Time.time + 5.0f;
        //Set the speed of the ship to a huge amount
        GameObject.Find("MenuPlayer").GetComponent<NavMeshAgent>().speed = 100;
        //Set the Moving to combat target waypoint boolean in the management class to true
        GameObject.Find("Main Camera").GetComponent<ManagementTap>().cTar = true;
        //switch scene to combat scene after 5 seconds
        Invoke("BeginGame", 5.0f);
    }
    void BeginGame()
    {
        //Used in StartCombat to load the scene using Invoke.
        SceneManager.LoadScene(1);
    }
    public void ClickObj()
    {
        //Used as a public function to allow other functions to use the attached click sound effect remotely.
        Click.Play();
    }
    public void ToggleMusic()
    {
        //Toggles the music on/off
        //Play generic click UI sound effect
        Click.Play();
        //Reverse the objects sound enabled status to enable or disable music
        Music.SetActive(!Music.activeInHierarchy);
    }
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //          Shipyard Button controls:

        //Upgrade button generic comments:
            //Click.Play();         - Plays Generic UI Button Click sound effect
            //If statement          - Checks if the users gold amount is greater than the cost of the item the player is trying to upgrade
                //UpgradeStuff.Play();  - Plays the successful upgrade item sound effect
                //SetInt #1             - Sets the players gold to be X amount lower than it is now dependant on the upgrade cost of that item
                //SetInt #2             - Increments upgrade level of item
            //SetFloat              - Sets the float (which is used in the combat scene) to the appropriate new amount based on the upgrade value of that item

    public void UpgradeSails()
    {
        Click.Play();
        if ((PlayerPrefs.GetInt("Gold") >= (PlayerPrefs.GetInt("Sails") * 5)) && (PlayerPrefs.GetInt("Sails") < 10))
        {
            UpgradeStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") - (PlayerPrefs.GetInt("Sails") * 5));
            PlayerPrefs.SetInt("Sails", PlayerPrefs.GetInt("Sails") + 1);
        }
        PlayerPrefs.SetFloat("Turn", ((PlayerPrefs.GetInt("Sails") / 10) + 1));
    }
    public void UpgradeDamage()
    {
        Click.Play();
        if ((PlayerPrefs.GetInt("Gold") >= (PlayerPrefs.GetInt("Damage") * 6)) && (PlayerPrefs.GetInt("Damage") < 10))
        {
            UpgradeStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") - (PlayerPrefs.GetInt("Damage") * 6));
            PlayerPrefs.SetInt("Damage", PlayerPrefs.GetInt("Damage") + 1);
        }
        PlayerPrefs.SetFloat("fDamage", (PlayerPrefs.GetInt("Damage") * 5) + 15);
    }
    public void UpgradeHull()
    {
        Click.Play();
        if ((PlayerPrefs.GetInt("Gold") >= (PlayerPrefs.GetInt("Hull") * 7)) && (PlayerPrefs.GetInt("Hull") < 10))
        {
            UpgradeStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") - (PlayerPrefs.GetInt("Hull") * 7));
            PlayerPrefs.SetInt("Hull", PlayerPrefs.GetInt("Hull") + 1);
        }
        PlayerPrefs.SetFloat("MHHP", ((PlayerPrefs.GetInt("Hull") - 1) * 10) + (PlayerPrefs.GetInt("Hull") <= 1 ? 50 : 60));
        PlayerPrefs.SetFloat("HHP", PlayerPrefs.GetFloat("MHHP"));
    }
    public void UpgradeStorage()
    {
        Click.Play();
        if ((PlayerPrefs.GetInt("Gold") >= (PlayerPrefs.GetInt("Storage") * 8)) && (PlayerPrefs.GetInt("Storage") < 10))
        {
            UpgradeStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") - (PlayerPrefs.GetInt("Storage") * 8));
            PlayerPrefs.SetInt("Storage", PlayerPrefs.GetInt("Storage") + 1);
        }
        PlayerPrefs.SetInt("Cargo", (PlayerPrefs.GetInt("Storage") * 5) + 5);
    }
    public void UpgradeQuality()
    {
        Click.Play();
        if ((PlayerPrefs.GetInt("Gold") >= (PlayerPrefs.GetInt("Quality") * 9)) && (PlayerPrefs.GetInt("Quality") < 10))
        {
            UpgradeStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") - (PlayerPrefs.GetInt("Quality") * 9));
            PlayerPrefs.SetInt("Quality", PlayerPrefs.GetInt("Quality") + 1);
        }
        PlayerPrefs.SetFloat("fQuality", 1.0f - (PlayerPrefs.GetInt("Quality") * 0.05f));
    }

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //          Market Button Controls:
    public void SellAll()
    {
        //This function sells all of the players cargo items based on the table found in the brief of the assignment
        //Play the generic ui click sound effect
        Click.Play();
        //Checks if there is even anything to sell:
        if (PlayerPrefs.GetInt("Grain") > 0 || 
            PlayerPrefs.GetInt("Fish") > 0 ||
            PlayerPrefs.GetInt("Oil") > 0 ||
            PlayerPrefs.GetInt("Wood") > 0 ||
            PlayerPrefs.GetInt("Brick") > 0 ||
            PlayerPrefs.GetInt("Iron") > 0 ||
            PlayerPrefs.GetInt("Rum") > 0 ||
            PlayerPrefs.GetInt("Silk") > 0 ||
            PlayerPrefs.GetInt("Silverware") > 0 ||
            PlayerPrefs.GetInt("Emerald") > 0)
        {
            //Play the selling sound effect if there was something to be sold
            SellStuff.Play();

            //Sets the players gold amount to the current value plus whatever they've earned by selling the cargo
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 
                (PlayerPrefs.GetInt("Grain") * 1) +
                (PlayerPrefs.GetInt("Fish") * 2) +
                (PlayerPrefs.GetInt("Oil") * 3) +
                (PlayerPrefs.GetInt("Wood") * 5) +
                (PlayerPrefs.GetInt("Brick") * 8) +
                (PlayerPrefs.GetInt("Iron") * 10) +
                (PlayerPrefs.GetInt("Rum") * 15) +
                (PlayerPrefs.GetInt("Silk") * 20) +
                (PlayerPrefs.GetInt("Silverware") * 30) +
                (PlayerPrefs.GetInt("Emerald") * 50) 
                );

            //Set the current cargo amount to 0
            PlayerPrefs.SetInt("Grain", 0);
            PlayerPrefs.SetInt("Fish", 0);
            PlayerPrefs.SetInt("Oil", 0);
            PlayerPrefs.SetInt("Wood", 0);
            PlayerPrefs.SetInt("Brick", 0);
            PlayerPrefs.SetInt("Iron", 0);
            PlayerPrefs.SetInt("Rum", 0);
            PlayerPrefs.SetInt("Silk", 0);
            PlayerPrefs.SetInt("Silverware", 0);
            PlayerPrefs.SetInt("Emerald", 0);
        }
    }
        //Generic Selling comments:
            //Sell one:
                //Click.Play();         - Play the generic UI clicking sound effect
                //If statement          - Check if there is any of the relevant item to be sold
                    //SellStuff.Play();         - Play the selling sound effect
                    //SetInt #1                 - Decrease the relevant cargo item by 1
                    //SetInt #2                 - Increase the gold based on that items value

            //Set all:
                //Click.Play();         - Play the generic UI clicking sound effect
                //If statement          - Check if there is any of the relevant item to be sold
                    //SellStuff.Play();         - Play the selling sound effect
                    //SetInt #1                 - Increase the players gold based on the value of the item(s) sold and the worth of each item
                    //SetInt #2                 - Set the amount of that cargo to 0

    public void GrainSellOne()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Grain") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Grain", PlayerPrefs.GetInt("Grain") - 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 1);
        }
    }
    public void GrainSellAll()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Grain") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + PlayerPrefs.GetInt("Grain"));
            PlayerPrefs.SetInt("Grain", 0);
        }
    }
    public void FishSellOne()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Fish") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Fish", PlayerPrefs.GetInt("Fish") - 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 2);
        }
    }
    public void FishSellAll()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Fish") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + (PlayerPrefs.GetInt("Fish") * 2));
            PlayerPrefs.SetInt("Fish", 0);
        }
    }
    public void OilSellOne()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Oil") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Oil", PlayerPrefs.GetInt("Oil") - 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 3);
        }
    }
    public void OilSellAll()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Oil") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + (PlayerPrefs.GetInt("Oil") * 3));
            PlayerPrefs.SetInt("Oil", 0);
        }
    }
    public void WoodSellOne()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Wood") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Wood", PlayerPrefs.GetInt("Wood") - 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 5);
        }
    }
    public void WoodSellAll()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Wood") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + (PlayerPrefs.GetInt("Wood") * 5));
            PlayerPrefs.SetInt("Wood", 0);
        }
    }
    public void BrickSellOne()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Brick") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Brick", PlayerPrefs.GetInt("Brick") - 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 8);
        }
    }
    public void BrickSellAll()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Brick") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + (PlayerPrefs.GetInt("Brick") * 8));
            PlayerPrefs.SetInt("Brick", 0);
        }
    }
    public void IronSellOne()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Iron") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Iron", PlayerPrefs.GetInt("Iron") - 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 10);
        }
    }
    public void IronSellAll()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Iron") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + (PlayerPrefs.GetInt("Iron") * 10));
            PlayerPrefs.SetInt("Iron", 0);
        }
    }
    public void RumSellOne()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Rum") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Rum", PlayerPrefs.GetInt("Rum") - 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 15);
        }
    }
    public void RumSellAll()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Rum") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + (PlayerPrefs.GetInt("Rum") * 15));
            PlayerPrefs.SetInt("Rum", 0);
        }
    }
    public void SilkSellOne()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Silk") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Silk", PlayerPrefs.GetInt("Silk") - 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 20);
        }
    }
    public void SilkSellAll()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Silk") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + (PlayerPrefs.GetInt("Silk") * 20));
            PlayerPrefs.SetInt("Silk", 0);
        }
    }
    public void SilverwareSellOne()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Silverware") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Silverware", PlayerPrefs.GetInt("Silverware") - 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 30);
        }
    }
    public void SilverwareSellAll()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Silverware") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + (PlayerPrefs.GetInt("Silverware") * 30));
            PlayerPrefs.SetInt("Silverware", 0);
        }
    }
    public void EmeraldSellOne()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Emerald") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Emerald", PlayerPrefs.GetInt("Emerald") - 1);
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + 50);
        }
    }
    public void EmeraldSellAll()
    {
        Click.Play();
        if (PlayerPrefs.GetInt("Emerald") > 0)
        {
            SellStuff.Play();
            PlayerPrefs.SetInt("Gold", PlayerPrefs.GetInt("Gold") + (PlayerPrefs.GetInt("Emerald") * 50));
            PlayerPrefs.SetInt("Emerald", 0);
        }
    }
}

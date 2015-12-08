using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour 
{
    //Holds the prefab of the GameManager
    public GameManager gameManager;

	// Use this for initialization
	void Awake () 
    {  
        //If a GameManager doesn't exist, make one
        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        } //end if	
	} //end Awake
} //end Loader class

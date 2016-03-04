﻿/***************************************************************************************** 
 * File:    Loader.cs
 * Summary: Creates GameManager upon opening of program
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System.Collections;
#endregion

public class Loader : MonoBehaviour 
{
    #region Variables
    //Holds the prefab of the GameManager
    public GameManager gameManager;
    #endregion

    #region Methods
	// Use this for initialization
	void Awake () 
    {  
        //If a GameManager doesn't exist, make one
        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        } //end if	
	} //end Awake
    #endregion
} //end Loader class

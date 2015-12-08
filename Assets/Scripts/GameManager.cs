using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	//Singleton Variable
	public static GameManager instance = null;

	// Use this for initialization
	void Awake ()
	{
		//If an instance doesn't exist, set it to this
		if (instance == null) 
		{
			instance = this;
		} //end if
        //If an instance exists that isn't this, destroy this
        else if (instance != this) 
		{
			Destroy (gameObject);
		} //end else if	

		//Keep GameManager from destruction OnLoad
		DontDestroyOnLoad (instance);
	} //end Awake
	
	// Update is called once per frame
	void Update ()
	{
	
	} //end Update
} //end GameManager class

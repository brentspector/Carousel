using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
	//Singleton Variable
	public static GameManager instance = null;

	//Scene variables
	IntroMenu intro;		//Manages intro and menu scenes

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

		//Get IntroMenu component
		intro = GetComponent<IntroMenu> ();
	} //end Awake
	
	// Update is called once per frame
	void Update ()
	{
		//If intro scene
		if(Application.loadedLevelName == "Intro")
		{
			intro.Intro();
		} //end if
	
	} //end Update
} //end GameManager class

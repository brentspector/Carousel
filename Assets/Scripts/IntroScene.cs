/***************************************************************************************** 
 * File:    IntroScene.cs
 * Summary: Handles process for Intro scene
 *****************************************************************************************/
#region Using
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#endregion

public class IntroScene : MonoBehaviour
{
	#region Variables
	//Scene variables
	int checkpoint = 0;				//Manages function progress
	GameObject title;				//Title on Intro scene
	GameObject image;				//Image on Intro scene
	GameObject enter;				//Press enter to start

	//Animation variables
	bool  playing = false;			//Animation is playing
	float damping = 0.8f;			//How fast animation is
	float maxScale = 2f;			//Maximum scale of object in animation
	int   bounces = 3;				//Bounces of title
	#endregion

	#region Methods
	/***************************************
	 * Name: RunIntro
	 * Play the introduction scene
	 ***************************************/
	public void RunIntro()
	{
		//Get title screen objects
		if (checkpoint == 0)
		{			
			GameManager.instance.LogErrorMessage("Checkpoint 0 of Intro started.");
			//Add the checkpoint delegate
			GameManager.instance.checkDel = ChangeCheckpoint;
			title = GameObject.Find("Title");
			image = GameObject.Find("Image");
			enter = GameObject.Find("PressEnter");
			checkpoint = 1;
			GameManager.instance.LogErrorMessage("Checkpoint 0 of Intro finished.");
		} //end if

		//Black out starting image, shrink title, hide enter
		else if (checkpoint == 1)
		{
			GameManager.instance.LogErrorMessage("Checkpoint 1 of Intro started.");
			image.GetComponent<Image>().color = Color.black;
			title.transform.localScale = new Vector3(0.2f, 0.2f);
			enter.SetActive(false);
			checkpoint = 2;
			GameManager.instance.LogErrorMessage("Checkpoint 1 of Intro finished.");
		} //end else if

		//Play animation
		else if (checkpoint == 2)
		{
			//Gather input if playing animation
			if (playing)
			{
				GetInput();
				return;
			} //end if
			GameManager.instance.LogErrorMessage("Checkpoint 2 of Intro started.");
			//PLaying animation
			playing = true;

			//Play animation, processing stays true until end
			StartCoroutine(IntroAnimation());
			GameManager.instance.LogErrorMessage("Checkpoint 2 of Intro finished.");
		} //end else if

		//End animation and fade out when player hits enter/return
		else if(checkpoint == 3)
		{
			//Proceed to main menu when play hits enter or mouse click
			GetInput();
		} //end else if

		//Move to menu scene when finished fading out
		else if(checkpoint == 4)
		{
			GameManager.instance.LogErrorMessage("Checkpoint 4 of Intro started.");
			//Begin transition, and set checkpoint to bogus to avoid multiple calls
			GameManager.instance.LoadScene("StartMenu", true);
			checkpoint = 5;
			GameManager.instance.LogErrorMessage("Checkpoint 4 of Intro finished.");
		} //end else if
	} //end RunIntro()

	/***************************************
	 * Name: GetInput
	 * Gather user input and set variables
	 * as necessary
	 ***************************************/
	void GetInput()
	{
		/*********************************************
		 * Left Arrow
		 *********************************************/
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{

		} //end if Left Arrow

		/*********************************************
		* Right Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{

		} //end else if Right Arrow

		/*********************************************
		* Up Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.UpArrow))
		{

		} //end else if Up Arrow

		/*********************************************
		* Down Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{

		} //end else if Down Arrow

		/*********************************************
		* Mouse Moves Left
		**********************************************/
		else if (Input.GetAxis("Mouse X") < 0)
		{

		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		else if (Input.GetAxis("Mouse X") > 0)
		{

		} //end else if Mouse Moves Right

		/*********************************************
		* Mouse Moves Up
		**********************************************/
		else if (Input.GetAxis("Mouse Y") > 0)
		{

		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		else if (Input.GetAxis("Mouse Y") < 0)
		{

		} //end else if Mouse Moves Down

		/*********************************************
		* Mouse Wheel Up
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{

		} //end else if Mouse Wheel Up

		/*********************************************
		* Mouse Wheel Down
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{

		} //end else if Mouse Wheel Down

		/*********************************************
		* Left Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(0))
		{
			//If playing animation
			if (checkpoint == 2 && playing)
			{
				playing = false;
			} //end if
			else if (checkpoint == 3)
			{
				GameManager.instance.LogErrorMessage("Checkpoint 3 of Intro finished by Left Mouse.");
				checkpoint = 4;
			} //end else if
		} //end else if Left Mouse Button

		/*********************************************
		* Right Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(1))
		{

		} //end else if Right Mouse Button

		/*********************************************
		* Enter/Return Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.Return))
		{
			//If playing animation
			if (checkpoint == 2 && playing)
			{
				playing = false;
			} //end if
			else if (checkpoint == 3)
			{
				GameManager.instance.LogErrorMessage("Checkpoint 3 of Intro finished by Enter key");
				checkpoint = 4;
			} //end else if
		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{

		} //end else if X Key
	} //end GetInput

	/***************************************
     * Name: IntroAnimation
     * This is what the intro animation is
     ***************************************/
	IEnumerator IntroAnimation()
	{
		GameManager.instance.LogErrorMessage("Started intro animation.");
		//Keep internal count of bounces
		int numBounce = 0;

		//Internal timer for lerp
		float elapsedTime = 0f;

		//Colors
		Color bounceColor = Color.white / bounces;					//Divdes color up into equal parts
		Color targetColor = bounceColor;							//Initializes first target color
		Color lastColor = image.GetComponent<Image> ().color;		//Initializes starting color

		//Title
		Vector3 targetScale = new Vector3(maxScale, maxScale, 0);	//Initializes title to bounce to highest
		Vector3 lastScale = title.transform.localScale;				//Initializes last scale size

		//Loop doesn't break until break command is reached
		while(true)
		{
			//If player ends the animation early, break the loop
			if(!playing)
			{
				GameManager.instance.LogErrorMessage("Player broke animation loop.");
				break;
			} //end if

			//Reset bounce target if target met
			if(title.transform.localScale == targetScale)
			{
				//Go to goal if bounces have been met
				if(numBounce == bounces - 1)
				{
					targetScale = new Vector3(1f, 1f);
				} //end if
				//If final bounce is done, break
				else if(numBounce == bounces)
				{
					GameManager.instance.LogErrorMessage("Animation finished normally.");
					break;
				} //end else if
				//If even number of bounces
				else if(numBounce%2 == 0)
				{
					//Get a value between 0.2 and 1
					float value = 0.2f + ((0.8f/(float)bounces) * (numBounce + 1));
					targetScale = new Vector3(value, value, 1);
				} //end else if
				//If odd number
				else
				{
					//Get a value between 1 and maxScale
					float value = maxScale - (((maxScale-1)/bounces) * numBounce);
					targetScale = new Vector3(value, value, 1);
				} //end else

				//Reset color target
				targetColor = targetColor+bounceColor;

				//Reset last positions
				lastColor = image.GetComponent<Image>().color;
				lastScale = title.transform.localScale;

				//Increase bounce count
				numBounce++;

				//Reset elapsed time
				elapsedTime = 0f;
			} //end if

			//Lighten image over bounces
			image.GetComponent<Image>().color = Color.Lerp(lastColor, targetColor, elapsedTime * damping);

			//Bounce title
			title.transform.localScale = Vector3.Lerp(lastScale, targetScale, elapsedTime * damping);

			//Increase elapsedTime by dt
			elapsedTime += Time.deltaTime;

			yield return null;
		} //end while

		//Make sure image color is white
		image.GetComponent<Image>().color = Color.white;

		//Make sure title scale is 1
		title.transform.localScale = new Vector3 (1f, 1f);

		//Process is finished
		enter.SetActive (true);
		playing = false;
		checkpoint = 3;
		GameManager.instance.LogErrorMessage("Finished intro animation.");
	} //end IntroAnimation

	/***************************************
     * Name: FinishedAnimation
     * Returns if intro animation is finished
     ***************************************/
	public bool FinishedAnimation()
	{
		return playing;
	} //end FinishedAnimation

	/***************************************
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end class IntroScene
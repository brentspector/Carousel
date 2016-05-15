/***************************************************************************************** 
 * File:    MenuScene.cs
 * Summary: Handles process for Menu scene
 *****************************************************************************************/
#region Using
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#endregion

public class MenuScene : MonoBehaviour
{
	#region Variables
	//Scene Variables
	int checkpoint = 0;				//Manages function progress
	int choiceNumber;   			//What choice is highlighted 
	bool processing = false;		//Whether there is a function in progress
	List<RectTransform> transforms;	//List of RectTransforms of choices
	GameObject mChoices;			//All the menu choices
	GameObject mContinue;			//Menu's continue data
	GameObject selection;			//Scene tools selection
	Text pName;						//Player's name on continue panel
	Text badges;					//Amount of badges save file has
	Text totalTime;					//Total time on save file
	#endregion

	#region Methods
	/***************************************
	 * Name: RunMenu
	 * Play the menu scene
	 ***************************************/
	public void RunMenu()
	{
		//Get menu objects
		if (checkpoint == 0)
		{
			//Move to next checkpoint only when fade in is finished
			if (processing)
			{
				return;
			} //end if

			//Begin processing
			processing = true;

			//Reset scene tools canvas' camera
			GameManager.tools.GetComponent<Canvas> ().worldCamera = Camera.main;

			//Set reference for menu components
			mChoices = GameObject.Find("ChoiceGrid");
			mContinue = GameObject.Find("ContinueGrid");
			selection = GameManager.tools.transform.FindChild("Selection").gameObject;
			pName = mContinue.transform.GetChild(0).GetComponent<Text>();
			badges = mContinue.transform.GetChild(1).GetComponent<Text>();
			totalTime = mContinue.transform.GetChild(2).GetComponent<Text>();

			//Initialize choice number
			choiceNumber = 0;

			//Initialize rect transform list
			transforms = new List<RectTransform>();
			for (int i = 0; i < mChoices.transform.childCount; i++)
			{
				transforms.Add(mChoices.transform.GetChild(i).GetComponent<RectTransform>());
			} //end for

			//If a file isn't found, disable continue option, process alternate menu
			if (!GameManager.instance.GetPersist())
			{
				mContinue.SetActive(false);
				mChoices.transform.GetChild(0).gameObject.SetActive(false);
				choiceNumber = 1;
				checkpoint = 3;
			} //end if

			//Otherwise file is found, process normally
			else
			{
				checkpoint = 1;
			} //end else
		} //end if

		//Initialize menu data (with continue option)
		else if (checkpoint == 1)
		{
			//If animation is playing, return
			if (GameManager.instance.IsProcessing())
			{
				return;
			} //end if

			//Resize selection to width of top choice, and 1/3 of height
			selection.GetComponent<RectTransform>().sizeDelta = 
				new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y / 3);

			//Reposition selection to top choice
			selection.transform.position = new Vector3(
				mChoices.transform.GetChild(choiceNumber).transform.position.x,
				mChoices.transform.GetChild(choiceNumber).transform.position.y - 1,
				100);

			//Fill in continue area
			pName.text = GameManager.instance.GetTrainer().PlayerName;
			badges.text = "Badges: " + GameManager.instance.GetTrainer().PlayerBadgeCount.ToString();
			totalTime.text = "Playtime: " + GameManager.instance.GetTrainer().HoursPlayed.ToString() + ":" +
			GameManager.instance.GetTrainer().MinutesPlayed.ToString("00");

			//Run fade in animation
			GameManager.instance.checkDel = ChangeCheckpoint;
			GameManager.instance.FadeInAnimation(2);
		} //end else if

		//Run menu (with continue option)
		else if (checkpoint == 2)
		{
			//Enable selection 
			if (!selection.activeSelf)
			{
				selection.SetActive(true);
			} //end if

			//Get player input
			GetInput();
		} //end else if

		//Initialize menu data (without continue option)
		else if (checkpoint == 3)
		{
			//Resize selection to width of top choice, and 1/3 of height
			selection.GetComponent<RectTransform>().sizeDelta = 
				new Vector2(transforms[choiceNumber].sizeDelta.x, transforms[choiceNumber].sizeDelta.y / 3);

			//Reposition selection to top choice
			selection.transform.position = new Vector3(
				mChoices.transform.GetChild(choiceNumber).transform.position.x,
				mChoices.transform.GetChild(choiceNumber).transform.position.y - 2,
				100);

			//Run fade in animation
			GameManager.instance.checkDel = ChangeCheckpoint;
			GameManager.instance.FadeInAnimation(4);
		} //end else if

		//Run menu (without continue option)
		else if (checkpoint == 4)
		{
			//Enable selection 
			if (!selection.activeSelf)
			{
				selection.SetActive(true);
			} //end if

			//Get player input
			GetInput();
		} //end else if

		//Move to selected scene
		else if (checkpoint == 5)
		{
			//Disable selection
			selection.SetActive(false);

			//Switch based on choice number
			switch (choiceNumber)
			{
				//First choice - Continue Game
				case 0:
					GameManager.instance.LoadScene("MainGame", true);
					break;
				//Second choice - New Game
				case 1:
					GameManager.instance.LoadScene("NewGame", true);
					break;
				//Third choice - Options
				case 2:
					GameManager.instance.LoadScene("Intro", true);
					break;
			} //end switch
		} //end else if
	} //end RunMenu()

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
			//Menu with continue game
			if (checkpoint == 2)
			{
				//Decrease choice number (higher options are on lower children)
				choiceNumber--;

				//Loop to end if necessary
				if (choiceNumber < 0)
				{
					choiceNumber = mChoices.transform.childCount-1;
				} //end if

				//Reposition selection
				selection.transform.position = new Vector3(
					mChoices.transform.GetChild(choiceNumber).transform.position.x,
					mChoices.transform.GetChild(choiceNumber).transform.position.y - 1,
					100);
			} //end if

			//Menu without continue game
			else if (checkpoint == 4)
			{
				//Decrease choice number (higher options are on lower children)
				choiceNumber--;

				//Loop to end if necessary
				if (choiceNumber < 1)
				{
					choiceNumber = mChoices.transform.childCount-1;
				} //end if

				//Reposition selection
				selection.transform.position = new Vector3(
					mChoices.transform.GetChild(choiceNumber).transform.position.x,
					mChoices.transform.GetChild(choiceNumber).transform.position.y - 2,
					100);
			} //end else if
		} //end else if Up Arrow

		/*********************************************
		* Down Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			//Menu with continue game
			if (checkpoint == 2)
			{
				//Increase choice number (lower options are on higher children)
				choiceNumber++;

				//Loop to beginning if necessary
				if (choiceNumber > mChoices.transform.childCount-1)
				{
					choiceNumber = 0;
				} //end if

				//Reposition selection
				selection.transform.position = new Vector3(
					mChoices.transform.GetChild(choiceNumber).transform.position.x,
					mChoices.transform.GetChild(choiceNumber).transform.position.y - 1,
					100);
			} //end if

			//Menu without continue game
			else if (checkpoint == 4)
			{
				//Increase choice number (lower options are on higher children)
				choiceNumber++;

				//Loop to beginning if necessary
				if (choiceNumber > mChoices.transform.childCount-1)
				{
					choiceNumber = 1;
				} //end if

				//Reposition selection
				selection.transform.position = new Vector3(
					mChoices.transform.GetChild(choiceNumber).transform.position.x,
					mChoices.transform.GetChild(choiceNumber).transform.position.y - 2,
					100);
			} //end else if
		} //end else if Down Arrow

		/*********************************************
		* Mouse Moves Left
		**********************************************/
		else if (Input.GetAxis("Mouse X") > 0)
		{

		} //end else if Mouse Moves Left

		/*********************************************
		* Mouse Moves Right
		**********************************************/
		else if (Input.GetAxis("Mouse X") < 0)
		{

		} //end else if Mouse Moves Right

		/*********************************************
		* Mouse Moves Up
		**********************************************/
		else if (Input.GetAxis("Mouse Y") > 0)
		{
			//Menu with continue game
			if (checkpoint == 2 && Input.mousePosition.y > Camera.main.WorldToScreenPoint(selection.transform.position).y +
				selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//Decrease choice number (higher options are on lower children)
				choiceNumber--;

				//Lock at beginning
				if (choiceNumber < 0)
				{
					choiceNumber = 0;
				} //end if

				//Reposition selection
				selection.transform.position = new Vector3(
					mChoices.transform.GetChild(choiceNumber).transform.position.x,
					mChoices.transform.GetChild(choiceNumber).transform.position.y - 1,
					100);
			} //end if

			//Menu without continue game
			else if (checkpoint == 4 && Input.mousePosition.y > Camera.main.WorldToScreenPoint(selection.transform.position).y +
			        selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//Decrease choice number (higher options are on lower children)
				choiceNumber--;

				//Lock at beginning
				if (choiceNumber < 1)
				{
					choiceNumber = 1;
				} //end if

				//Reposition selection
				selection.transform.position = new Vector3(
					mChoices.transform.GetChild(choiceNumber).transform.position.x,
					mChoices.transform.GetChild(choiceNumber).transform.position.y - 2,
					100);
			} //end else if
		} //end else if Mouse Moves Up

		/*********************************************
		* Mouse Moves Down
		**********************************************/
		else if (Input.GetAxis("Mouse Y") < 0)
		{
			//Menu with continue game
			if (checkpoint == 2 && Input.mousePosition.y < Camera.main.WorldToScreenPoint(selection.transform.position).y -
				selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//Increase choice number (lower options are on higher children)
				choiceNumber++;

				//Lock at end
				if (choiceNumber > mChoices.transform.childCount-1)
				{
					choiceNumber = mChoices.transform.childCount-1;
				} //end if

				//Reposition selection
				selection.transform.position = new Vector3(
					mChoices.transform.GetChild(choiceNumber).transform.position.x,
					mChoices.transform.GetChild(choiceNumber).transform.position.y - 1,
					100);
			} //end if

			//Menu without continue game
			else if (checkpoint == 4 && Input.mousePosition.y < Camera.main.WorldToScreenPoint(selection.transform.position).y -
				selection.GetComponent<RectTransform>().rect.height / 2)
			{
				//Increase choice number (lower options are on higher children)
				choiceNumber++;

				//Lock at end
				if (choiceNumber > mChoices.transform.childCount-1)
				{
					choiceNumber = mChoices.transform.childCount-1;
				} //end if

				//Reposition selection
				selection.transform.position = new Vector3(
					mChoices.transform.GetChild(choiceNumber).transform.position.x,
					mChoices.transform.GetChild(choiceNumber).transform.position.y - 2,
					100);
			} //end else if
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
			//A choice was made, move to resolution checkpoint
			if (checkpoint == 2 || checkpoint == 4)
			{
				checkpoint = 5;
			} //end if
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
			//A choice was made, move to resolution checkpoint
			if (checkpoint == 2 || checkpoint == 4)
			{
				checkpoint = 5;
			} //end if
		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{

		} //end else if X Key
	} //end GetInput

	/***************************************
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end class MenuScene
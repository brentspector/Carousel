/***************************************************************************************** 
 * File:    NewGameScene.cs
 * Summary: Handles process for New Game scene
 *****************************************************************************************/
#region Using
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#endregion

public class NewGameScene : MonoBehaviour
{
	#region Variables
	//Scene variables
	int checkpoint = 0;				//Manage function progress
	int playerChoice;				//The trainer the player is currently selecting
	Image profBase;					//Base professor stands on
	Image professor;				//Professor image
	InputField inputText;			//The actual text of input
	GameObject prevTrainer;			//Object of previous trainer
	GameObject currTrainer;			//Object of currently selected trainer
	GameObject nextTrainer;			//Object of next trainer
	GameObject input;				//The input portion of scene tools
	string playerName;				//The player's name
	#endregion

	#region Methods
	/***************************************
	 * Name: RunNewGame
	 * Play the new game scene
	 ***************************************/
	public void RunNewGame()
	{
		//Set up scene
		if (checkpoint == 0)
		{
			//Return if animation is playing
			if (GameManager.instance.IsProcessing())
			{
				return;
			} //end if

			//Set checkpoint delegate
			GameManager.instance.checkDel = ChangeCheckpoint;

			//Set confirm delegate
			GameManager.instance.confirmDel = ApplyConfirm;

			//Get references
			profBase = GameObject.Find("Base").GetComponent<Image>();
			professor = GameObject.Find("Professor").GetComponent<Image>();
			input = GameManager.tools.transform.FindChild("Input").gameObject;
			inputText = input.transform.GetChild(1).GetComponent<InputField>();
			prevTrainer = GameObject.Find("PreviousTrainer");
			currTrainer = GameObject.Find("CurrentTrainer");
			nextTrainer = GameObject.Find("NextTrainer");

			//Initialze starting states
			playerChoice = 0;
			profBase.color = new Color(1, 1, 1, 0);
			professor.color = new Color(1, 1, 1, 0);
			prevTrainer.SetActive(false);
			currTrainer.SetActive(false);
			nextTrainer.SetActive(false);

			//Run fade in animation
			GameManager.instance.FadeInAnimation(1);
		} //end if

		//Fade professor in
		else if (checkpoint == 1)
		{
			//If animation is running
			if (GameManager.instance.IsProcessing())
			{
				return;
			} //end if
				
			//Run Fade In on Base and Professor objects
			GameManager.instance.FadeInObjects(new Image[] { profBase, professor }, 2);
		} //end else if

		//Begin scene
		else if (checkpoint == 2)
		{
			//Display text
			GameManager.instance.DisplayText("Welcome to Pokemon Carousel! I am the Ringmaster " +
			"Melotta.", false);

			//Move to next checkpoint
			checkpoint = 3;
		} //end else if

		//Continue scene
		else if (checkpoint == 3)
		{
			//Display text
			GameManager.instance.DisplayText("This circus has attracted major gym Leaders from " +
			"around the world! In fact, that's why you're here, isn't it?", false);

			//Move to next checkpoint
			checkpoint = 4;
		} //end else if

		//Request player name
		else if (checkpoint == 4)
		{
			//Display text
			GameManager.instance.DisplayText("Alright, let's get you set up. First, what is " +
			"your name?", true);

			//Move to next checkpoint
			checkpoint = 5;
		} //end else if

		//Get player name
		else if (checkpoint == 5)
		{
			//Display name input
			input.SetActive(true);
			input.transform.GetChild(2).GetComponent<Text>().text = "Please enter your name.";
			input.transform.GetChild(0).GetComponent<Text>().text = "Player name: ";
			inputText.text = "";
			inputText.ActivateInputField();
			checkpoint = 6;
		} //end else if

		//Wait for player to finish entering their name
		else if (checkpoint == 6)
		{
			//Get player input
			GetInput();
		} //end else if

		//Move to character image selection
		else if (checkpoint == 7)
		{
			//Display text
			GameManager.instance.DisplayText("Great! Which character are you?", true);

			//Move to next checkpoint
			checkpoint = 8;
		} //end else if

		//Turn on character choices
		else if (checkpoint == 8)
		{
			//Disable professor
			profBase.color = Color.clear;
			professor.color = Color.clear;

			//Activate player choices
			prevTrainer.SetActive(true);
			currTrainer.SetActive(true);
			nextTrainer.SetActive(true);

			//Move to next checkpoint
			checkpoint = 9;
		} //end else if

		//Wait for player to select a character
		else if (checkpoint == 9)
		{
			//Get player input
			GetInput();

			//Update trainer sprites
			//Previous
			if (playerChoice == 0)
			{
				prevTrainer.transform.FindChild("TrainerImage").GetComponent<Image>().sprite =
					DataContents.trainerCardSprites[DataContents.trainerCardSprites.Length - 1];
			} //end if
			else
			{
				prevTrainer.transform.FindChild("TrainerImage").GetComponent<Image>().sprite =
					DataContents.trainerCardSprites[playerChoice - 1];
			} //end else

			//Current
			currTrainer.transform.FindChild("TrainerImage").GetComponent<Image>().sprite =
				DataContents.trainerCardSprites[playerChoice];

			//Next
			if (playerChoice == DataContents.trainerCardSprites.Length - 1)
			{
				nextTrainer.transform.FindChild("TrainerImage").GetComponent<Image>().sprite =
					DataContents.trainerCardSprites[0];
			} //end if
			else
			{
				nextTrainer.transform.FindChild("TrainerImage").GetComponent<Image>().sprite =
					DataContents.trainerCardSprites[playerChoice + 1];
			} //end else
		} //end else if

		//Finish new game
		else if (checkpoint == 10)
		{
			//Enable professor
			profBase.color = Color.white;
			professor.color = Color.white;

			//Display text
			GameManager.instance.DisplayText("Fantastic! Here's your sample team. Enjoy the show.", true);

			//Move to next checkpoint
			checkpoint = 11;
		} //end else if

		//Return to introduction
		else if (checkpoint == 11)
		{
			//Save new file, then go back to intro
			GameManager.instance.RestartFile(GameManager.instance.GetPersist());
			GameManager.instance.GetTrainer().PlayerName = playerName;
			GameManager.instance.GetTrainer().PlayerImage = playerChoice;
			GameManager.instance.GetTrainer().RandomTeam();
			GameManager.instance.GetTrainer().PopulateStock(5);
			GameManager.instance.Persist(false);
			GameManager.instance.Reset();

			//Set checkpoint to dummy
			checkpoint = 12;
		} //end else if
	} //end RunNewGame

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
			//Cycle through trainer choices
			if (checkpoint == 9)
			{
				//Decrease player choice
				playerChoice--;

				//Loop to end of selection if necessary
				if (playerChoice < 0)
				{
					playerChoice = DataContents.trainerCardSprites.Length - 1;
				} //end if
			} //end if
		} //end if Left Arrow

		/*********************************************
		* Right Arrow
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			//Cycle through trainer choices
			if (checkpoint == 9)
			{
				//Increase player choice
				playerChoice++;

				//Loop to beginning of selection if necessary
				if (playerChoice >  DataContents.trainerCardSprites.Length - 1)
				{
					playerChoice = 0;
				} //end if
			} //end if
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
			//Cycle through trainer choices
			if (checkpoint == 9)
			{
				//Increase player choice
				playerChoice++;

				//Loop to beginning of selection if necessary
				if (playerChoice >  DataContents.trainerCardSprites.Length - 1)
				{
					playerChoice = 0;
				} //end if
			} //end if
		} //end else if Mouse Wheel Up

		/*********************************************
		* Mouse Wheel Down
		**********************************************/
		else if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			//Cycle through trainer choices
			if (checkpoint == 9)
			{
				//Decrease player choice
				playerChoice--;

				//Loop to end of selection if necessary
				if (playerChoice < 0)
				{
					playerChoice = DataContents.trainerCardSprites.Length - 1;
				} //end if
			} //end if
		} //end else if Mouse Wheel Down

		/*********************************************
		* Left Mouse Button
		**********************************************/
		else if (Input.GetMouseButtonUp(0))
		{
			//Player finished entering their name
			if (checkpoint == 6 && inputText.text.Length > 0)
			{
				//Conver input name to player's name
				playerName = inputText.text;
				input.SetActive(false);

				//Display text
				GameManager.instance.DisplayConfirm("So your name is " + playerName + "?", 0, false);
			} //end if

			//Player selected a character
			else if (checkpoint == 9)
			{
				//Turn off character selection
				prevTrainer.SetActive(false);
				currTrainer.SetActive(false);
				nextTrainer.SetActive(false);
				checkpoint = 10;
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
			//Player finished entering their name
			if (checkpoint == 6 && inputText.text.Length > 0)
			{
				//Conver input name to player's name
				playerName = inputText.text;
				input.SetActive(false);

				//Display text
				GameManager.instance.DisplayConfirm("So your name is " + playerName + "?", 0, false);
			} //end if

			//Player selected a character
			else if (checkpoint == 9)
			{
				//Turn off character selection
				prevTrainer.SetActive(false);
				currTrainer.SetActive(false);
				nextTrainer.SetActive(false);
				checkpoint = 10;
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
	 * Name: ApplyConfirm
	 * Appliees the confirm choice
	 ***************************************/
	public void ApplyConfirm(ConfirmChoice e)
	{
		//Yes selected
		if (e.Choice == 0)
		{
			checkpoint = 7;
		} //end if

		//No selected
		else if(e.Choice== 1)
		{
			GameManager.instance.DisplayText("Ok. Let's try that again. What is your name?", true);
			checkpoint = 5;
		} //end else			
	} //end ApplyConfirm(ConfirmChoice e)

	/***************************************
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end class NewGameScene

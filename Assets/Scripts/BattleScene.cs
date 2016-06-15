/***************************************************************************************** 
 * File:    BattleScene.cs
 * Summary: Handles Process for Battle
 *****************************************************************************************/ 
#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#endregion

public class BattleScene : MonoBehaviour 
{
	#region Variables
	int checkpoint = 0;				//Manage function progress
	int lastAttacker;				//Who was the last pokemon to attack
	int battleType;					//Singles, Doubles, Triples, or other type
	List<int> participants;			//The pokemon that participated in the fight
	List<List<int>> fieldEffects;	//Effects that are present on the field
	bool processing = false;		//Whether a function is already processing something
	#endregion

	#region Methods
	/***************************************
	 * Name: RunBattle
	 * Play the battle scene
	 ***************************************/
	public void RunBattle()
	{
		//Initialize scene variables
		if (checkpoint == 0)
		{
			//Set checkpoint delegate
			GameManager.instance.checkDel = ChangeCheckpoint;

			//Initialize references
			lastAttacker = -1;
			participants = new List<int>();
			fieldEffects = new List<List<int>>();

			//Initialize effects lists
			for (int i = 0; i < fieldEffects.Count; i++)
			{
				for (int j = 0; j < (int)FieldEffects.COUNT; j++)
				{
					fieldEffects[i].Add(0);
				} //end for
			} //end for

			//Move to next checkpoint
			checkpoint = 1;
		} //end if

		//Set up start of scene
		else if (checkpoint == 1)
		{
			//Return if processing
			if (processing)
			{
				return;
			} //end if

			//Begin processing
			processing = true;



			//Move to next checkpoint
			GameManager.instance.FadeInAnimation(2);
		} //end else if
		else if (checkpoint == 2)
		{	
			//Get player input
			GetInput();

			/* Start of Battle
			 * -Display trainers
			 * -AI sends out first pokemon in roster
			 * -Player sends out first pokemon in roster
			 * 
			 * On Entry
			 * -Resolve field
			 * -Resolve ability (intimidate,levitate,frisk)
			 * -Resolve item(air balloon)
			 * 
			 * Beginning of round
			 * -Player picks Fight/Bag/Pokemon/Run
			 * --Fight
			 * ----Display pokemon attacks
			 * ----Player picks an attack or cancels back to main
			 * --Bag
			 * ----Display inventory
			 * ----Player picks an item with a battle use
			 * ----Player picks pokemon to use it on
			 * --Pokemon
			 * ----Team roster is shown
			 * ----Player picks a pokemon
			 * ----If switch is chosen, queue it
			 * ----Player can view summary or cancel back to main
			 * --Run
			 * ----End the fight and return any variables to start
			 * ----Return to battle menu
			 * -AI picks Fight/Item/Switch
			 * 
			 * End of Choice
			 * -Queue events
			 * --Run
			 * --Switch (fastest goes first)
			 * ---Resolve OnEntry
			 * --Item (fastest goes first)
			 * --Attack
			 * ---Sort by Priority, then Speed
			 * --Queue with flag indentifying the pokemon who used it
			 * Middle of round
			 * -Resolve Queue
			 * --Attack
			 * ----Resolve ability, item, and field
			 * ----Resolve damage
			 * ----Resolve recoil
			 * ----Check if either pokemon is fainted
			 * -----Resolve ability (aftermath)
			 * -----Resolve player targeted secondary effects
			 * -----If opponent is still alive, process opponent secondary effects
			 * --Loop until queue is empty
			 * End of round
			 * -Resolve field & item
			 * -Replace missing pokemon
			 * -Loop to beginning of round
			 * 
			 * End of Battle
			 * -Give badge if needed
			 * -Give points
			 */

		} //end else if
	} //end RunBattle

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

		} //end else if Enter/Return Key

		/*********************************************
		* X Key
		**********************************************/
		else if (Input.GetKeyDown(KeyCode.X))
		{

		} //end else if X Key
	} //end GetInput

	/***************************************
	 * Name: CheckEffect
	 * Checks if an effect is in place on
	 * the field
	 ***************************************/
	public bool CheckEffect(int effect, int target)
	{
		return fieldEffects[target][effect] > 0;
	} //end CheckEffect(int effect, int target)

	/***************************************
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
		processing = false;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end class BattleScene

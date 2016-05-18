/***************************************************************************************** 
 * File:    PCScene.cs
 * Summary: Handles process for PC scene
 *****************************************************************************************/
#region Using
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#endregion

public class PCScene : MonoBehaviour
{
	#region Variables
	int checkpoint = 0;			//Manage function progress
	#endregion

	#region Methods
	/***************************************
	 * Name: RunPC
	 * Play the PC scene
	 ***************************************/
	public void RunPC()
	{

	} //end RunPC

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

	//    /***************************************
	//     * Name: FillInChoices
	//     * Sets the choices for the choice menu
	//     * depending on the scene
	//     ***************************************/
	//    void FillInChoices()
	//    {
	//        //If in PC in Pokemon Region
	//        else if (sceneState == OverallGame.PC && boxChoice > -1)
	//        {
	//            //Fill in choices box
	//            for (int i = choices.transform.childCount - 1; i < 6; i++)
	//            {
	//                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
	//                                       choices.transform.GetChild (0).position,
	//                                       Quaternion.identity) as GameObject;
	//                clone.transform.SetParent (choices.transform);
	//            } //end for
	//            choices.transform.GetChild (0).GetComponent<Text> ().text = "Move";
	//            choices.transform.GetChild (1).GetComponent<Text> ().text = "Summary";
	//            choices.transform.GetChild (2).GetComponent<Text> ().text = "Item";
	//            choices.transform.GetChild (3).GetComponent<Text> ().text = "Ribbons";
	//            choices.transform.GetChild (4).GetComponent<Text> ().text = "Markings";
	//            choices.transform.GetChild (5).GetComponent<Text> ().text = "Release";
	//            choices.transform.GetChild (6).GetComponent<Text> ().text = "Cancel";
	//            choices.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (1).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (2).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (3).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (4).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (5).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (6).GetComponent<Text> ().color = Color.black;
	//            if (choices.transform.childCount > 7)
	//            {
	//                for (int i = 7; i < choices.transform.childCount; i++)
	//                {
	//                    Destroy (choices.transform.GetChild (i).gameObject);
	//                } //end for
	//            } //end if
	//        } //end else if
	//        //If in PC on Box Title
	//        else if (sceneState == OverallGame.PC && boxChoice == -2)
	//        {
	//            //Fill in choices box
	//            for (int i = choices.transform.childCount-1; i < 3; i++)
	//            {
	//                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
	//                    choices.transform.GetChild (0).position,
	//                    Quaternion.identity) as GameObject;
	//                clone.transform.SetParent (choices.transform);
	//            } //end for
	//            choices.transform.GetChild (0).GetComponent<Text> ().text = "Jump To";
	//            choices.transform.GetChild (1).GetComponent<Text> ().text = "Rename";
	//            choices.transform.GetChild (2).GetComponent<Text> ().text = "Wallpaper";
	//            choices.transform.GetChild (3).GetComponent<Text> ().text = "Cancel";
	//            choices.transform.GetChild (0).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (1).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (2).GetComponent<Text> ().color = Color.black;
	//            choices.transform.GetChild (3).GetComponent<Text> ().color = Color.black;
	//            if (choices.transform.childCount > 4)
	//            {
	//                for (int i = 4; i < choices.transform.childCount; i++)
	//                {
	//                    Destroy (choices.transform.GetChild (i).gameObject);
	//                } //end for
	//            } //end if
	//        }  //end else if
	//        //If in PC Markings
	//        else if (sceneState == OverallGame.PC && pcState == PCGame.POKEMONMARKINGS)
	//        {
	//            for (int i = choices.transform.childCount-1; i < DataContents.markingCharacters.Length+2; i++)
	//            {
	//                GameObject clone = Instantiate (choices.transform.GetChild (0).gameObject,
	//                    choices.transform.GetChild (0).position, Quaternion.identity) as GameObject;
	//                clone.transform.SetParent (choices.transform);
	//            } //end for
	//            for(int i = 0; i < DataContents.markingCharacters.Length+2; i++)
	//            {
	//                if(i == DataContents.markingCharacters.Length)
	//                {
	//                    choices.transform.GetChild(i).GetComponent<Text>().text = "OK";
	//                } //end if
	//                else if(i == DataContents.markingCharacters.Length+1)
	//                {
	//                    choices.transform.GetChild(i).GetComponent<Text>().text = "Cancel";
	//                } //end else if
	//                else
	//                {                           
	//                    choices.transform.GetChild(i).GetComponent<Text>().text =
	//                        DataContents.markingCharacters[i].ToString(); 
	//                } //end else
	//            } //end for
	//
	//            //Destroy extra
	//            if (choices.transform.childCount > DataContents.markingCharacters.Length+1)
	//            {
	//                for (int i = DataContents.markingCharacters.Length+2; i < choices.transform.childCount; i++)
	//                {
	//                    Destroy (choices.transform.GetChild (i).gameObject);
	//                } //end for
	//            } //end if
	//
	//            //Color in choices
	//            for(int i = 0; i < markingChoices.Count; i++)
	//            {
	//                choices.transform.GetChild(i).GetComponent<Text>().color =
	//                    markingChoices[i] ? Color.black : Color.gray;
	//            } //end for           
	//        } //end else if
	//    } //end FillInChoices

	/***************************************
	 * Name: ChangeCheckpoint
	 * Changes the checkpoint
	 ***************************************/
	public void ChangeCheckpoint(int newCheckpoint)
	{
		checkpoint = newCheckpoint;
	} //end ChangeCheckpoint(int newCheckpoint)
	#endregion
} //end class PCScene


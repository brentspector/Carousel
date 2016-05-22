///***************************************************************************************** 
// * File:    GatherInput.cs
// * Summary: Handles input for SceneManager
// *****************************************************************************************/
//#region Using
//using UnityEngine;
//using System.Collections;
//#endregion
//
//public class GatherInput : MonoBehaviour
//{
//	
//	/***************************************
//     * Name: GetInput
//     * Gather user input and set variables 
//     * as necessary
//     ***************************************/
//	public void GetInput()
//	{
//		/***********************************************
//         * Left Arrow
//         ***********************************************/ 
//		if(Input.GetKeyDown(KeyCode.LeftArrow))
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//
//					//Pokedex
//				case OverallGame.POKEDEX:
//					{
//						//Regular processing
//						if(checkpoint == 2)
//						{
//							pokedexIndex = ExtensionMethods.BindToInt(pokedexIndex-10, 1);
//						} //end if
//						break;
//					} //end case OverallGame POKEDEX
//			} //end scene switch
//		} //end if Left Arrow
//
//		/***********************************************
//         * Right Arrow
//         ***********************************************/ 
//		else if(Input.GetKeyDown(KeyCode.RightArrow))
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//
//					//Pokedex
//				case OverallGame.POKEDEX:
//					{
//						//Regular processing
//						if(checkpoint == 2)
//						{
//							if(pokedexIndex < 712)
//							{
//								pokedexIndex = ExtensionMethods.CapAtInt(pokedexIndex+10, 712);
//							} //end if
//							else
//							{
//								pokedexIndex = 721;
//							} //end else
//						} //end if
//						break;
//					} //end case OverallGame POKEDEX
//			} //end scene switch
//		} //end else if Right Arrow
//
//		/***********************************************
//         * Up Arrow
//         ***********************************************/
//		else if(Input.GetKeyDown(KeyCode.UpArrow))
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//
//					//Pokedex
//				case OverallGame.POKEDEX:
//					{
//						//Regular processing
//						if(checkpoint == 2)
//						{
//							pokedexIndex = ExtensionMethods.BindToInt(pokedexIndex-1, 1);
//						} //end if
//						break;
//					} //end case OverallGame POKEDEX
//			} //end scene switch
//		}//end else if Up Arrow
//
//		/***********************************************
//         * Down Arrow
//         ***********************************************/ 
//		else if(Input.GetKeyDown(KeyCode.DownArrow))
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//
//					//Pokedex
//				case OverallGame.POKEDEX:
//					{
//						//Regular processing
//						if(checkpoint == 2)
//						{
//							pokedexIndex = ExtensionMethods.CapAtInt(pokedexIndex+1, 721);
//						} //end if
//						break;
//					} //end case OverallGame POKEDEX
//			} //end scene switch
//		}//end else if Down Arrow
//
//		/***********************************************
//         * Mouse Moves Down
//         ***********************************************/ 
//		if(Input.GetAxis("Mouse Y") < 0)
//		{
//		} //end else if Mouse Moves Down
//
//		/***********************************************
//         * Mouse Moves Up
//         ***********************************************/ 
//		else if(Input.GetAxis("Mouse Y") > 0)
//		{
//		} //end else if Mouse Moves Up
//
//		/***********************************************
//         * Mouse Moves Left
//         ***********************************************/ 
//		if(Input.GetAxis("Mouse X") < 0)
//		{

//		} //end if Mouse Moves Left
//
//		/***********************************************
//         * Mouse Moves Right
//         ***********************************************/ 
//		else if(Input.GetAxis("Mouse X") > 0)
//		{
//		} //end else if Mouse Moves Right
//
//		/***********************************************
//         * Left Mouse Button
//         ***********************************************/ 
//		else if(Input.GetMouseButtonUp(0))
//		{
//		} //end else if Left Mouse Button
//
//		/***********************************************
//         * Right Mouse Button
//         ***********************************************/ 
//		else if(Input.GetMouseButtonDown(1))
//		{            
//			//Scene switch
//			switch(sceneState)
//			{
//
//					//Pokedex
//				case OverallGame.POKEDEX:
//					{
//						//Regular processing
//						if(checkpoint == 2)
//						{
//							StartCoroutine(LoadScene("MainGame", OverallGame.CONTINUE, true));
//						} //end if
//						break;
//					} //end case OverallGame POKEDEX
//			} //end scene switch
//		} //end else if Right Mouse Button
//
//		/***********************************************
//         * Mouse Wheel Up
//         ***********************************************/ 
//		else if(Input.GetAxis("Mouse ScrollWheel") > 0)
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//					//Pokedex
//				case OverallGame.POKEDEX:
//					{
//						//Regular processing
//						if(checkpoint == 2)
//						{
//							pokedexIndex = ExtensionMethods.BindToInt(pokedexIndex-1, 1);
//						} //end if
//						break;
//					} //end case OverallGame POKEDEX
//			} //end scene switch
//		} //end else if Mouse Wheel Up
//
//		/***********************************************
//         * Mouse Wheel Down
//         ***********************************************/ 
//		else if(Input.GetAxis("Mouse ScrollWheel") < 0)
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//
//					//Pokedex
//				case OverallGame.POKEDEX:
//					{
//						//Regular processing
//						if(checkpoint == 2)
//						{
//							pokedexIndex = ExtensionMethods.CapAtInt(pokedexIndex+1, 721);
//						} //end if
//						break;
//					} //end case OverallGame POKEDEX
//			} //end scene switch
//		} //end else if Mouse Wheel Down
//
//		/***********************************************
//         * Enter/Return Key
//         ***********************************************/ 
//		if(Input.GetKeyDown(KeyCode.Return))
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//
//					//Pokedex
//				case OverallGame.POKEDEX:
//					{
//						//Regular processing
//						if(checkpoint == 2)
//						{
//							ToggleShown();
//						} //end if
//						break;
//					} //end OverallGame POKEDEX
//			} //end scene switch
//		} //end else if Enter/Return Key
//
//		/***********************************************
//         * X Key
//         ***********************************************/ 
//		if(Input.GetKeyDown(KeyCode.X))
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//
//					//Pokedex
//				case OverallGame.POKEDEX:
//					{
//						//Regular processing
//						if(checkpoint == 2)
//						{
//							StartCoroutine(LoadScene("MainGame", OverallGame.CONTINUE, true));
//						} //end if
//						break;
//					} //end case OverallGame POKEDEX
//			} //end scene switch
//		} //end else if X Key
//	} //end GatherInput
//	#endregion
//} //end class GatherInput
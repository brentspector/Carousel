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
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						if (checkpoint == 11)
//						{
//							//Cycle each trainer
//							playerChoice--;
//							if (playerChoice < 0)
//							{
//								playerChoice = DataContents.trainerCardSprites.Length-1;
//							} //end if
//						} //end if
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon Summary on Continue Game -> Summary
//						if(gameState == MainGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//Deactivate current page
//								summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);
//
//								//Decrease choice
//								summaryChoice--;
//
//								//Loop to last child if on first child
//								if(summaryChoice < 0)
//								{
//									summaryChoice = 4;
//								} //end if
//							} //end if
//						} //end if Pokemon Summary on Continue Game -> Summary
//
//						//Continue Game -> My Team
//						else if(gameState == MainGame.TEAM)
//						{
//							//Decrease (higher slots are lower childs)
//							choiceNumber--;
//
//							//Clamp between -1 and team size
//							if(choiceNumber < -1)
//							{
//								choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//							} //end if
//
//							//Set currentSlotChoice
//							if(choiceNumber > 0)
//							{
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end if
//							else if(choiceNumber == 0)
//							{
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
//							} //end else if
//							else if(choiceNumber == -1)
//							{
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
//							} //end else if
//						} //end else if Continue Game -> My Team
//
//						//Pokemon Switch on Continue Game -> Switch
//						else if(gameState == MainGame.POKEMONSWITCH)
//						{
//							//Decrease (higher slots are lower childs)
//							switchChoice--;
//
//							//Clamp between 1 and team size
//							if(switchChoice < 1)
//							{
//								switchChoice = GameManager.instance.GetTrainer().Team.Count;
//							} //end if
//
//							//Set currentSwitchSlot
//							currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
//						} //end else if Pokemon Switch on Continue Game -> Switch
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState == MainGame.POKEMONRIBBONS)
//						{
//							//Decrease (higher slots are lower childs)
//							ribbonChoice--;
//
//							//Clamp at 0
//							if(ribbonChoice < 0)
//							{
//								ribbonChoice = 0;
//								previousRibbonChoice = -1;
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//
//							//Read ribbon
//							ReadRibbon();
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//						break;
//					} //end case OverallState CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						//PC Home
//						if(pcState == PCGame.HOME)
//						{
//							//If on box title
//							if(boxChoice == -2)
//							{
//								boxChoice = -3;
//							} //end if
//							//If on top left slot
//							else if(boxChoice == 0)
//							{
//								boxChoice = -2;
//							} //end else if
//							//Otherwise move left
//							else
//							{
//								//Decrease (higher slots on lower children)
//								boxChoice--;
//							} //end else
//						} //end if Home
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY)
//						{
//							//Decrease (higher slots are lower childs)
//							choiceNumber--;
//
//							//Clamp at 0
//							if(choiceNumber < 0)
//							{
//								//If there is a held pokemon
//								if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
//								{
//									choiceNumber = GameManager.instance.GetTrainer().Team.Count + 1;
//								} //end if
//								else
//								{
//									choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//								} //end else
//							} //end if
//
//							//Set currentSlotChoice
//							if(choiceNumber > 0)
//							{
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end if
//							else if(choiceNumber == 0)
//							{
//								currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
//							} //end else if
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Summary on PC -> Summary
//						else if(pcState == PCGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//Deactivate current page
//								summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);
//
//								//Decrease choice
//								summaryChoice--;
//
//								//Loop to last child if on first child
//								if(summaryChoice < 0)
//								{
//									summaryChoice = 4;
//								} //end if
//							} //end if
//						} //end else if Pokemon Summary on PC -> Summary
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS)
//						{
//							//Decrease (higher slots are lower childs)
//							ribbonChoice--;
//
//							//Clamp at 0
//							if(ribbonChoice < 0)
//							{
//								ribbonChoice = 0;
//								previousRibbonChoice = -1;
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//
//							//Read ribbon
//							ReadRibbon();
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//						break;
//					} //end case OverallState PC
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
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						if (checkpoint == 11)
//						{
//							//Cycle each trainer
//							playerChoice++;
//							if (playerChoice > DataContents.trainerCardSprites.Length-1)
//							{
//								playerChoice = 0;
//							} //end if
//						} //end if
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon Summary on Continue Game -> Summary
//						if(gameState == MainGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//Deactivate current page
//								summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);
//
//								//Increase choice
//								summaryChoice++;
//
//								//Loop to last child if on first child
//								if(summaryChoice > 4)
//								{
//									summaryChoice = 0;
//								} //end if
//							} //end if
//						} //end if Pokemon Summary on Continue Game -> Summary
//
//						//Continue Game -> My Team
//						else if(gameState == MainGame.TEAM)
//						{
//							//Increase (lower slots are higher children)
//							choiceNumber++;
//
//							//Clamp between -1 and team size
//							if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
//							{
//								choiceNumber = -1;
//							} //end if
//
//							//Set currentSlotChoice
//							if(choiceNumber > 0)
//							{
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end if
//							else if(choiceNumber == 0)
//							{
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
//							} //end else if
//							else if(choiceNumber == -1)
//							{
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
//							} //end else if
//
//						} //end else if Pokemon submenu on Continue Game -> My Team
//
//						//Pokemon Switch on Continue Game -> Switch
//						else if(gameState == MainGame.POKEMONSWITCH)
//						{
//							//Increase (lower slots are higher childs)
//							switchChoice++;
//
//							//Clamp between 1 and team size
//							if(switchChoice > GameManager.instance.GetTrainer().Team.Count)
//							{
//								switchChoice = 1;
//							} //end if
//
//							//Set currentSwitchSlot
//							currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
//						} //end else if Pokemon Switch on Continue Game -> Switch
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState == MainGame.POKEMONRIBBONS)
//						{
//							//Increase (lower slots are higher childs)
//							ribbonChoice++;
//
//							//Clamp at ribbon length
//							if(ribbonChoice >= GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount())
//							{
//								ribbonChoice = ExtensionMethods.BindToInt(
//									GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount()-1, 0);
//								previousRibbonChoice = -1;
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//
//							//Read ribbon
//							ReadRibbon();
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//						break;
//					} // end case OverallGame CONTINUEGAME
//
//					//PC
//				case OverallGame.PC:
//					{
//						//PC Home
//						if(pcState == PCGame.HOME)
//						{
//							//Increase (lower slots on higher children)
//							boxChoice++;
//
//							//Clamp at 31
//							if(boxChoice > 31)
//							{
//								boxChoice = 31;
//							} //end if
//						} //end if Home
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY)
//						{
//							//Increase (lower slots are higher children)
//							choiceNumber++;
//
//							//If there is a held pokemon
//							if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
//							{
//								//Clamp at one over team size
//								if(choiceNumber > GameManager.instance.GetTrainer().Team.Count + 1)
//								{
//									choiceNumber = 0;
//								} //end if
//							} //end if
//							else
//							{
//								//Clamp at team size
//								if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
//								{
//									choiceNumber = 0;
//								} //end if
//							} //end else
//
//							//Set currentSlotChoice
//							if(choiceNumber > 0)
//							{
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end if
//							else if(choiceNumber == 0)
//							{
//								currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
//							} //end else if
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Summary on PC -> Summary
//						else if(pcState == PCGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//Deactivate current page
//								summaryScreen.transform.GetChild(summaryChoice).gameObject.SetActive(false);
//
//								//Increase choice
//								summaryChoice++;
//
//								//Loop to last child if on first child
//								if(summaryChoice > 4)
//								{
//									summaryChoice = 0;
//								} //end if
//							} //end if
//						} //end else if Pokemon Summary on PC -> Summary
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS)
//						{
//							//Increase (lower slots are higher childs)
//							ribbonChoice++;
//
//							//Clamp at ribbon length
//							if(ribbonChoice >= selectedPokemon.GetRibbonCount())
//							{
//								ribbonChoice = ExtensionMethods.BindToInt(selectedPokemon.GetRibbonCount()-1, 0);
//								previousRibbonChoice = -1;
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//
//							//Read ribbon
//							ReadRibbon();
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//						break;
//					} //end case OverallState PC
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
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						//If up arrow is pressed on confirm box
//						if(checkpoint == 8)
//						{
//							//Decrease choice to show previous option is highlighted
//							choiceNumber--;
//							//Loop to last choice if it's lower than zero
//							if(choiceNumber < 0)
//							{
//								choiceNumber = 1;
//							} //end if
//
//							//Resize to choice width
//							selection.transform.position = new Vector3(
//								confirm.transform.GetChild(choiceNumber).position.x,
//								confirm.transform.GetChild(choiceNumber).position.y, 100);
//						} //end if					
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon submenu on Continue Game -> My Team
//						if(gameState == MainGame.POKEMONSUBMENU)
//						{
//							//Decrease choice (higher slots on lower children)
//							subMenuChoice--;
//
//							//If on the first option, loop to end
//							if(subMenuChoice < 0)
//							{
//								subMenuChoice = choices.transform.childCount-1;
//							} //end if
//
//							//Reposition selection
//							selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
//						} //end if Pokemon submenu on Continue Game -> My Team
//
//						//Continue Game -> My Team
//						else if(gameState == MainGame.TEAM)
//						{
//							//Move from top slot to Cancel button
//							if(choiceNumber == 1 || choiceNumber == 2)
//							{
//								choiceNumber = 0;
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
//							} //end if
//							//Move from PC button to last team slot
//							else if(choiceNumber == -1)
//							{
//								choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else if
//							//Move from Cancel button to PC button
//							else if(choiceNumber == 0)
//							{
//								choiceNumber = -1;
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
//							} //end else if
//							//Go up vertically
//							else
//							{
//								choiceNumber -= 2;
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else
//						} //end else if Continue Game -> My Team
//
//						//Pokemon Summary on Continue Game -> Summary
//						else if(gameState == MainGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//Decrease (higher slots are lower childs)
//								choiceNumber--;
//
//								//Clamp between 1 and team size
//								if(choiceNumber < 1)
//								{
//									choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//								} //end if
//							} //end if
//							else
//							{
//								//Decrease (higher slots are lower childs)
//								moveChoice--;
//
//								//Clamp between 0 and highest non-null move
//								if(moveChoice < 0)
//								{
//									moveChoice = GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount()-1;
//								} //end if
//
//								//Set move slot
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move"+(moveChoice+1)).gameObject;
//							} //end else
//						} //end else if Pokemon Summary on Continue Game -> Summary
//
//						//Pokemon Switch on Continue Game -> Switch
//						else if(gameState == MainGame.POKEMONSWITCH)
//						{
//							//Move from top slot to last slot
//							if(switchChoice <= 2)
//							{
//								switchChoice = GameManager.instance.GetTrainer().Team.Count;
//							} //end else if
//							//Go up vertically
//							else
//							{
//								switchChoice -= 2;
//							} //end else
//
//							//Set currentSwitchSlot
//							currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
//						} //end else if Pokemon Switch on Continue Game -> Switch
//
//						//Move Switch on Continue Game -> Summary -> Move Details
//						else if(gameState  == MainGame.MOVESWITCH)
//						{
//							//Decrease (higher slots on lower children)
//							switchChoice--;
//
//							//Clamp between 0 and highest non-null move
//							if(switchChoice < 0)
//							{
//								switchChoice = GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount()-1;
//							} //end if
//
//							//Set currentSwitchSlot
//							currentSwitchSlot = summaryScreen.transform.GetChild(5).
//								FindChild("Move"+(switchChoice+1)).gameObject;
//						} //end else if Move Switch on Continue Game -> Summary -> Move Details
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState == MainGame.POKEMONRIBBONS)
//						{
//							//Decrease (higher slots are lower childs)
//							choiceNumber--;
//
//							//Clamp between 1 and team size
//							if(choiceNumber < 1)
//							{
//								choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//							} //end if
//
//							//Reload ribbons
//							initialize = false;
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//						break;
//					} //end case OverallGame CONTINUEGAME
//
//					//PC
//				case OverallGame.PC:
//					{
//						//PC Home
//						if(pcState == PCGame.HOME)
//						{
//							//If on title, move to party button
//							if(boxChoice == -2)
//							{
//								boxChoice = 30;
//							} //end if
//							else
//							{
//								//Decrease (higher slots on lower children)
//								boxChoice -= 6;
//
//								//Clamp at -2 if not on a pokemon
//								if(boxChoice < 0)
//								{
//									boxChoice = -2;
//								} //end if
//							} //end else
//						} //end if
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY)
//						{
//							//If on first or second slot, go to Close button
//							if(choiceNumber == 1 || choiceNumber == 2)
//							{
//								choiceNumber = 0;
//								currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
//							} //end if
//							//If on Close button, go to last slot
//							else if(choiceNumber == 0)
//							{
//								//If there is a held pokemon
//								if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
//								{
//									choiceNumber =  GameManager.instance.GetTrainer().Team.Count + 1;
//								} //end if
//								else
//								{
//									choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//								} //end else
//
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon" + choiceNumber).gameObject;
//							} //end else if
//							//Go up vertically
//							else
//							{
//								choiceNumber -= 2;
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Submenu on PC
//						else if(pcState == PCGame.POKEMONSUBMENU)
//						{
//							//Decrease choice (higher slots on lower children)
//							subMenuChoice--;
//
//							//If on the first option, loop to end
//							if(subMenuChoice < 0)
//							{
//								subMenuChoice = choices.transform.childCount-1;
//							} //end if
//
//							//Reposition selection
//							selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
//						} //end else if Pokemon Submenu on PC
//
//						//Pokemon Summary on PC -> Summary
//						else if(pcState == PCGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//If party is open
//								if(partyTab.activeSelf && heldPokemon == null)
//								{
//									//Decrease (higher slots are lower childs)
//									choiceNumber--;
//
//									//Clamp between 1 and team size
//									if(choiceNumber < 1)
//									{
//										choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//									} //end if
//								} //end if
//								else if(!partyTab.activeSelf && heldPokemon == null)
//								{
//									//Decrease to previous pokemon index
//									boxChoice = GameManager.instance.GetTrainer().GetPreviousPokemon(boxChoice);
//								} //end else
//							} //end if
//							else
//							{
//								//Decrease (higher slots are lower childs)
//								moveChoice--;
//
//								//Clamp between 0 and highest non-null move
//								if(moveChoice < 0)
//								{
//									moveChoice = selectedPokemon.GetMoveCount()-1;
//								} //end if
//
//								//Set move slot
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move"+(moveChoice+1)).gameObject;
//							} //end else
//						} //end else if Pokemon Summary on PC -> Summary
//
//						//Move Switch on PC -> Summary -> Move Details
//						else if(pcState == PCGame.MOVESWITCH)
//						{
//							//Decrease (higher slots on lower children)
//							switchChoice--;
//
//							//Clamp between 0 and highest non-null move
//							if(switchChoice < 0)
//							{
//								switchChoice = selectedPokemon.GetMoveCount()-1;
//							} //end if
//
//							//Set currentSwitchSlot
//							currentSwitchSlot = summaryScreen.transform.GetChild(5).
//								FindChild("Move"+(switchChoice+1)).gameObject;
//						} //end else if Move Switch on PC -> Summary -> Move Details
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS)
//						{
//							//If party is open
//							if(partyTab.activeSelf && heldPokemon == null)
//							{
//								//Decrease (higher slots are lower childs)
//								choiceNumber--;
//
//								//Clamp between 1 and team size
//								if(choiceNumber < 1)
//								{
//									choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//								} //end if
//
//								//Update selected pokemon
//								selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];
//							} //end if
//							//If in pokemon region
//							else if(!partyTab.activeSelf && heldPokemon == null)
//							{
//								//Decrease to previous pokemon index
//								boxChoice = GameManager.instance.GetTrainer().GetPreviousPokemon(boxChoice);
//
//								//Update selected pokemon
//								selectedPokemon = GameManager.instance.GetTrainer().GetPC(
//									GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
//							} //end else if
//
//							//Reload ribbons
//							initialize = false;
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//
//						//Pokemon Markings on PC -> Submenu
//						else if(pcState == PCGame.POKEMONMARKINGS)
//						{
//							//Decrease choice (higher slots on lower children)
//							subMenuChoice--;
//
//							//If on the first option, loop to end
//							if(subMenuChoice < 0)
//							{
//								subMenuChoice = choices.transform.childCount-1;
//							} //end if
//						} //end else if Pokemon Markings on PC -> Submenu
//						break;
//					} //end case OverallState PC
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
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						//If down arrow is pressed on confirm box
//						if(checkpoint == 8)
//						{
//							//Increase choice to show next option is highlighted
//							choiceNumber++;
//							//Loop back to start if it's higher than the amount of choices
//							if(choiceNumber > 1)
//							{
//								choiceNumber = 0;
//							} //end if
//
//							//Reposition to choice location
//							selection.transform.position = new Vector3(
//								confirm.transform.GetChild(choiceNumber).position.x,
//								confirm.transform.GetChild(choiceNumber).position.y, 100);
//						} //end if
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon submenu on Continue Game -> My Team
//						if(gameState == MainGame.POKEMONSUBMENU)
//						{
//							//Increase choice (lower slots on higher children)
//							subMenuChoice++;
//
//							//If on the last option, loop to first
//							if(subMenuChoice > choices.transform.childCount-1)
//							{
//								subMenuChoice = 0;
//							} //end if
//
//							//Reposition selection
//							selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
//						} //end if Pokemon submenu on Continue Game -> My Team
//
//						//Continue Game -> My Team
//						else if(gameState == MainGame.TEAM)
//						{
//							//If on last, or second to last team slot, go to PC button
//							if((choiceNumber == GameManager.instance.GetTrainer().Team.Count - 1
//								&& choiceNumber > 0)
//								|| choiceNumber == GameManager.instance.GetTrainer().Team.Count)
//							{
//								choiceNumber = -1;
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
//							} //end if
//							//If on Cancel button, go to first slot
//							else if(choiceNumber == 0)
//							{
//								choiceNumber = 1;
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon1").gameObject;
//							} //end else if
//							//If on PC button, go to Cancel button
//							else if(choiceNumber == -1)
//							{
//								choiceNumber = 0;
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
//							} //end else if
//							//Go down vertically
//							else
//							{
//								choiceNumber += 2;
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else
//						} //end else if Continue Game -> My Team
//
//						//Pokemon Summary on Continue Game -> Summary
//						else if(gameState == MainGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//Increase (lower slots are on higher childs)
//								choiceNumber++;
//
//								//Clamp between 1 and team size
//								if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
//								{
//									choiceNumber = 1;
//								} //end if
//							} //end if
//							else
//							{
//								//Increase (lower slots are on higher childs)
//								moveChoice++;
//
//								//If chosen move is null, loop to top
//								if(moveChoice >= 4 || GameManager.instance.GetTrainer().Team[choiceNumber-1].
//									GetMove(moveChoice) == -1)
//								{
//									moveChoice = 0;
//								} //end if
//
//								//Set move slot
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move"+(moveChoice+1)).gameObject;
//							} //end else
//						} //end else if Pokemon Summary on Continue Game -> Summary
//
//						//Pokemon Switch on Continue Game -> Switch
//						else if(gameState == MainGame.POKEMONSWITCH)
//						{
//							//Move from bottom slot to top slot
//							if(switchChoice >= GameManager.instance.GetTrainer().Team.Count - 1)
//							{
//								switchChoice = 1;
//							} //end else if
//							//Go down vertically
//							else
//							{
//								switchChoice += 2;
//							} //end else
//
//							//Set currentSwitchSlot
//							currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
//						} //end else if Pokemon Switch on Continue Game -> Switch
//
//						//Move Switch on Continue Game -> Summary -> Move Details
//						else if(gameState  == MainGame.MOVESWITCH)
//						{
//							//Increase (lower slots on higher children)
//							switchChoice++;
//
//							//Clamp between 0 and highest non-null move
//							if(switchChoice > GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount()-1)
//							{
//								switchChoice = 0;
//							} //end if
//
//							//Set currentSwitchSlot
//							currentSwitchSlot = summaryScreen.transform.GetChild(5).
//								FindChild("Move"+(switchChoice+1)).gameObject;
//						} //end else if Move Switch on Continue Game -> Summary -> Move Details
//
//						//Pokemon Ribbons in Continue Game -> Ribbons
//						else if(gameState == MainGame.POKEMONRIBBONS)
//						{
//							//Increase (lower slots are on higher childs)
//							choiceNumber++;
//
//							//Clamp between 1 and team size
//							if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
//							{
//								choiceNumber = 1;
//							} //end if
//
//							//Reload ribbons
//							initialize = false;
//						} //end else if Pokemon Ribbons in Continue Game -> Ribbons
//						break;
//					} //end case OverallGame CONTINUEGAME
//
//					//PC
//				case OverallGame.PC:
//					{
//						//PC Home
//						if(pcState == PCGame.HOME)
//						{
//							//If on party button, move to box title
//							if(boxChoice == 30)
//							{
//								boxChoice = -2;
//							} //end if
//
//							//Otherwise increase (lower slots on higher children)
//							else
//							{
//								boxChoice += 6;
//
//								//Clamp to 30 (party button)
//								if(boxChoice > 29)
//								{
//									boxChoice = 30;
//								} //end if
//							} //end else
//						} //end if PC Home
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY)
//						{
//							//If there is a held pokemon
//							if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
//							{
//								//If on last, or one after last slot, go to Close button
//								if(choiceNumber == GameManager.instance.GetTrainer().Team.Count ||
//									choiceNumber == GameManager.instance.GetTrainer().Team.Count+1)
//								{
//									choiceNumber = 0;
//									currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
//								} //end if
//								//If on Close button, go to first slot
//								else if(choiceNumber == 0)
//								{
//									choiceNumber = 1;
//									currentTeamSlot = partyTab.transform.FindChild("Pokemon1").gameObject;
//								} //end else if
//								//Go down vertically
//								else
//								{
//									choiceNumber += 2;
//									currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//								} //end else
//							} //end if
//							//If on last, or second to last team slot, go to Close button
//							else if((choiceNumber == GameManager.instance.GetTrainer().Team.Count - 1
//								&& choiceNumber > 0)
//								|| choiceNumber == GameManager.instance.GetTrainer().Team.Count)
//							{
//								choiceNumber = 0;
//								currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
//							} //end else if
//							//If on Close button, go to first slot
//							else if(choiceNumber == 0)
//							{
//								choiceNumber = 1;
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon1").gameObject;
//							} //end else if
//							//Go down vertically
//							else
//							{
//								choiceNumber += 2;
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Submenu on PC
//						else if(pcState == PCGame.POKEMONSUBMENU)
//						{
//							//Increase choice (lower slots on higher children)
//							subMenuChoice++;
//
//							//If on the last option, loop to first
//							if(subMenuChoice > choices.transform.childCount-1)
//							{
//								subMenuChoice = 0;
//							} //end if
//
//							//Reposition selection
//							selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
//						} //end else if Pokemon Submenu on PC
//
//						//Pokemon Summary on PC -> Summary
//						else if(pcState == PCGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//If party is open
//								if(partyTab.activeSelf && heldPokemon == null)
//								{
//									//Increase (lower slots are on higher childs)
//									choiceNumber++;
//
//									//Clamp between 1 and team size
//									if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
//									{
//										choiceNumber = 1;
//									} //end if
//								} //end if
//								//If in pokemon region
//								else if(!partyTab.activeSelf && heldPokemon == null)
//								{
//									//Increase to next pokemon slot
//									boxChoice = GameManager.instance.GetTrainer().GetNextPokemon(boxChoice);
//								} //end else if
//							} //end else if
//							else
//							{
//								//Increase (lower slots are on higher childs)
//								moveChoice++;
//
//								//If chosen move is null, loop to top
//								if(moveChoice >= 4 || selectedPokemon.GetMove(moveChoice) == -1)
//								{
//									moveChoice = 0;
//								} //end if
//
//								//Set move slot
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move"+(moveChoice+1)).gameObject;
//							} //end else
//						} //end else if Pokemon Summary on PC -> Summary
//
//						//Move Switch on PC -> Summary -> Move Details
//						else if(pcState == PCGame.MOVESWITCH)
//						{
//							//Increase (lower slots on higher children)
//							switchChoice++;
//
//							//Clamp between 0 and highest non-null move
//							if(switchChoice > selectedPokemon.GetMoveCount()-1)
//							{
//								switchChoice = 0;
//							} //end if
//
//							//Set currentSwitchSlot
//							currentSwitchSlot = summaryScreen.transform.GetChild(5).
//								FindChild("Move"+(switchChoice+1)).gameObject;
//						} //end else if Move Switch on PC -> Summary -> Move Details
//
//						//Pokemon Ribbons in PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS)
//						{
//							//If party is open
//							if(partyTab.activeSelf && heldPokemon == null)
//							{
//								//Increase (lower slots are on higher childs)
//								choiceNumber++;
//
//								//Clamp between 1 and team size
//								if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
//								{
//									choiceNumber = 1;
//								} //end if
//
//								//Update selected pokemon
//								selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];
//							} //end if
//							//If in pokemon region
//							else if(!partyTab.activeSelf && heldPokemon == null)
//							{
//								//Increase to next pokemon slot
//								boxChoice = GameManager.instance.GetTrainer().GetNextPokemon(boxChoice);
//
//								//Update selected pokemon
//								selectedPokemon = GameManager.instance.GetTrainer().GetPC(
//									GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
//							} //end else if
//
//							//Reload ribbons
//							initialize = false;
//						} //end else if Pokemon Ribbons in PC -> Ribbons
//
//						//Pokemon Markings on PC -> Submenu
//						else if(pcState == PCGame.POKEMONMARKINGS)
//						{
//							//Increase choice (lower slots on higher children)
//							subMenuChoice++;
//
//							//If on the last option, loop to first
//							if(subMenuChoice > choices.transform.childCount-1)
//							{
//								subMenuChoice = 0;
//							} //end if
//						} //end else if Pokemon Markings on PC -> Submenu
//						break;
//					} //end case OverallState PC
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
//			//Scene switch
//			switch(sceneState)
//			{
//
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						//If the mouse is lower than the selection box, and it moved, reposition to next choice
//						if(checkpoint == 8 && Input.mousePosition.y < selection.transform.position.y-1)
//						{
//							//Make sure it's not on last choice
//							if(choiceNumber == 0)
//							{
//								//Update choiceNumber
//								choiceNumber++;
//
//								//Reposition to choice location
//								selection.transform.position = new Vector3(
//									confirm.transform.GetChild(choiceNumber).position.x,
//									confirm.transform.GetChild(choiceNumber).position.y, 100);
//							} //end if
//						} //end if
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon submenu on Continue Game -> My Team
//						if(gameState == MainGame.POKEMONSUBMENU && Input.mousePosition.y < 
//							selection.transform.position.y - selection.GetComponent<RectTransform>().
//							rect.height/2)
//						{
//							//If not on the last option, increase (lower slots on higher children)
//							if(subMenuChoice < choices.transform.childCount-1)
//							{
//								subMenuChoice++;
//							} //end if
//
//							//Reposition selection
//							selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
//						} //end if Pokemon submenu on Continue Game -> My Team
//
//						//Continue Game -> My Team
//						else if(gameState == MainGame.TEAM &&
//							Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentTeamSlot.transform.
//								position).y - currentTeamSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If on last or second to last slot, go to PC button
//							if((choiceNumber == GameManager.instance.GetTrainer().Team.Count - 1
//								&& choiceNumber > 0)
//								|| choiceNumber == GameManager.instance.GetTrainer().Team.Count)
//							{
//								choiceNumber = -1;
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
//							} //end if
//							//If on Cancel button, stay on Cancel button (Causes glitches when mouse goes below button)
//							else if(choiceNumber == 0)
//							{
//								choiceNumber = 0;
//							} //end else if
//							//If on PC button, go to Cancel button
//							else if(choiceNumber == -1)
//							{
//								choiceNumber = 0;
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(1).gameObject;
//							} //end else if
//							//Go down vertically
//							else
//							{
//								choiceNumber += 2;
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else
//						} //end else if Continue Game -> My Team
//
//						//Pokemon Switch on Continue Game -> Switch
//						else if(gameState == MainGame.POKEMONSWITCH &&
//							Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
//								position).y - currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If not on second to last or last slot, go to down vertically
//							if(switchChoice < GameManager.instance.GetTrainer().Team.Count - 1)
//							{
//								switchChoice += 2;
//								currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
//							} //end else
//						} //end else if Pokemon Switch on Continue Game -> Switch
//
//						//Pokemon Summary on Continue Game -> Summary
//						else if(gameState == MainGame.POKEMONSUMMARY && summaryChoice == 5 &&
//							Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentMoveSlot.transform.
//								position).y - currentMoveSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If not on the last slot
//							if(moveChoice < 3)
//							{
//								//If next slot is null, don't move
//								if(moveChoice < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount()-1)
//								{
//									moveChoice++;
//								} //end if
//
//								//Set currentMoveSlot
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move"+(moveChoice+1)).gameObject;
//							} //end if
//						} //end else if Pokemon Summary on Continue Game -> Summary
//
//						//Move Switch on Continue Game -> Summary -> Move Details
//						else if(gameState  == MainGame.MOVESWITCH &&
//							Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
//								position).y - currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If next slot is null, don't move
//							if(switchChoice < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetMoveCount() - 1)
//							{
//								switchChoice++;
//							} //end if
//
//							//Set currentSwitchSlot
//							currentSwitchSlot = summaryScreen.transform.GetChild(5).
//								FindChild("Move"+(switchChoice+1)).gameObject;
//						} //end else if Move Switch on Continue Game -> Summary -> Move Details
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState  == MainGame.POKEMONRIBBONS &&
//							Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
//								position).y - currentRibbonSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If next slot is null, don't move
//							if(ribbonChoice+4 < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount())
//							{
//								ribbonChoice += 4;
//
//								//Read ribbon
//								ReadRibbon();
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//						break;
//					} //end case OverallGame CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						//PC Home
//						if(pcState == PCGame.HOME && Input.mousePosition.y < 
//							Camera.main.WorldToScreenPoint(currentPCSlot.transform.position).y - 
//							currentPCSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If not at bottom of PC
//							if(boxChoice < 24)
//							{
//								boxChoice += 6;
//							} //end if
//							//Otherwise go to nearest button
//							else if(boxChoice < 27)
//							{
//								//Set to party button
//								boxChoice = 30;
//							} //end else if
//							else if(boxChoice < 30)
//							{
//								//Set to return button
//								boxChoice = 31;
//							} //end else
//						} //end if PC Home
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY && Input.mousePosition.y < 
//							Camera.main.WorldToScreenPoint(currentTeamSlot.transform.position).y - 
//							currentTeamSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If there is a held pokemon
//							if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
//							{
//								//If on last, or one after last slot, go to Close button
//								if(choiceNumber == GameManager.instance.GetTrainer().Team.Count ||
//									choiceNumber == GameManager.instance.GetTrainer().Team.Count+1)
//								{
//									choiceNumber = 0;
//									currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
//								} //end if
//								//If on Cancel button, stay on Cancel button (Causes glitches when mouse goes below button)
//								else if(choiceNumber == 0)
//								{
//									choiceNumber = 0;
//									currentTeamSlot = partyTab.transform.FindChild("Pokemon1").gameObject;
//								} //end else if
//								//Go down vertically
//								else
//								{
//									choiceNumber += 2;
//									currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//								} //end else
//							} //end if
//							//If on last or second to last slot, go to Close button
//							else if((choiceNumber == GameManager.instance.GetTrainer().Team.Count - 1
//								&& choiceNumber > 0)
//								|| choiceNumber == GameManager.instance.GetTrainer().Team.Count)
//							{
//								choiceNumber = 0;
//								currentTeamSlot = partyTab.transform.FindChild("Close").gameObject;
//							} //end else if
//							//If on Cancel button, stay on Cancel button (Causes glitches when mouse goes below button)
//							else if(choiceNumber == 0)
//							{
//								choiceNumber = 0;
//							} //end else if
//							//Go down vertically
//							else
//							{
//								choiceNumber += 2;
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Submenu on PC
//						else if(pcState == PCGame.POKEMONSUBMENU && Input.mousePosition.y < 
//							selection.transform.position.y - selection.GetComponent<RectTransform>().
//							rect.height/2)
//						{
//							//If not on the last option, increase (lower slots on higher children)
//							if(subMenuChoice < choices.transform.childCount-1)
//							{
//								subMenuChoice++;
//							} //end if
//
//							//Reposition selection
//							selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
//						} //end else if Pokemon Submenu on PC
//
//						//Pokemon Summary on PC -> Summary
//						else if(pcState == PCGame.POKEMONSUMMARY && summaryChoice == 5 &&
//							Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentMoveSlot.transform.
//								position).y - currentMoveSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If not on the last slot
//							if(moveChoice < 3)
//							{
//								//If next slot is null, don't move
//								if(moveChoice < selectedPokemon.GetMoveCount() - 1)
//								{
//									moveChoice++;
//								} //end if
//
//								//Set currentMoveSlot
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move"+(moveChoice+1)).gameObject;
//							} //end if
//						} //end else if Pokemon Summary on PC -> Summary
//
//						//Move Switch on PC -> Summary -> Move Details
//						else if(pcState == PCGame.MOVESWITCH &&
//							Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
//								position).y - currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If next slot is null, don't move
//							if(switchChoice < selectedPokemon.GetMoveCount() - 1)
//							{
//								switchChoice++;
//							} //end if
//
//							//Set currentSwitchSlot
//							currentSwitchSlot = summaryScreen.transform.GetChild(5).
//								FindChild("Move"+(switchChoice+1)).gameObject;
//						} //end else if Move Switch on PC -> Summary -> Move Details
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS &&
//							Input.mousePosition.y < Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
//								position).y - currentRibbonSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If next slot is null, don't move
//							if(ribbonChoice+4 < selectedPokemon.GetRibbonCount())
//							{
//								ribbonChoice += 4;
//
//								//Read ribbon
//								ReadRibbon();
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//
//						//Pokemon Markings on PC -> Submenu
//						else if(pcState == PCGame.POKEMONMARKINGS && Input.mousePosition.y < 
//							selection.transform.position.y-1)
//						{
//							//If not on the last option, increase (lower slots on higher children)
//							if(subMenuChoice < choices.transform.childCount-1)
//							{
//								subMenuChoice++;
//							} //end if
//						} //end else if Pokemon Markings on PC -> Submenu
//						break;
//					} //end case OverallGame PC
//			} //end scene switch
//		} //end else if Mouse Moves Down
//
//		/***********************************************
//         * Mouse Moves Up
//         ***********************************************/ 
//		else if(Input.GetAxis("Mouse Y") > 0)
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						//If the mouse is higher than the selection box, and it moved, reposition to next choice
//						if(checkpoint == 8 && Input.mousePosition.y > selection.transform.position.y+1)
//						{
//							//Make sure it's not on last choice
//							if(choiceNumber == 1)
//							{
//								//Update choiceNumber
//								choiceNumber--;
//
//								//Reposition to choice location
//								selection.transform.position = new Vector3(
//									confirm.transform.GetChild(choiceNumber).position.x,
//									confirm.transform.GetChild(choiceNumber).position.y, 100);
//							} //end if
//						} //end if
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon submenu on Continue Game -> My Team
//						if(gameState == MainGame.POKEMONSUBMENU && Input.mousePosition.y > 
//							selection.transform.position.y + selection.GetComponent<RectTransform>().
//							rect.height/2)
//						{
//							//If not on the first option, decrease
//							if(subMenuChoice > 0)
//							{
//								subMenuChoice--;
//							} //end if
//
//							//Reposition selection
//							selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
//						} //end if Pokemon submenu on Continue Game -> My Team
//
//						//Continue Game -> My Team
//						else if(gameState == MainGame.TEAM &&
//							Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentTeamSlot.transform.
//								position).y + currentTeamSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If on a top slot, stay there (Causes glitches when mouse is above slot)
//							if(choiceNumber == 1 || choiceNumber == 2)
//							{
//								choiceNumber = choiceNumber;
//							} //end if
//							//If on PC button, go to last team slot
//							else if(choiceNumber == -1)
//							{
//								choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else if
//							//If on Cancel button, go to PC button
//							else if(choiceNumber == 0)
//							{
//								choiceNumber = -1;
//								currentTeamSlot = playerTeam.transform.FindChild("Buttons").GetChild(0).gameObject;
//							} //end else if
//							//Go up vertically
//							else
//							{
//								choiceNumber -= 2;
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else
//						} //end else if Continue Game -> My Team
//
//						//Pokemon Switch on Continue Game -> Switch
//						else if(gameState == MainGame.POKEMONSWITCH &&
//							Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
//								position).y + currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If not on a top slot, go up vertically
//							if(switchChoice > 2)
//							{
//								switchChoice -= 2;
//								currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
//							} //end else
//						} //end else if Pokemon Switch on Continue Game -> Switch
//
//						//Pokemon Summary on Continue Game -> Summary
//						else if(gameState == MainGame.POKEMONSUMMARY && summaryChoice == 5 &&
//							Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentMoveSlot.transform.
//								position).y + currentMoveSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If not on first slot, go up vertically
//							if(moveChoice > 0)
//							{
//								moveChoice--;
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move"+(moveChoice+1)).gameObject;
//							} //end if
//						} //end else if Pokemon Summary on Continue Game -> Summary
//
//						//Move Switch on Continue Game -> Summary -> Move Details
//						else if(gameState  == MainGame.MOVESWITCH &&
//							Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
//								position).y + currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If not on first slot ,go up vertically
//							if(switchChoice > 0)
//							{
//								switchChoice--;                        
//								currentSwitchSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move"+(switchChoice+1)).gameObject;
//							} //end if
//						} //end else if Move Switch on Continue Game -> Summary -> Move Details
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState  == MainGame.POKEMONRIBBONS &&
//							Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
//								position).y + currentRibbonSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If next slot is null, don't move
//							if(ribbonChoice-4 > -1)
//							{
//								ribbonChoice -= 4;
//
//								//Read ribbon
//								ReadRibbon();
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//						break;
//					} //end case OverallGame CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						//PC Home
//						if(pcState == PCGame.HOME && Input.mousePosition.y > 
//							Camera.main.WorldToScreenPoint(currentPCSlot.transform.position).y + 
//							currentPCSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If not at top of PC
//							if(boxChoice > 5)
//							{
//								boxChoice -= 6;
//							} //end if
//							else
//							{
//								//Go to title                           
//								boxChoice = -2;
//							} //end else
//						} //end if Home
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY && Input.mousePosition.y > 
//							Camera.main.WorldToScreenPoint(currentTeamSlot.transform.position).y + 
//							currentTeamSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If on a top slot, stay there (Causes glitches when mouse is above slot)
//							if(choiceNumber == 1 || choiceNumber == 2)
//							{
//								choiceNumber = choiceNumber;
//							} //end if
//							//If on Close button, go to last team slot
//							else if(choiceNumber == 0)
//							{
//								if(heldPokemon != null && GameManager.instance.GetTrainer().Team.Count < 6)
//								{
//									choiceNumber = GameManager.instance.GetTrainer().Team.Count + 1;
//								} //end if
//								else
//								{
//									choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//								} //end else
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else if
//							//Go up vertically
//							else
//							{
//								choiceNumber -= 2;
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else
//						} //end else if Continue Game -> My Team
//
//						//Pokemon Submenu on PC
//						if(pcState == PCGame.POKEMONSUBMENU && Input.mousePosition.y > 
//							selection.transform.position.y + selection.GetComponent<RectTransform>().
//							rect.height/2)
//						{
//							//If not on the first option, decrease
//							if(subMenuChoice > 0)
//							{
//								subMenuChoice--;
//							} //end if
//
//							//Reposition selection
//							selection.transform.position = choices.transform.GetChild(subMenuChoice).position;
//						} //end if Pokemon Submenu on PC
//
//						//Pokemon Summary on PC -> Summary
//						else if(pcState == PCGame.POKEMONSUMMARY && summaryChoice == 5 &&
//							Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentMoveSlot.transform.
//								position).y + currentMoveSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If not on first slot, go up vertically
//							if(moveChoice > 0)
//							{
//								moveChoice--;
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move"+(moveChoice+1)).gameObject;
//							} //end if
//						} //end else if Pokemon Summary on PC -> Summary
//
//						//Move Switch on PC -> Summary -> Move Details
//						else if(pcState == PCGame.MOVESWITCH &&
//							Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
//								position).y + currentSwitchSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If not on first slot, go up vertically
//							if(switchChoice > 0)
//							{
//								switchChoice--;                        
//								currentSwitchSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move"+(switchChoice+1)).gameObject;
//							} //end if
//						} //end else if Move Switch on PC -> Summary -> Move Details
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS &&
//							Input.mousePosition.y > Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
//								position).y + currentRibbonSlot.GetComponent<RectTransform>().rect.height/2)
//						{
//							//If next slot is null, don't move
//							if(ribbonChoice-4 > -1)
//							{
//								ribbonChoice -= 4;
//
//								//Read ribbon
//								ReadRibbon();
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//
//						//Pokemon Markings on PC -> Submenu
//						else if(pcState == PCGame.POKEMONMARKINGS && Input.mousePosition.y > 
//							selection.transform.position.y+1)
//						{
//							//If not on the first option, decrease
//							if(subMenuChoice > 0)
//							{
//								subMenuChoice--;
//							} //end if
//						} //end else if Pokemon Markings on PC -> Submenu
//						break;
//					} //end case OverallGame PC
//			} //end scene switch
//		} //end else if Mouse Moves Up
//
//		/***********************************************
//         * Mouse Moves Left
//         ***********************************************/ 
//		if(Input.GetAxis("Mouse X") < 0)
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//				//Intro
//				case OverallGame.INTRO:
//					{
//						break;
//					} //end case OverallGame INTRO
//
//					//Menu
//				case OverallGame.MENU:
//					{
//						break;
//					} //end case OverallGame MENU
//
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Continue Game -> My Team
//						if(gameState == MainGame.TEAM &&
//							Input.mousePosition.x < Camera.main.WorldToScreenPoint(currentTeamSlot.transform.
//								position).x - currentTeamSlot.GetComponent<RectTransform>().rect.width/2)
//						{
//							//If choice number is not odd, and is greater than 0, move left
//							if((choiceNumber&1) != 1 && choiceNumber > 0)
//							{
//								choiceNumber--;
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end if   
//						} //end if Continue Game -> My Team
//
//						//Pokemon Switch on Continue Game -> Switch
//						if(gameState == MainGame.POKEMONSWITCH &&
//							Input.mousePosition.x < Camera.main.WorldToScreenPoint(currentSwitchSlot.transform.
//								position).x - currentSwitchSlot.GetComponent<RectTransform>().rect.width/2)
//						{
//							//If switch number is not odd, move left
//							if((switchChoice&1) != 1)
//							{
//								switchChoice--;
//								currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
//							} //end if   
//						} //end if Pokemon Switch on Continue Game -> Switch
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState  == MainGame.POKEMONRIBBONS &&
//							Input.mousePosition.x < Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
//								position).x - currentRibbonSlot.GetComponent<RectTransform>().rect.width/2)
//						{
//							//If next slot is null, don't move
//							if(ribbonChoice-1 > -1 && ribbonChoice % 4 != 0)
//							{
//								ribbonChoice--;
//
//								//Read ribbon
//								ReadRibbon();
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//						break;
//					} //end OverallGame CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						if(pcState == PCGame.HOME && Input.mousePosition.x < 
//							Camera.main.WorldToScreenPoint(currentPCSlot.transform.position).x - 
//							currentPCSlot.GetComponent<RectTransform>().rect.width/2)
//						{
//							//If not at bottom of PC or on left side
//							if(boxChoice < 30 && boxChoice % 6 != 0)
//							{
//								boxChoice--;
//							} //end if
//							else if(boxChoice == 31)
//							{
//								//Set to party button
//								boxChoice = 30;
//							} //end else if
//							else if(boxChoice == -2)
//							{
//								//Set to left box
//								boxChoice = -3;
//							} //end else
//						} //end if
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY && Input.mousePosition.x < 
//							Camera.main.WorldToScreenPoint(currentTeamSlot.transform.position).x - 
//							currentTeamSlot.GetComponent<RectTransform>().rect.width/2)
//						{
//							//If choice number is not odd, and is greater than 0, move left
//							if((choiceNumber&1) != 1 && choiceNumber > 0)
//							{
//								choiceNumber--;
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end if   
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS &&
//							Input.mousePosition.x < Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
//								position).x - currentRibbonSlot.GetComponent<RectTransform>().rect.width/2)
//						{
//							//If next slot is null, don't move
//							if(ribbonChoice-1 > -1 && ribbonChoice % 4 != 0)
//							{
//								ribbonChoice--;
//
//								//Read ribbon
//								ReadRibbon();
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//						break;
//					} //end case OverallGame PC
//			} //end scene switch
//		} //end if Mouse Moves Left
//
//		/***********************************************
//         * Mouse Moves Right
//         ***********************************************/ 
//		else if(Input.GetAxis("Mouse X") > 0)
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//				//Intro
//				case OverallGame.INTRO:
//					{
//						break;
//					} //end case OverallGame INTRO
//
//					//Menu
//				case OverallGame.MENU:
//					{
//						break;
//					} //end case OverallGame MENU
//
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Continue Game -> My Team
//						if(gameState == MainGame.TEAM && Input.mousePosition.x > Camera.main.
//							WorldToScreenPoint(currentTeamSlot.transform.position).x + currentTeamSlot.
//							GetComponent<RectTransform>().rect.width/2)
//						{
//							//If choice is odd and team is not odd numbered and choice is greater than 0, move right
//							if((choiceNumber&1) == 1 && choiceNumber != GameManager.instance.GetTrainer().Team.Count
//								&& choiceNumber > 0)
//							{
//								choiceNumber++;
//								currentTeamSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end if   
//						} //end if Continue Game -> My Team
//
//						//Pokemon Switch on Continue Game -> Switch
//						if(gameState == MainGame.POKEMONSWITCH && Input.mousePosition.x > Camera.main.
//							WorldToScreenPoint(currentSwitchSlot.transform.position).x + currentSwitchSlot.
//							GetComponent<RectTransform>().rect.width/2)
//						{
//							//If switch is odd and team is not odd numbered, move right
//							if((switchChoice&1) == 1 && switchChoice != GameManager.instance.GetTrainer().Team.Count)
//							{
//								switchChoice++;
//								currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+switchChoice).gameObject;
//							} //end if   
//						} //end if Pokemon Switch on Continue Game -> Switch
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState  == MainGame.POKEMONRIBBONS &&
//							Input.mousePosition.x > Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
//								position).x + currentRibbonSlot.GetComponent<RectTransform>().rect.width/2)
//						{
//							//If next slot is null, don't move
//							if(ribbonChoice + 1 < GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount() &&
//								ribbonChoice % 4 != 3)
//							{
//								ribbonChoice++;
//
//								//Read Ribbon
//								ReadRibbon();
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//						break;
//					} //end OverallGame CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						if(pcState == PCGame.HOME && Input.mousePosition.x > 
//							Camera.main.WorldToScreenPoint(currentPCSlot.transform.position).x + 
//							currentPCSlot.GetComponent<RectTransform>().rect.width/2)
//						{
//							//If not at bottom of PC or on right side
//							if(boxChoice < 30 && boxChoice % 6 != 5)
//							{
//								boxChoice++;
//							} //end if
//							else if(boxChoice == 30)
//							{
//								//Set to return button
//								boxChoice = 31;
//							} //end else if
//							else if(boxChoice == -2)
//							{
//								//Set to right box
//								boxChoice = -1;
//							} //end else
//						} //end if
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY && Input.mousePosition.x > Camera.main.
//							WorldToScreenPoint(currentTeamSlot.transform.position).x + currentTeamSlot.
//							GetComponent<RectTransform>().rect.width/2)
//						{
//							//If choice is odd, and currently less than or equal to team size, and a pokemon is held, move right
//							if((choiceNumber&1) == 1 && choiceNumber <= GameManager.instance.GetTrainer().Team.Count && 
//								heldPokemon != null)
//							{
//								choiceNumber++;
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end if
//							//If choice is odd and team is not odd numbered and choice is greater than 0, move right
//							else  if((choiceNumber&1) == 1 && choiceNumber != GameManager.instance.GetTrainer().Team.Count && 
//								choiceNumber > 0 && heldPokemon == null)
//							{
//								choiceNumber++;
//								currentTeamSlot = partyTab.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//							} //end else if
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS &&
//							Input.mousePosition.x > Camera.main.WorldToScreenPoint(currentRibbonSlot.transform.
//								position).x + currentRibbonSlot.GetComponent<RectTransform>().rect.width/2)
//						{
//							//If next slot is null, don't move
//							if(ribbonChoice + 1 < selectedPokemon.GetRibbonCount() && ribbonChoice % 4 != 3)
//							{
//								ribbonChoice++;
//
//								//Read Ribbon
//								ReadRibbon();
//							} //end if
//
//							//Set currentRibbonSlot
//							currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//								GetChild(ribbonChoice).gameObject;
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//						break;
//					} //end case OverallGame PC
//			} //end scene switch
//		} //end else if Mouse Moves Right
//
//		/***********************************************
//         * Left Mouse Button
//         ***********************************************/ 
//		else if(Input.GetMouseButtonUp(0))
//		{
//			//Scene switch
//			switch(sceneState)
//			{
//				//Intro
//				case OverallGame.INTRO:
//					{
//						//If player cancels animation
//						if(playing && processing)
//						{
//							playing = false;
//						} //end if
//
//						//Fade out in prepareation for menu scene
//						else if(checkpoint == 3)
//						{
//							playing = true;
//							StartCoroutine(FadeOutAnimation(4));
//						} //end else if
//						break;
//					} //end case OverallGame INTRO
//
//					//Menu
//				case OverallGame.MENU:
//					{
//						break;
//					} //end case OverallGame MENU
//
//					//New Game
//				case OverallGame.NEWGAME:
//					{

//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon submenu on Continue Game -> My Team is Open
//						if(gameState == MainGame.POKEMONSUBMENU)
//						{
//							//Apply appropriate action based on submenu selection
//							switch(subMenuChoice)
//							{
//								//Summary
//								case 0:
//									{
//										selection.SetActive(false);
//										choices.SetActive(false);
//										summaryScreen.SetActive(true);
//										summaryChoice = 0;
//										summaryScreen.transform.GetChild(0).gameObject.SetActive(false);
//										summaryScreen.transform.GetChild(1).gameObject.SetActive(false);
//										summaryScreen.transform.GetChild(2).gameObject.SetActive(false);
//										summaryScreen.transform.GetChild(3).gameObject.SetActive(false);
//										summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
//										summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
//										gameState = MainGame.POKEMONSUMMARY;
//										break;
//									} //end case 0 (Summary)
//
//									//Switch
//								case 1:
//									{
//										selection.SetActive(false);
//										choices.SetActive(false);
//										switchChoice = choiceNumber;
//										previousSwitchSlot = choiceNumber;
//										currentSwitchSlot = playerTeam.transform.FindChild("Pokemon" + choiceNumber).gameObject;
//										gameState = MainGame.POKEMONSWITCH;
//										break;
//									} //end case 1 (Switch)
//
//									//Item
//								case 2:
//									{
//										if(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item == 0)
//										{
//											GameManager.instance.GetTrainer().Team[choiceNumber-1].Item=
//												GameManager.instance.RandomInt(1, 500);
//										}
//										else
//										{
//											GameManager.instance.GetTrainer().Team[choiceNumber-1].Item = 0;
//										} //end else
//										selection.SetActive(false);
//										choices.SetActive(false);
//										initialize = false;
//										gameState = MainGame.TEAM;
//										break;
//									} //end case 2 (Item)
//
//									//Ribbons
//								case 3:
//									{
//										initialize = false;
//										choices.SetActive(false);
//										selection.SetActive(false);
//										currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//											GetChild(0).gameObject;
//										gameState = MainGame.POKEMONRIBBONS;
//										break;
//									} //end case 3 (Ribbons)
//
//									//Cancel
//								case 4:
//									{
//										choices.SetActive(false);
//										selection.SetActive(false);
//										gameState = MainGame.TEAM;
//										break;
//									} //end case 4 (Cancel)
//							} //end switch
//						} //end if Pokemon submenu on Continue Game -> My Team is Open
//
//						//Open menu if not open, as long as player isn't selecting a button
//						else if(gameState == MainGame.TEAM && choiceNumber > 0)
//						{
//							//Set submenu active
//							choices.SetActive(true);
//							selection.SetActive(true);
//
//							//Set up selection box at end of frame if it doesn't fit
//							if(selection.GetComponent<RectTransform>().sizeDelta != 
//								choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
//							{
//								selection.SetActive(false);
//								StartCoroutine("WaitForResize");
//							} //end if
//
//							//Reset position to top of menu
//							initialize = false;
//							subMenuChoice = 0;
//							gameState = MainGame.POKEMONSUBMENU;
//						} //end else if Open menu if not open
//
//						//Pokemon Switch on Continue Game -> Switch
//						else if(gameState == MainGame.POKEMONSWITCH)
//						{
//							//If the choice equals the switch, don't run swap
//							if(choiceNumber != switchChoice)
//							{
//								//Switch function
//								GameManager.instance.GetTrainer().Swap(choiceNumber-1, switchChoice-1);
//							} //end if
//
//							//Go back to team
//							initialize = false;
//							gameState = MainGame.TEAM;
//						} //end else if Pokemon Switch on Continue Game -> Switch
//
//						//Pokemon Summary on Continue Game -> Summary
//						else if(gameState == MainGame.POKEMONSUMMARY)
//						{
//							//If on moves screen, switch to move details
//							if(summaryChoice == 4)
//							{
//								moveChoice = 0;
//								summaryChoice = 5;
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move1").gameObject;
//							} //end if
//
//							//If on move details screen, go to move switch
//							else if(summaryChoice == 5)
//							{
//								currentMoveSlot.GetComponent<Image>().color = Color.white;
//								switchChoice = moveChoice;
//								currentSwitchSlot = currentMoveSlot;
//								gameState = MainGame.MOVESWITCH;
//							} //end else if
//						} //end else if Pokemon Summary on Continue Game -> Summary
//
//						//Move Switch on Continue Game -> Summary -> Move Details
//						else if(gameState  == MainGame.MOVESWITCH)
//						{
//							//If switching spots aren't the same
//							if(moveChoice != switchChoice)
//							{
//								GameManager.instance.GetTrainer().Team[choiceNumber-1].SwitchMoves(moveChoice, switchChoice);
//							} //end if
//
//							currentMoveSlot.GetComponent<Image>().color = Color.clear;
//							gameState = MainGame.POKEMONSUMMARY;
//						} //end else if Move Switch on Continue Game -> Summary -> Move Details
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState  == MainGame.POKEMONRIBBONS)
//						{
//							//Make sure there are ribbons to be read
//							if(GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount() > 0)
//							{
//								selection.SetActive(!selection.activeSelf);
//								ReadRibbon();
//							} //end if
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//						break;
//					} //end case OverallGame CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						//PC Home
//						if(pcState == PCGame.HOME && (selectedPokemon != null || heldPokemon != null || boxChoice == -2))
//						{
//							//Open submenu as long as player is in pokemon region or on title
//							if((boxChoice > -1 && boxChoice < 30) || boxChoice == -2)
//							{
//								//Set submenu active
//								choices.SetActive(true);
//								selection.SetActive(true);
//
//								//Set up selection box at end of frame if it doesn't fit
//								if(selection.GetComponent<RectTransform>().sizeDelta != 
//									choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
//								{
//									selection.SetActive(false);
//									StartCoroutine("WaitForResize");
//								} //end if
//
//								//Fill in choices
//								FillInChoices();
//
//								//Reset position to top of menu
//								subMenuChoice = 0;
//								initialize = false;
//								pcState = PCGame.POKEMONSUBMENU;
//							} //end if  
//						} //end if Home
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY && (selectedPokemon != null || heldPokemon != null))
//						{
//							//Open submenu as long as player is in party
//							if(choiceNumber > 0)
//							{
//								//Set submenu active
//								choices.SetActive(true);
//								selection.SetActive(true);
//
//								//Set up selection box at end of frame if it doesn't fit
//								if(selection.GetComponent<RectTransform>().sizeDelta != 
//									choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
//								{
//									selection.SetActive(false);
//									StartCoroutine("WaitForResize");
//								} //end if
//
//								//Fill in choices
//								FillInChoices(); 
//
//								//Reset position to top of menu
//								subMenuChoice = 0;
//								initialize = false;
//								pcState = PCGame.POKEMONSUBMENU;
//							} //end if  
//							//Close submenu if open
//							else
//							{
//								choices.SetActive(false);
//								selection.SetActive(false);
//							} //end else
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Submenu on PC
//						else if(pcState == PCGame.POKEMONSUBMENU)
//						{
//							if(boxChoice != -2)
//							{
//								//Apply appropriate action based on submenu selection
//								switch(subMenuChoice)
//								{
//									//Move
//									case 0:
//										{
//											selection.SetActive(false);
//											choices.SetActive(false);
//											pcState = PCGame.POKEMONHELD;
//											break;
//										} //end case 0 (Move)
//
//										//Summary
//									case 1:
//										{
//											selection.SetActive(false);
//											choices.SetActive(false);
//											summaryScreen.SetActive(true);
//											summaryChoice = 0;
//											summaryScreen.transform.GetChild(0).gameObject.SetActive(false);
//											summaryScreen.transform.GetChild(1).gameObject.SetActive(false);
//											summaryScreen.transform.GetChild(2).gameObject.SetActive(false);
//											summaryScreen.transform.GetChild(3).gameObject.SetActive(false);
//											summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
//											summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
//											pcState = PCGame.POKEMONSUMMARY;
//											break;
//										} //end case 1 (Summary)
//
//										//Item
//									case 2:
//										{
//											selection.SetActive(false);
//											choices.SetActive(false);
//											if(selectedPokemon.Item == 0)
//											{
//												selectedPokemon.Item = GameManager.instance.RandomInt(1, 500);
//											} //end if
//											else
//											{
//												selectedPokemon.Item = 0;
//											} //end else
//											pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//											break;
//										} //end case 2 (Item)
//
//										//Ribbons
//									case 3:
//										{
//											initialize = false;
//											choices.SetActive(false);
//											selection.SetActive(false);
//											currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//												GetChild(0).gameObject;
//											pcState = PCGame.POKEMONRIBBONS;
//											break;
//										} //end case 3 (Ribbons)
//
//										//Markings
//									case 4:
//										{
//											markingChoices = heldPokemon == null ? selectedPokemon.GetMarkings().ToList() :
//												heldPokemon.GetMarkings().ToList();
//											initialize = false;
//											pcState = PCGame.POKEMONMARKINGS;
//											break;
//										} //end case 4 (Markings)
//
//										//Release
//									case 5:
//										{
//											//If party tab is open
//											if(partyTab.activeSelf && GameManager.instance.GetTrainer().Team.Count > 1)
//											{
//												//Get the pokemon
//												selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];
//
//												//Remove the pokemon from the party
//												GameManager.instance.GetTrainer().RemovePokemon(choiceNumber-1);
//
//												//Fill in party tab
//												for(int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
//												{
//													partyTab.transform.FindChild("Pokemon"+i).GetComponent<Image>().sprite =
//														GetCorrectIcon(GameManager.instance.GetTrainer().Team[i-1]);
//												} //end for
//
//												//Deactivate any empty party spots
//												for(int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
//												{
//													partyTab.transform.FindChild("Pokemon" + (i)).gameObject.SetActive(false);
//												} //end for
//											} //end if
//											else
//											{
//												//Get the pokemon
//												selectedPokemon = GameManager.instance.GetTrainer().GetPC(
//													GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
//
//												//Remove the pokemon from the PC
//												GameManager.instance.GetTrainer().RemoveFromPC(
//													GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
//
//												//Set PC slot to clear
//												boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).
//												GetComponent<Image>().color = Color.clear;
//											} //end else
//
//											choices.SetActive(false);
//											selection.SetActive(false);
//											GameManager.instance.DisplayText("You released " + selectedPokemon.Nickname, true);
//											selectedPokemon = null;
//											pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//											break;
//										} //end case 5 (Release)
//
//										//Cancel
//									case 6:
//										{
//											choices.SetActive(false);
//											selection.SetActive(false);
//											pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//											break;
//										} //end case 6 (Cancel)
//								} //end switch
//							} //end if
//							else
//							{
//								//Apply appropriate action based on subMenuChoice
//								switch(subMenuChoice)
//								{
//									//Jump To
//									case 0:
//										{
//											input.transform.GetChild(2).GetComponent<Text>().text = "What box do you want to jump to? (0-49)";
//											input.transform.GetChild(0).GetComponent<Text>().text = "Desired box:";
//											inputText.text = GameManager.instance.GetTrainer().GetPCBox().ToString();
//											choices.SetActive(false);
//											selection.SetActive(false);
//											input.SetActive(true);
//											pcState = PCGame.INPUTSCREEN;
//											break;
//										} //end case 0
//										//Change Name
//									case 1:
//										{
//											input.transform.GetChild(2).GetComponent<Text>().text = "Please enter box name.";
//											input.transform.GetChild(0).GetComponent<Text>().text = "Box name:";
//											inputText.text = GameManager.instance.GetTrainer().GetPCBoxName();
//											choices.SetActive(false);
//											selection.SetActive(false);
//											input.SetActive(true);
//											pcState = PCGame.INPUTSCREEN;
//											break;
//										} //end case 1
//										//Change Wallpaper
//									case 2:
//										{
//											input.transform.GetChild(2).GetComponent<Text>().text = "Choose a wallpaper. (0-24)";
//											input.transform.GetChild(0).GetComponent<Text>().text = "Wallpaper:";
//											inputText.text = GameManager.instance.GetTrainer().GetPCBoxWallpaper().ToString();
//											choices.SetActive(false);
//											selection.SetActive(false);
//											input.SetActive(true);
//											pcState = PCGame.INPUTSCREEN;
//											break;
//										} //end case 2
//										//Cancel
//									case 3:
//										{
//											choices.SetActive(false);
//											selection.SetActive(false);
//											pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//											break;
//										} //end case 3
//								} //end switch
//							} //end else
//						} //end else if Pokemon Submenu on PC
//
//						//Pokemon Summary on PC -> Summary
//						else if(pcState == PCGame.POKEMONSUMMARY)
//						{
//							//If on moves screen, switch to move details
//							if(summaryChoice == 4)
//							{
//								moveChoice = 0;
//								summaryChoice = 5;
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move1").gameObject;
//							} //end if
//
//							//If on move details screen, go to move switch
//							else if(summaryChoice == 5)
//							{
//								currentMoveSlot.GetComponent<Image>().color = Color.white;
//								switchChoice = moveChoice;
//								currentSwitchSlot = currentMoveSlot;
//								pcState = PCGame.MOVESWITCH;
//							} //end else if
//						} //end else if Pokemon Summary on PC -> Summary
//
//						//Move Switch on PC -> Summary -> Move Details
//						else if(pcState == PCGame.MOVESWITCH)
//						{
//							//If switching spots aren't the same
//							if(moveChoice != switchChoice)
//							{
//								selectedPokemon.SwitchMoves(moveChoice, switchChoice);
//							} //end if
//
//							currentMoveSlot.GetComponent<Image>().color = Color.clear;
//							pcState = PCGame.POKEMONSUMMARY;
//						} //end else if Move Switch on PC -> Summary -> Move Details
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS)
//						{
//							//Make sure there are ribbons to be read
//							if(selectedPokemon.GetRibbonCount() > 0)
//							{
//								selection.SetActive(!selection.activeSelf);
//								ReadRibbon();
//							} //end if
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//
//						//Pokemon Markings on PC -> Submenu
//						else if(pcState == PCGame.POKEMONMARKINGS)
//						{
//							if(subMenuChoice < DataContents.markingCharacters.Length)
//							{
//								//Turn the marking on or off
//								markingChoices[subMenuChoice] = !markingChoices[subMenuChoice];
//
//								//Color in choices
//								for(int i = 0; i < markingChoices.Count; i++)
//								{
//									choices.transform.GetChild(i).GetComponent<Text>().color =
//										markingChoices[i] ? Color.black : Color.gray;
//								} //end for
//							} //end if
//							else if(subMenuChoice == DataContents.markingCharacters.Length)
//							{
//								//Turn menu off                            
//								choices.SetActive(false);
//								selection.SetActive(false);
//
//								//If holding a pokemon
//								if(heldPokemon != null)
//								{
//									//Update held pokemon markings
//									heldPokemon.SetMarkings(markingChoices.ToArray());
//
//									//Return to home or party
//									pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//								} //end if
//								//If in party
//								else if(partyTab.activeSelf)
//								{
//									//Update team pokemon markings
//									GameManager.instance.GetTrainer().Team[choiceNumber-1].SetMarkings(markingChoices.ToArray());
//
//									//Return to party
//									pcState = PCGame.PARTY;
//								} //end else if
//								//In pokemon region
//								else
//								{
//									//Update pc box pokemon markings
//									GameManager.instance.GetTrainer().GetPC(
//										GameManager.instance.GetTrainer().GetPCBox(),
//										boxChoice).SetMarkings(markingChoices.ToArray());
//
//									//Return to home
//									pcState = PCGame.HOME;
//								} //end else
//							} //end else if
//							else if(subMenuChoice == DataContents.markingCharacters.Length+1)
//							{
//								//Turn menu off
//								FillInChoices();
//								choices.SetActive(false);
//								selection.SetActive(false);
//
//								//Return to home or party
//								pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//							} //end else if
//						} //end else if Pokemon Markings on PC -> Submenu
//
//						//Pokemon Input on PC -> Input
//						else if(pcState == PCGame.INPUTSCREEN)
//						{
//							//Handle according to subMenuChoice
//							switch(subMenuChoice)
//							{
//								//Jump to
//								case 0:
//									{
//										if (inputText.text.Length > 0)
//										{
//											int outputHolder = 0;
//											if (int.TryParse(inputText.text, out outputHolder))
//											{
//												outputHolder = ExtensionMethods.WithinIntRange(outputHolder, 0, 49);
//												GameManager.instance.GetTrainer().ChangeBox(outputHolder);
//											} //end if
//										} //end if
//										break;
//									} //end case 0
//									//Change name
//								case 1:
//									{
//										if (inputText.text.Length > 0)
//										{
//											GameManager.instance.GetTrainer().SetPCBoxName(inputText.text);
//										} //end if
//										break;
//									} //end case 1
//									//Change wallpaper
//								case 2:
//									{
//										if (inputText.text.Length > 0)
//										{
//											int outputHolder = 0;
//											if (int.TryParse(inputText.text, out outputHolder))
//											{
//												outputHolder = ExtensionMethods.WithinIntRange(outputHolder, 0, 24);
//												GameManager.instance.GetTrainer().SetPCBoxWallpaper(outputHolder);
//											} //end if
//										} //end if
//										break;
//									} //end case 2
//							} //end switch
//
//							//Turn input off
//							input.SetActive(false);
//
//							//Return to home
//							checkpoint = 1;
//							pcState = PCGame.HOME;
//						} //end else if Pokemon Input on PC -> Input
//						break;
//					} //end case OverallGame PC
//			} //end scene switch
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
//				//Intro
//				case OverallGame.INTRO:
//					{
//						break;
//					} //end case OverallGame INTRO
//
//					//Menu
//				case OverallGame.MENU:
//					{
//						break;
//					} //end case OverallGame MENU
//
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon Summary on Continue Game -> Summary
//						if(gameState == MainGame.POKEMONSUMMARY)
//						{
//							//If on any page besides details
//							if(summaryChoice != 5)
//							{
//								//Deactivate summary
//								summaryScreen.SetActive(false);
//
//								//Return to team
//								gameState = MainGame.TEAM;
//							} //end if
//							else
//							{
//								summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
//								selection.SetActive(false);
//								summaryChoice = 4;
//							} //end else
//						} //end if Pokemon Summary on Continue Game -> Summary
//
//						//Pokemon submenu on Continue Game -> My Team
//						else if(gameState == MainGame.POKEMONSUBMENU)
//						{
//							//Deactivate submenu 
//							choices.SetActive(false);
//							selection.SetActive(false);
//
//							gameState = MainGame.TEAM;
//						} //end else if Pokemon submenu on Continue Game -> My Team
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState == MainGame.POKEMONRIBBONS)
//						{
//							//Deactivate ribbons
//							ribbonScreen.SetActive(false);
//							selection.SetActive(false);
//							ribbonChoice = 0;
//							previousRibbonChoice = -1;
//
//							//Return to team
//							gameState = MainGame.TEAM;
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//
//						//Continue Game -> My Team
//						else if(gameState == MainGame.TEAM)
//						{
//							EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
//							gameState = MainGame.HOME;
//						} //end else if Continue Game -> My Team
//
//						//Pokemon Switch on Continue Game -> Switch
//						else if(gameState == MainGame.POKEMONSWITCH)
//						{
//							//Go back to team
//							gameState = MainGame.TEAM;
//						} //end else if Pokemon Switch on Continue Game -> Switch
//
//						//Move Switch on Continue Game -> Summary -> Move Details
//						else if(gameState  == MainGame.MOVESWITCH)
//						{
//							//Return to summary
//							currentMoveSlot.GetComponent<Image>().color = Color.clear;
//							gameState = MainGame.POKEMONSUMMARY;
//						} //end else if Move Switch on Continue Game -> Summary -> Move Details
//
//						//Continue Game -> Gym Leader
//						else if(gameState == MainGame.GYMBATTLE)
//						{
//							EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
//							gameState = MainGame.HOME;
//						} //end else if 
//
//						//Continue Game -> Trainer Card
//						else if(gameState == MainGame.TRAINERCARD)
//						{
//							EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
//							gameState = MainGame.HOME;
//						} //end else if 
//
//						//Continue Game -> Debug Options
//						else if(gameState == MainGame.DEBUG)
//						{
//							EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
//							gameState = MainGame.HOME;
//						} //end else if 
//						break;
//					} //end case OverallGame CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						//PC Home
//						if(pcState == PCGame.HOME)
//						{
//							StartCoroutine(LoadScene("MainGame", OverallGame.CONTINUE, true));
//						} //end if PC Home
//
//						//Pokemon Submenu on PC
//						else  if(pcState == PCGame.POKEMONSUBMENU)
//						{
//							choices.SetActive(false);
//							selection.SetActive(false);
//							pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//						} //end else if Pokemon Submenu on PC
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY)
//						{
//							choices.SetActive(false);
//							selection.SetActive(false);
//							StartCoroutine(PartyState(false));
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Summary on PC -> Summary
//						else if(pcState == PCGame.POKEMONSUMMARY)
//						{
//							//If on any page besides details
//							if(summaryChoice != 5)
//							{
//								//Deactivate summary
//								summaryScreen.SetActive(false);
//
//								//Return home or party
//								pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//							} //end if
//							else
//							{
//								summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
//								selection.SetActive(false);
//								summaryChoice = 4;
//							} //end else
//						} //end else if Pokemon Summary on PC -> Summary
//
//						//Move Switch on PC -> Summary -> Move Details
//						else if(pcState == PCGame.MOVESWITCH)
//						{
//							//Return to summary
//							currentMoveSlot.GetComponent<Image>().color = Color.clear;
//							pcState = PCGame.POKEMONSUMMARY;
//						} //end else if Move Switch on PC -> Summary -> Move Details
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS)
//						{
//							//Deactivate ribbons
//							ribbonScreen.SetActive(false);
//							selection.SetActive(false);
//							ribbonChoice = 0;
//							previousRibbonChoice = -1;
//
//							//Return to home or party
//							pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//
//						//Pokemon Markings on PC -> Submenu
//						else if(pcState == PCGame.POKEMONMARKINGS)
//						{
//							//Turn menu off
//							FillInChoices();
//							choices.SetActive(false);
//							selection.SetActive(false);
//
//							//Return to home or party
//							pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//						} //end else if Pokemon Markings on PC -> Submenu
//						break;
//					} //end case OverallGame PC
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
//				//Intro
//				case OverallGame.INTRO:
//					{
//						break;
//					} //end case OverallGame INTRO
//
//					//Menu
//				case OverallGame.MENU:
//					{
//						break;
//					} //end case OverallGame MENU
//
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						if (checkpoint == 11)
//						{
//							//Cycle through trainers
//							playerChoice++;
//							if (playerChoice > DataContents.trainerCardSprites.Length - 1)
//							{
//								playerChoice = 0;
//							} //end if
//						} //end if
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon Summary on Continue Game -> Summary
//						if(gameState == MainGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//Decrease (higher slots are lower childs)
//								choiceNumber--;
//
//								//Clamp between 1 and team size
//								if(choiceNumber < 1)
//								{
//									choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//								} //end if
//							} //end if
//						} //end else if Pokemon Summary on Continue Game -> Summary
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState == MainGame.POKEMONRIBBONS)
//						{
//							//Decrease (higher slots are lower childs)
//							choiceNumber--;
//
//							//Clamp between 1 and team size
//							if(choiceNumber < 1)
//							{
//								choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//							} //end if
//
//							//Reload ribbons
//							initialize = false;
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//						break;
//					} //end case OverallGame CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						//Pokemon Summary on PC -> Summary
//						if(pcState == PCGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//If party tab is open
//								if(partyTab.activeSelf && heldPokemon == null)
//								{
//									//Decrease (higher slots are lower childs)
//									choiceNumber--;
//
//									//Clamp between 1 and team size
//									if(choiceNumber < 1)
//									{
//										choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//									} //end if
//								} //end if
//								else if(!partyTab.activeSelf && heldPokemon == null)
//								{
//									//Decrease to previous pokemon slot
//									boxChoice = GameManager.instance.GetTrainer().GetPreviousPokemon(boxChoice);
//								} //end else if
//							} //end if
//						} //end if Pokemon Summary on PC -> Summary
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS)
//						{
//							//If party tab is open
//							if(partyTab.activeSelf && heldPokemon == null)
//							{
//								//Decrease (higher slots are lower childs)
//								choiceNumber--;
//
//								//Clamp between 1 and team size
//								if(choiceNumber < 1)
//								{
//									choiceNumber = GameManager.instance.GetTrainer().Team.Count;
//								} //end if
//
//								//Reload ribbons
//								initialize = false;
//							} //end if
//							//If in pokemon region
//							else if(!partyTab.activeSelf && heldPokemon == null)
//							{
//								//Decrease to previous pokemon slot
//								boxChoice = GameManager.instance.GetTrainer().GetPreviousPokemon(boxChoice);
//							} //end else if
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//						break;
//					} //end case OverallGame PC
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
//				//Intro
//				case OverallGame.INTRO:
//					{
//						break;
//					} //end case OverallGame INTRO
//
//					//Menu
//				case OverallGame.MENU:
//					{
//						break;
//					} //end case OverallGame MENU
//
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						if (checkpoint == 11)
//						{
//							//Cycle through trainers
//							playerChoice--;
//							if (playerChoice < 0)
//							{
//								playerChoice =DataContents.trainerCardSprites.Length - 1;
//							} //end if
//						} //end if
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon Summary on Continue Game -> Summary
//						if(gameState == MainGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//Increase (lower slots are on higher childs)
//								choiceNumber++;
//
//								//Clamp between 1 and team size
//								if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
//								{
//									choiceNumber = 1;
//								} //end if
//							} //end if
//						} //end else if Pokemon Summary on Continue Game -> Summary
//
//						//Pokemon Ribbons in Continue Game -> Ribbons
//						else if(gameState == MainGame.POKEMONRIBBONS)
//						{
//							//Increase (lower slots are on higher childs)
//							choiceNumber++;
//
//							//Clamp between 1 and team size
//							if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
//							{
//								choiceNumber = 1;
//							} //end if
//
//							//Reload ribbons
//							initialize = false;
//						} //end else if Pokemon Ribbons in Continue Game -> Ribbons
//						break;
//					} //end case OverallGame CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						//Pokemon Summary on PC -> Summary
//						if(pcState == PCGame.POKEMONSUMMARY)
//						{
//							//If on any page besides move details
//							if(summaryChoice != 5)
//							{
//								//Party tab is open
//								if(partyTab.activeSelf && heldPokemon == null)
//								{
//									//Increase (lower slots are on higher childs)
//									choiceNumber++;
//
//									//Clamp between 1 and team size
//									if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
//									{
//										choiceNumber = 1;
//									} //end if
//								} //end if
//								else if(!partyTab.activeSelf && heldPokemon == null)
//								{
//									//Increase to next pokemon slot
//									boxChoice = GameManager.instance.GetTrainer().GetNextPokemon(boxChoice);
//								} //end else if
//							} //end if
//						} //end if Pokemon Summary on PC -> Summary
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS)
//						{
//							//Party tab is open
//							if(partyTab.activeSelf && heldPokemon == null)
//							{
//								//Increase (lower slots are on higher childs)
//								choiceNumber++;
//
//								//Clamp between 1 and team size
//								if(choiceNumber > GameManager.instance.GetTrainer().Team.Count)
//								{
//									choiceNumber = 1;
//								} //end if
//							} //end if
//							else if(!partyTab.activeSelf && heldPokemon == null)
//							{
//								//Increase to next pokemon slot
//								boxChoice = GameManager.instance.GetTrainer().GetNextPokemon(boxChoice);
//							} //end else if
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//						break;
//					} //end case OverallGame PC
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
//				//Intro
//				case OverallGame.INTRO:
//					{
//						//If player cancels animation
//						if(playing && processing)
//						{
//							playing = false;
//						} //end if
//
//						//Fade out in prepareation for menu scene
//						else if(checkpoint == 3)
//						{
//							playing = true;
//							StartCoroutine(FadeOutAnimation(4));
//						} //end else if
//						break;
//					} //end case OverallGame INTRO
//
//					//Menu
//				case OverallGame.MENU:
//					{
//						if(checkpoint == 2)
//						{
//							selection.SetActive(false);
//							StartCoroutine(FadeOutAnimation(5));
//						} //end if
//						else if(checkpoint == 4)
//						{
//							selection.SetActive(false);
//							StartCoroutine(FadeOutAnimation(5));
//						} //end else if
//						break;
//					} //end case OverallGame MENU
//
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						if(checkpoint == 6 && inputText.text.Length != 0)
//						{
//							//Convert input name to player's name
//							playerName = inputText.text;
//							input.SetActive(false);
//							GameManager.instance.DisplayText("So your name is " + playerName + "?", false, true);
//							checkpoint = 7;
//						} //end if
//						else if(checkpoint == 8)
//						{
//							// Yes selected
//							if(choiceNumber == 0)
//							{
//								checkpoint = 9;
//							} //end if
//							// No selected
//							else if(choiceNumber == 1)
//							{
//								GameManager.instance.DisplayText("Ok let's try again. What is your name?", true);
//								checkpoint = 5;
//							} //end else if
//
//							//Disable choice and selection
//							selection.SetActive(false);
//							confirm.SetActive(false);
//						} //end else if
//						else if (checkpoint == 11)
//						{
//							prevTrainer.SetActive(false);
//							currTrainer.SetActive(false);
//							nextTrainer.SetActive(false);
//							checkpoint = 12;
//						} //end else if
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon submenu on Continue Game -> My Team is Open
//						if(gameState == MainGame.POKEMONSUBMENU)
//						{
//							//Apply appropriate action based on submenu selection
//							switch(subMenuChoice)
//							{
//								//Summary
//								case 0:
//									{
//										selection.SetActive(false);
//										choices.SetActive(false);
//										summaryScreen.SetActive(true);
//										summaryChoice = 0;
//										summaryScreen.transform.GetChild(0).gameObject.SetActive(false);
//										summaryScreen.transform.GetChild(1).gameObject.SetActive(false);
//										summaryScreen.transform.GetChild(2).gameObject.SetActive(false);
//										summaryScreen.transform.GetChild(3).gameObject.SetActive(false);
//										summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
//										summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
//										gameState = MainGame.POKEMONSUMMARY;
//										break;
//									} //end case 0 (Summary)
//
//									//Switch
//								case 1:
//									{
//										selection.SetActive(false);
//										choices.SetActive(false);
//										switchChoice = choiceNumber;
//										previousSwitchSlot = choiceNumber;
//										currentSwitchSlot = playerTeam.transform.FindChild("Pokemon"+choiceNumber).gameObject;
//										gameState = MainGame.POKEMONSWITCH;
//										break;
//									} //end case 1 (Switch)
//
//									//Item
//								case 2:
//									{
//										if(GameManager.instance.GetTrainer().Team[choiceNumber-1].Item == 0)
//										{
//											GameManager.instance.GetTrainer().Team[choiceNumber-1].Item=
//												GameManager.instance.RandomInt(1, 500);
//										}
//										else
//										{
//											GameManager.instance.GetTrainer().Team[choiceNumber-1].Item = 0;
//										} //end else
//										selection.SetActive(false);
//										choices.SetActive(false);
//										initialize = false;
//										gameState = MainGame.TEAM;
//										break;
//									} //end case 2 (Item)
//
//									//Ribbons
//								case 3:
//									{
//										initialize = false;
//										choices.SetActive(false);
//										selection.SetActive(false);
//										currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//											GetChild(0).gameObject;
//										gameState = MainGame.POKEMONRIBBONS;
//										break;
//									} //end case 3 (Ribbons)
//									//Cancel
//								case 4:
//									{
//										choices.SetActive(false);
//										selection.SetActive(false);
//										gameState = MainGame.TEAM;
//										break;
//									} //end case 4 (Cancel)
//							} //end switch
//						} //end if Pokemon submenu on Continue Game -> My Team is Open
//
//						//Pokemon Team on Continue Game -> My Team
//						else if(gameState == MainGame.TEAM)
//						{
//							//Open menu if not open, as long as player isn't selecting a button
//							if(choiceNumber > 0)
//							{
//								//Set submenu active
//								choices.SetActive(true);
//								selection.SetActive(true);
//
//								//Set up selection box at end of frame if it doesn't fit
//								if(selection.GetComponent<RectTransform>().sizeDelta != 
//									choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
//								{
//									selection.SetActive(false);
//									StartCoroutine("WaitForResize");
//								} //end if
//
//								//Reset position to top of menu
//								subMenuChoice = 0;
//								initialize = false;
//								gameState = MainGame.POKEMONSUBMENU;
//							} //end if
//							//Open PC if choice is PC
//							else if(choiceNumber == -1)
//							{
//								StartCoroutine(LoadScene("PC", SceneManager.OverallGame.PC, true));
//							} //end else if
//							//Return to Home if choice is Cancel
//							else if(choiceNumber == 0)
//							{
//								EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
//								gameState = MainGame.HOME;
//							} //end else if
//						} //end else if Pokemon Team on Continue Game -> My Team
//
//						//Pokemon Switch on Continue Game -> Switch
//						else if(gameState == MainGame.POKEMONSWITCH)
//						{
//							//If the choice equals the switch, don't run swap
//							if(choiceNumber != switchChoice)
//							{
//								//Switch function
//								GameManager.instance.GetTrainer().Swap(choiceNumber-1, switchChoice-1);
//							} //end if
//
//							//Go back to team
//							initialize = false;
//							playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
//							interactable = true;
//							playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
//							interactable = true;
//							gameState = MainGame.TEAM;
//						} //end else if Pokemon Switch on Continue Game -> Switch
//
//						//Pokemon Summary on Continue Game -> Summary
//						else if(gameState == MainGame.POKEMONSUMMARY)
//						{
//							//If on moves screen, switch to move details
//							if(summaryChoice == 4)
//							{
//								moveChoice = 0;
//								summaryChoice = 5;
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move1").gameObject;
//							} //end if
//
//							//If on move details screen, go to move switch
//							else if(summaryChoice == 5)
//							{
//								currentMoveSlot.GetComponent<Image>().color = Color.white;
//								switchChoice = moveChoice;
//								currentSwitchSlot = currentMoveSlot;
//								gameState = MainGame.MOVESWITCH;
//							} //end else if
//						} //end else if Pokemon Summary on Continue Game -> Summary
//
//						//Move Switch on Continue Game -> Summary -> Move Details
//						else if(gameState  == MainGame.MOVESWITCH)
//						{
//							//If switching spots aren't the same
//							if(moveChoice != switchChoice)
//							{
//								GameManager.instance.GetTrainer().Team[choiceNumber-1].SwitchMoves(moveChoice, switchChoice);
//							} //end if
//
//							currentMoveSlot.GetComponent<Image>().color = Color.clear;
//							gameState = MainGame.POKEMONSUMMARY;
//						} //end else if Move Switch on Continue Game -> Summary -> Move Details
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState  == MainGame.POKEMONRIBBONS)
//						{
//							//Make sure there are ribbons to be read
//							if(GameManager.instance.GetTrainer().Team[choiceNumber-1].GetRibbonCount() > 0)
//							{
//								selection.SetActive(!selection.activeSelf);
//								ReadRibbon();
//							} //end if
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//						break;
//					} //end case OverallGame CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						//PC Home
//						if(pcState == PCGame.HOME)
//						{
//							//Open submenu as long as player is in pokemon region or on title
//							if((boxChoice > -1 && boxChoice < 30 && (selectedPokemon != null || heldPokemon != null)) || boxChoice == -2)
//							{
//								//Set submenu active
//								choices.SetActive(true);
//								selection.SetActive(true);
//
//								//Set up selection box at end of frame if it doesn't fit
//								if(selection.GetComponent<RectTransform>().sizeDelta != 
//									choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
//								{
//									selection.SetActive(false);
//									StartCoroutine("WaitForResize");
//								} //end if
//
//								//Fill in choices
//								FillInChoices();
//
//								//Reset position to top of menu
//								subMenuChoice = 0;
//								initialize = false;
//								pcState = PCGame.POKEMONSUBMENU;
//							} //end if
//							//If on Party button
//							else if(boxChoice == 30)
//							{
//								StartCoroutine(PartyState(true));
//							} //end else if
//							//If on Return button
//							else if(boxChoice == 31)
//							{
//								StartCoroutine(LoadScene("MainGame", OverallGame.CONTINUE, true));
//							} //end else if
//						} //end if Home
//
//						//Pokemon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY)
//						{
//							//Open submenu as long as player is in party
//							if(choiceNumber > 0 && (selectedPokemon != null || heldPokemon != null))
//							{
//								//Set submenu active
//								choices.SetActive(true);
//								selection.SetActive(true);
//
//								//Set up selection box at end of frame if it doesn't fit
//								if(selection.GetComponent<RectTransform>().sizeDelta != 
//									choices.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta)
//								{
//									selection.SetActive(false);
//									StartCoroutine("WaitForResize");
//								} //end if
//
//								//Fill in choices
//								FillInChoices();
//
//								//Reset position to top of menu
//								subMenuChoice = 0;
//								initialize = false;
//								pcState = PCGame.POKEMONSUBMENU;
//							} //end if  
//							else
//							{
//								StartCoroutine(PartyState(false));
//							} //end else 
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Submenu on PC
//						else if(pcState == PCGame.POKEMONSUBMENU)
//						{
//							if(boxChoice != -2)
//							{
//								//Apply appropriate action based on submenu selection
//								switch(subMenuChoice)
//								{
//									//Move
//									case 0:
//										{
//											selection.SetActive(false);
//											choices.SetActive(false);
//											pcState = PCGame.POKEMONHELD;
//											break;
//										} //end case 0 (Move)
//
//										//Summary
//									case 1:
//										{
//											selection.SetActive(false);
//											choices.SetActive(false);
//											summaryScreen.SetActive(true);
//											summaryChoice = 0;
//											summaryScreen.transform.GetChild(0).gameObject.SetActive(false);
//											summaryScreen.transform.GetChild(1).gameObject.SetActive(false);
//											summaryScreen.transform.GetChild(2).gameObject.SetActive(false);
//											summaryScreen.transform.GetChild(3).gameObject.SetActive(false);
//											summaryScreen.transform.GetChild(4).gameObject.SetActive(false);
//											summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
//											pcState = PCGame.POKEMONSUMMARY;
//											break;
//										} //end case 1 (Summary)
//
//										//Item
//									case 2:
//										{
//											selection.SetActive(false);
//											choices.SetActive(false);
//											if(selectedPokemon.Item == 0)
//											{
//												selectedPokemon.Item = GameManager.instance.RandomInt(1, 500);
//											} //end if
//											else
//											{
//												selectedPokemon.Item = 0;
//											} //end else
//											pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//											break;
//										} //end case 2 (Item)
//
//										//Ribbons
//									case 3:
//										{
//											initialize = false;
//											choices.SetActive(false);
//											selection.SetActive(false);
//											currentRibbonSlot = ribbonScreen.transform.FindChild("RibbonRegion").
//												GetChild(0).gameObject;
//											pcState = PCGame.POKEMONRIBBONS;
//											break;
//										} //end case 3 (Ribbons)
//
//										//Markings
//									case 4:
//										{
//											markingChoices = heldPokemon == null ? selectedPokemon.GetMarkings().ToList() :
//												heldPokemon.GetMarkings().ToList();
//											initialize = false;
//											pcState = PCGame.POKEMONMARKINGS;
//											break;
//										} //end case 4 (Markings)
//
//										//Release
//									case 5:
//										{
//											//If party tab is open
//											if(partyTab.activeSelf && GameManager.instance.GetTrainer().Team.Count > 1)
//											{
//												//Get the pokemon
//												selectedPokemon = GameManager.instance.GetTrainer().Team[choiceNumber-1];
//
//												//Remove the pokemon from the party
//												GameManager.instance.GetTrainer().RemovePokemon(choiceNumber-1);
//
//												//Fill in party tab
//												for(int i = 1; i < GameManager.instance.GetTrainer().Team.Count + 1; i++)
//												{
//													partyTab.transform.FindChild("Pokemon"+i).GetComponent<Image>().sprite =
//														GetCorrectIcon(GameManager.instance.GetTrainer().Team[i-1]);
//												} //end for
//
//												//Deactivate any empty party spots
//												for(int i = 6; i > GameManager.instance.GetTrainer().Team.Count; i--)
//												{
//													partyTab.transform.FindChild("Pokemon" + (i)).gameObject.SetActive(false);
//												} //end for
//											} //end if
//											else
//											{
//												//Get the pokemon
//												selectedPokemon = GameManager.instance.GetTrainer().GetPC(
//													GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
//
//												//Remove the pokemon from the PC
//												GameManager.instance.GetTrainer().RemoveFromPC(
//													GameManager.instance.GetTrainer().GetPCBox(), boxChoice);
//
//												//Set PC slot to clear
//												boxBack.transform.FindChild("PokemonRegion").GetChild(boxChoice).
//												GetComponent<Image>().color = Color.clear;
//											} //end else
//
//											choices.SetActive(false);
//											selection.SetActive(false);
//											GameManager.instance.DisplayText("You released " + selectedPokemon.Nickname, true);
//											selectedPokemon = null;
//											pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//											break;
//										} //end case 5 (Release)
//
//										//Cancel
//									case 6:
//										{
//											choices.SetActive(false);
//											selection.SetActive(false);
//											pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//											break;
//										} //end case 6 (Cancel)
//								} //end switch
//							} //end if
//							else
//							{
//								//Apply appropriate action based on subMenuChoice
//								switch(subMenuChoice)
//								{
//									//Jump To
//									case 0:
//										{
//											input.transform.GetChild(2).GetComponent<Text>().text = "What box do you want to jump to? (0-49)";
//											input.transform.GetChild(0).GetComponent<Text>().text = "Desired box:";
//											inputText.text = GameManager.instance.GetTrainer().GetPCBox().ToString();
//											choices.SetActive(false);
//											selection.SetActive(false);
//											input.SetActive(true);
//											pcState = PCGame.INPUTSCREEN;
//											break;
//										} //end case 0
//										//Change Name
//									case 1:
//										{
//											input.transform.GetChild(2).GetComponent<Text>().text = "Please enter box name.";
//											input.transform.GetChild(0).GetComponent<Text>().text = "Box name:";
//											inputText.text = GameManager.instance.GetTrainer().GetPCBoxName();
//											choices.SetActive(false);
//											selection.SetActive(false);
//											input.SetActive(true);
//											pcState = PCGame.INPUTSCREEN;
//											break;
//										} //end case 1
//										//Change Wallpaper
//									case 2:
//										{
//											input.transform.GetChild(2).GetComponent<Text>().text = "Choose a wallpaper. (0-24)";
//											input.transform.GetChild(0).GetComponent<Text>().text = "Wallpaper:";
//											inputText.text = GameManager.instance.GetTrainer().GetPCBoxWallpaper().ToString();
//											choices.SetActive(false);
//											selection.SetActive(false);
//											input.SetActive(true);
//											pcState = PCGame.INPUTSCREEN;
//											break;
//										} //end case 2
//										//Cancel
//									case 3:
//										{
//											choices.SetActive(false);
//											selection.SetActive(false);
//											pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//											break;
//										} //end case 3
//								} //end switch
//							} //end else
//						} //end else if Pokemon Submenu on PC
//
//						//Pokemon Summary on PC -> Summary
//						else if(pcState == PCGame.POKEMONSUMMARY)
//						{
//							//If on moves screen, switch to move details
//							if(summaryChoice == 4)
//							{
//								moveChoice = 0;
//								summaryChoice = 5;
//								currentMoveSlot = summaryScreen.transform.GetChild(5).
//									FindChild("Move1").gameObject;
//							} //end if
//
//							//If on move details screen, go to move switch
//							else if(summaryChoice == 5)
//							{
//								currentMoveSlot.GetComponent<Image>().color = Color.white;
//								switchChoice = moveChoice;
//								currentSwitchSlot = currentMoveSlot;
//								pcState = PCGame.MOVESWITCH;
//							} //end else if
//						} //end else if Pokemon Summary on PC -> Summary
//
//						//Move Switch on PC -> Summary -> Move Details
//						else if(pcState == PCGame.MOVESWITCH)
//						{
//							//If switching spots aren't the same
//							if(moveChoice != switchChoice)
//							{
//								selectedPokemon.SwitchMoves(moveChoice, switchChoice);
//							} //end if
//
//							currentMoveSlot.GetComponent<Image>().color = Color.clear;
//							pcState = PCGame.POKEMONSUMMARY;
//						} //end else if Move Switch on PC -> Summary -> Move Details
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS)
//						{
//							//Make sure there are ribbons to be read
//							if(selectedPokemon.GetRibbonCount() > 0)
//							{
//								selection.SetActive(!selection.activeSelf);
//								ReadRibbon();
//							} //end if
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//
//						//Pokemon Markings on PC -> Submenu
//						else if(pcState == PCGame.POKEMONMARKINGS)
//						{
//							if(subMenuChoice < DataContents.markingCharacters.Length)
//							{
//								//Turn the marking on or off
//								markingChoices[subMenuChoice] = !markingChoices[subMenuChoice];
//
//								//Color in choices
//								for(int i = 0; i < markingChoices.Count; i++)
//								{
//									choices.transform.GetChild(i).GetComponent<Text>().color =
//										markingChoices[i] ? Color.black : Color.gray;
//								} //end for
//							} //end if
//							else if(subMenuChoice == DataContents.markingCharacters.Length)
//							{
//								//Turn menu off
//								FillInChoices();
//								choices.SetActive(false);
//								selection.SetActive(false);
//
//								//If holding a pokemon
//								if(heldPokemon != null)
//								{
//									//Update held pokemon markings
//									heldPokemon.SetMarkings(markingChoices.ToArray());
//
//									//Return to home or party
//									pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//								} //end if
//								//If in party
//								else if(partyTab.activeSelf)
//								{
//									//Update team pokemon markings
//									GameManager.instance.GetTrainer().Team[choiceNumber-1].SetMarkings(markingChoices.ToArray());
//
//									//Return to party
//									pcState = PCGame.PARTY;
//								} //end else if
//								//In pokemon region
//								else
//								{
//									//Update pc box pokemon markings
//									GameManager.instance.GetTrainer().GetPC(
//										GameManager.instance.GetTrainer().GetPCBox(),
//										boxChoice).SetMarkings(markingChoices.ToArray());
//
//									//Return to home
//									pcState = PCGame.HOME;
//								} //end else
//							} //end else if
//							else if(subMenuChoice == DataContents.markingCharacters.Length+1)
//							{
//								//Turn menu off
//								FillInChoices();
//								choices.SetActive(false);
//								selection.SetActive(false);
//
//								//Return to home or party
//								pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//							} //end else if
//						} //end else if Pokemon Markings on PC -> Submenu
//
//						//Pokemon Input on PC -> Input
//						else if(pcState == PCGame.INPUTSCREEN)
//						{
//							//Handle according to subMenuChoice
//							switch(subMenuChoice)
//							{
//								//Jump to
//								case 0:
//									{
//										if (inputText.text.Length > 0)
//										{
//											int outputHolder = 0;
//											if (int.TryParse(inputText.text, out outputHolder))
//											{
//												outputHolder = ExtensionMethods.WithinIntRange(outputHolder, 0, 49);
//												GameManager.instance.GetTrainer().ChangeBox(outputHolder);
//											} //end if
//										} //end if
//										break;
//									} //end case 0
//									//Change name
//								case 1:
//									{
//										if (inputText.text.Length > 0)
//										{
//											GameManager.instance.GetTrainer().SetPCBoxName(inputText.text);
//										} //end if
//										break;
//									} //end case 1
//									//Change wallpaper
//								case 2:
//									{
//										if (inputText.text.Length > 0)
//										{
//											int outputHolder = 0;
//											if (int.TryParse(inputText.text, out outputHolder))
//											{
//												outputHolder = ExtensionMethods.WithinIntRange(outputHolder, 0, 24);
//												GameManager.instance.GetTrainer().SetPCBoxWallpaper(outputHolder);
//											} //end if
//										} //end if
//										break;
//									} //end case 2
//							} //end switch
//
//							//Turn input off
//							input.SetActive(false);
//
//							//Return to home
//							checkpoint = 1;
//							pcState = PCGame.HOME;
//						} //end else if Pokemon Input on PC -> Input
//						break;
//					} //end OverallGame PC
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
//				//Intro
//				case OverallGame.INTRO:
//					{
//						break;
//					} //end case OverallGame INTRO
//
//					//Menu
//				case OverallGame.MENU:
//					{
//						break;
//					} //end case OverallGame MENU
//
//					//New Game
//				case OverallGame.NEWGAME:
//					{
//						break;
//					} //end case OverallGame NEWGAME
//
//					//Main Game scene
//				case OverallGame.CONTINUE:
//					{
//						//Pokemon Summary on Continue Game -> Summary
//						if(gameState == MainGame.POKEMONSUMMARY)
//						{
//							//If on any page besides details
//							if(summaryChoice != 5)
//							{
//								//Deactivate summary
//								summaryScreen.SetActive(false);
//
//								//Enable buttons again
//								playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
//								interactable = true;
//								playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
//								interactable = true;
//								gameState = MainGame.TEAM;
//							} //end if
//							else
//							{
//								summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
//								selection.SetActive(false);
//								summaryChoice = 4;
//							} //end else
//						} //end if Pokemon Summary on Continue Game -> Summary
//
//						//Pokemon submenu on Continue Game -> My Team
//						else if(gameState == MainGame.POKEMONSUBMENU)
//						{
//							//Deactivate submenu 
//							choices.SetActive(false);
//							selection.SetActive(false);                        
//
//							gameState = MainGame.TEAM;
//						} //end else if Pokemon submenu on Continue Game -> My Team
//
//						//Pokemon Ribbons on Continue Game -> Ribbons
//						else if(gameState == MainGame.POKEMONRIBBONS)
//						{
//							//Deactivate ribbons
//							ribbonScreen.SetActive(false);
//							selection.SetActive(false);
//							ribbonChoice = 0;
//							previousRibbonChoice = -1;
//
//							//Enable buttons again
//							playerTeam.transform.FindChild("Buttons").GetChild(0).GetComponent<Button>().
//							interactable = true;
//							playerTeam.transform.FindChild("Buttons").GetChild(1).GetComponent<Button>().
//							interactable = true;
//							gameState = MainGame.TEAM;
//						} //end else if Pokemon Ribbons on Continue Game -> Ribbons
//
//						//Continue Game -> My Team
//						else if(gameState == MainGame.TEAM)
//						{
//							EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
//							gameState = MainGame.HOME;
//						} //end else if Continue Game -> My Team
//
//						//Pokemon Switch on Continue Game -> Switch
//						else if(gameState == MainGame.POKEMONSWITCH)
//						{
//							//Return to team
//							gameState = MainGame.TEAM;
//						} //end else if Pokemon Switch on Continue Game -> My Team
//
//						//Move Switch on Continue Game -> Summary -> Move Details
//						else if(gameState  == MainGame.MOVESWITCH)
//						{
//							//Return to summary
//							currentMoveSlot.GetComponent<Image>().color = Color.clear;
//							gameState = MainGame.POKEMONSUMMARY;
//						} //end else if Move Switch on Continue Game -> Summary -> Move Details
//
//						//Continue Game -> Gym Leader
//						else if(gameState == MainGame.GYMBATTLE)
//						{
//							EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
//							gameState = MainGame.HOME;
//						} //end else if 
//
//						//Continue Game -> Trainer Card
//						else if(gameState == MainGame.TRAINERCARD)
//						{
//							EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
//							gameState = MainGame.HOME;
//						} //end else if 
//
//						//Continue Game -> Debug Options
//						else if(gameState == MainGame.DEBUG)
//						{
//							EventSystem.current.SetSelectedGameObject(buttonMenu.transform.GetChild(0).gameObject);
//							gameState = MainGame.HOME;
//						} //end else if 
//						break;
//					} //end case OverallGame CONTINUEGAME
//					//PC
//				case OverallGame.PC:
//					{
//						//PC Home
//						if(pcState == PCGame.HOME)
//						{
//							StartCoroutine(LoadScene("MainGame", OverallGame.CONTINUE, true));
//						} //end if PC Home
//
//						//Pokemon Submenu on PC
//						else if(pcState == PCGame.POKEMONSUBMENU)
//						{
//							choices.SetActive(false);
//							selection.SetActive(false);
//							pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//						} //end else if Pokemon Submenu on PC
//
//						//Pokeon Party on PC -> Party Tab
//						else if(pcState == PCGame.PARTY)
//						{
//							StartCoroutine(PartyState(false));
//						} //end else if Pokemon Party on PC -> Party Tab
//
//						//Pokemon Summary on PC -> Summary
//						else if(pcState == PCGame.POKEMONSUMMARY)
//						{
//							//If on any page besides details
//							if(summaryChoice != 5)
//							{
//								//Deactivate summary
//								summaryScreen.SetActive(false);
//
//								//Return to home or party
//								pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//							} //end if
//							else
//							{
//								summaryScreen.transform.GetChild(5).gameObject.SetActive(false);
//								selection.SetActive(false);
//								summaryChoice = 4;
//							} //end else
//						} //end else if Pokemon Summary on PC -> Summary
//
//						//Move Switch on PC -> Summary -> Move Details
//						else if(pcState == PCGame.MOVESWITCH)
//						{
//							//Return to summary
//							currentMoveSlot.GetComponent<Image>().color = Color.clear;
//							pcState = PCGame.POKEMONSUMMARY;
//						} //end else if Move Switch on PC -> Summary -> Move Details
//
//						//Pokemon Ribbons on PC -> Ribbons
//						else if(pcState == PCGame.POKEMONRIBBONS)
//						{
//							//Deactivate ribbons
//							ribbonScreen.SetActive(false);
//							selection.SetActive(false);
//							ribbonChoice = 0;
//							previousRibbonChoice = -1;
//
//							//Return to home or party
//							pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//						} //end else if Pokemon Ribbons on PC -> Ribbons
//
//						//Pokemon Markings on PC -> Submenu
//						else if(pcState == PCGame.POKEMONMARKINGS)
//						{
//							//Turn menu off
//							FillInChoices();
//							choices.SetActive(false);
//							selection.SetActive(false);
//
//							//Return to home or party
//							pcState = partyTab.activeSelf ? PCGame.PARTY : PCGame.HOME;
//						} //end else if Pokemon Markings on PC -> Submenu
//						break;
//					} //end case OverallGame PC
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
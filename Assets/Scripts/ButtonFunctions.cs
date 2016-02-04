using UnityEngine;
using System.Collections;

public class ButtonFunctions : MonoBehaviour 
{
	//Continue option from main screen
	public void Continue()
	{
		GameManager.instance.Continue ();
	} //end Continue

	//New Game option from main screen
	public void NewGame()
	{
		GameManager.instance.NewGame ();
	} //end NewGame

	//Options option from main screen
	public void Options()
	{
		GameManager.instance.Options ();
	} //end Options

    //Jump to pokedex number from game screen
    public void Jump()
    {
        GameManager.instance.Jump ();
    }
} //end ButtonFunctions class

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour {

	//Text variables
	string message;			//Complete text to output
	Text textComp;			//Text currently output
	GameObject arrow;		//Arrow to signal end of text
	bool displaying;		//If text is currently being output

	//Used for text initialization
	public void GetText(Text textArea, GameObject endArrow)
	{
		//Initialize text reference and gate
		textComp = textArea;
		displaying = false;

		//Disable arrow until text is finished
		arrow = endArrow;
		arrow.SetActive (false);
	} //end Start

	//Display text
	public bool PlayText(string textMessage)
	{
		//If already displaying text, end function
		if(displaying)
		{
			return false;
		} //end if

		//Setup message and begin typewriting
		message = textMessage;
		displaying = true;
		StartCoroutine(TypeText ());
		return true;
	} //end PlayText

	//Whether the text has finished displaying
	public bool GetDisplay()
	{
		return displaying;
	} //end GetDisplay

	//Types text a letter or more at a time
	IEnumerator TypeText () 
	{
		//Clear output and convert message to characters
		textComp.text = "";
		char[] letters = message.ToCharArray ();
		for (int i = 0; i < letters.Length; i++) 
		{
			//Display 1 letter at a time if FPS is high
			if(Time.deltaTime < 0.05f)
			{
				textComp.text += letters[i];		
			} //end if
			//Display 2 letters at a time if FPS is slower
			else if(Time.deltaTime < 0.1f)
			{
				textComp.text += letters[i];
				textComp.text += letters[i+1];
				i++;
			} //end else if
			//Display 3 letters at a time if FPS is terrible
			else if(Time.deltaTime <0.3f)
			{
				textComp.text += letters[i];
				textComp.text += letters[i+1];
				textComp.text += letters[i+2];
				i+=2;
			} //end else if
			//Display whole message if FPS is unbearable
			else
			{
				textComp.text = message;
				i = letters.Length;
			} //end else
			yield return null;
		} //end for

		//Finished displaying text
		displaying = false;
		arrow.SetActive (true);
	} //end TypeText
} //end SystemManager class

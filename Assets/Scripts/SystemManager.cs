using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SystemManager : MonoBehaviour {

	//Text variables
	string message;			//Complete text to output
	Text textComp;			//Text currently output

	//Used for text initialization
	public void GetText(Text textArea)
	{
		//Initialize message and get text
		textComp = textArea;
		message = textComp.text;

		//Clear output text and begin typewriting
		textComp.text = "";
		StartCoroutine(TypeText ());
	} //end Start

	//Types text a letter or more at a time
	IEnumerator TypeText () 
	{
		char[] letters = message.ToCharArray ();
		for (int i = 0; i < letters.Length; i++) 
		{
			if(Time.deltaTime < 0.05f)
			{
				textComp.text += letters[i];
			} //end if
			else if(Time.deltaTime < 0.1f)
			{
				textComp.text += letters[i];
				textComp.text += letters[i+1];
				i++;
			} //end else if
			else if(Time.deltaTime <0.3f)
			{
				textComp.text += letters[i];
				textComp.text += letters[i+1];
				textComp.text += letters[i+2];
				i+=2;
			} //end else if
			else
			{
				textComp.text = message;
				i = letters.Length;
			} //end else
			yield return null;
		} //end for
	} //end TypeText
} //end SystemManager class

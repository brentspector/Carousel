using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Test : MonoBehaviour {
	
	string message;
	Text textComp;
	
	// Use this for initialization
	void Start () {
		textComp = GetComponent<Text>();
		message = textComp.text;
		textComp.text = "";
		StartCoroutine(TypeText ());
	}
	
	IEnumerator TypeText () 
	{
		char[] letters = message.ToCharArray ();
		for (int i = 0; i < letters.Length; i++) 
		{
			textComp.text += letters[i];
			yield return null;
		}
	}
}


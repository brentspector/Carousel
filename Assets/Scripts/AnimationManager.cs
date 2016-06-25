/***************************************************************************************** 
 * File:    AnimationManager.cs
 * Summary: Handles generic animations in the game
 *****************************************************************************************/
#region Using
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#endregion

public class AnimationManager : MonoBehaviour
{
	#region Variables
	//Animation variables
	bool processing;				//Currently performing task

	//Scene tools variables
	Image fade;						//The fade screen
	GameObject openScene;			//Opens or closes scene between center and top/bottom
	#endregion

	#region Methods
	/***************************************
	 * Name: Awake
	 * Initalizes varaibles
	 ***************************************/
	void Awake()
	{
		processing = false;
		fade = GameManager.tools.transform.FindChild("Fade").GetComponent<Image>();
		openScene = GameManager.tools.transform.FindChild("SceneOpen").gameObject;
	} //end Awake

	/***************************************
     * Name: FadeInAnimation
     * Fades the blackout screen to show a 
     * "fade in" effect of scene
     ***************************************/
	public IEnumerator FadeInAnimation(int targetCheckpoint)
	{
		//Begin animation
		processing = true;

		//Initialize color values
		Color startColor = new Color (0, 0, 0, 1);
		Color endColor = new Color (0, 0, 0, 0);

		//Internal elapsed time
		float elapsedTime = 0f;

		//Set fade active
		fade.gameObject.SetActive (true);
		fade.color = startColor;

		//Lerp color for specified time
		while(fade.color.a != 0)
		{
			fade.color = Color.Lerp(startColor, endColor, 2 * elapsedTime);
			elapsedTime+= Time.deltaTime;
			yield return null;
		} //end while

		//Move to next checkpoint
		fade.gameObject.SetActive (false);

		//Move to target checkpoint
		GameManager.instance.ChangeCheckpoint(targetCheckpoint);

		//Animation is finished
		processing = false;
	} //end FadeInAnimation(int targetCheckpoint)

	/***************************************
     * Name: FadeOutAnimation
     * Fades blackout screen to create 
     * "fade out" effect of scene
     ***************************************/
	public IEnumerator FadeOutAnimation(string levelName)
	{
		//Begin processing
		processing = true;

		//Process at end of frame
		yield return new WaitForEndOfFrame();

		//Initialize color values
		Color startColor = new Color (0, 0, 0, 0);
		Color endColor = new Color (0, 0, 0, 1);

		//Internal elapsed time
		float elapsedTime = 0f;

		//Set fade active
		fade.gameObject.SetActive (true);
		fade.color = startColor;

		//Lerp color for specified time
		while(fade.color.a != 1)
		{
			fade.color = Color.Lerp(startColor, endColor, 2* elapsedTime);
			elapsedTime+= Time.deltaTime;
			yield return null;
		} //end while
			
		//Move to next scene, return last scene to checkpoint 0
		GameManager.instance.LoadScene(levelName);

		//Finished animation
		processing = false;
	} //end FadeOutAnimation(string levelName)

	/***************************************
     * Name: FadeObjectIn
     * Fades one or more objects in, then 
     * moves to the given checkpoint. Assumes 
     * objects already exist
     ***************************************/
	public IEnumerator FadeObjectIn(Image[] targetObject, int targetCheckpoint)
	{
		//Begin animation
		processing = true;

		//Initialize color value containers
		Color[] startColor = new Color[targetObject.Length];
		Color[] endColor = new Color[targetObject.Length];

		//Fill containers
		for(int i = 0; i < targetObject.Length; i++)
		{
			startColor[i] = targetObject[i].color;
			endColor[i] = new Color(startColor[i].r, startColor[i].g, startColor[i].b, 1);
		} //end foreach

		//Internal elapsed time
		float elapsedTime = 0f;

		//Lerp color for specified time
		while(targetObject[targetObject.Length-1].color.a != 1)
		{
			for(int i = 0; i < targetObject.Length; i++)
			{
				targetObject[i].color = Color.Lerp(startColor[i], endColor[i], 2* elapsedTime);
			} //end for
			elapsedTime+= Time.deltaTime;
			yield return null;
		} //end while

		//Move to target checkpoint
		GameManager.instance.ChangeCheckpoint(targetCheckpoint);

		//End fade animation
		processing = false;
	} //end FadeObjectIn(Image[] targetObject, int targetCheckpoint)

	/***************************************
     * Name: FadeObjectOut
     * Fades one or more objects out, then 
     * moves to the given checkpoint. 
     * Assumes objects already exist
     ***************************************/
	public IEnumerator FadeObjectOut(Image[] targetObject, int targetCheckpoint)
	{
		//Begin animation
		processing = true;

		//Initialize color value containers
		Color[] startColor = new Color[targetObject.Length];
		Color[] endColor = new Color[targetObject.Length];

		//Fill containers
		for(int i = 0; i < targetObject.Length; i++)
		{
			startColor[i] = targetObject[i].color;
			endColor[i] = new Color(startColor[i].r, startColor[i].g, startColor[i].b, 0);
		} //end foreach

		//Internal elapsed time
		float elapsedTime = 0f;

		//Lerp color for specified time
		while(targetObject[targetObject.Length-1].color.a != 0)
		{
			for(int i = 0; i < targetObject.Length; i++)
			{
				targetObject[i].color = Color.Lerp(startColor[i], endColor[i], 2* elapsedTime);
			} //end for
			elapsedTime+= Time.deltaTime;
			yield return null;
		} //end while

		//End fade animation
		processing = false;
	} //end FadeObjectOut(Image[] targetObject, int targetCheckpoint)

	/***************************************
     * Name: OpenScene
     * Scales top and bottom images to 0 to
     * create a "shutter open" effect
     ***************************************/
	public IEnumerator OpenScene(int targetCheckpoint)
	{
		//Begin animation
		processing = true;

		//Turn off fade, turn on openScene
		fade.gameObject.SetActive(false);
		openScene.SetActive(true);

		//Initialize starting position of objects
		openScene.transform.GetChild(0).GetComponent<RectTransform>().localScale = Vector3.one;
		openScene.transform.GetChild(1).GetComponent<RectTransform>().localScale = Vector3.one;

		//Internal elapsed time
		float elapsedTime = 0f;

		Vector3 endScale = new Vector3(1, 0, 0);

		//Lerp shutters for specified time
		while (openScene.transform.GetChild(0).GetComponent<RectTransform>().localScale.y > 0)
		{
			openScene.transform.GetChild(0).GetComponent<RectTransform>().localScale =
				Vector3.Lerp(Vector3.one, endScale, elapsedTime);
			openScene.transform.GetChild(1).GetComponent<RectTransform>().localScale =
				Vector3.Lerp(Vector3.one, endScale, elapsedTime);
			elapsedTime += Time.deltaTime;
			yield return null;
		} //end while

		//Move to target checkpoint
		GameManager.instance.ChangeCheckpoint(targetCheckpoint);

		//End scene open
		processing = false;
	} //end OpenScene(int targetCheckpoint)

	/***************************************
     * Name: ShowFoeParty
     * Plays animation for the foe's party to
     * appear
     ***************************************/
	public void ShowFoeParty()
	{
		GameObject.Find("FoePartyLineup").GetComponent<Animator>().SetTrigger("ShowParty");
	} //end ShowFoeParty

	/***************************************
     * Name: ShowPlayerParty
     * Plays animation for the player's party 
     * to appear
     ***************************************/
	public void ShowPlayerParty()
	{
		GameObject.Find("PlayerPartyLineup").GetComponent<Animator>().SetTrigger("ShowParty");
	} //end ShowPlayerParty

	/***************************************
     * Name: HideFoeParty
     * Plays animation for the foe's party to
     * disappear
     ***************************************/
	public void HideFoeParty()
	{
		GameObject.Find("FoePartyLineup").GetComponent<Animator>().SetTrigger("HideParty");
	} //end HideFoeParty

	/***************************************
     * Name: HidePlayerParty
     * Plays animation for the player's party 
     * to disappear
     ***************************************/
	public void HidePlayerParty()
	{
		GameObject.Find("PlayerPartyLineup").GetComponent<Animator>().SetTrigger("HideParty");
	} //end HidePlayerParty

	/***************************************
     * Name: ShowFoeBox
     * Plays animation for the foe's pokemon
     * status box to appear
     ***************************************/
	public void ShowFoeBox()
	{
		GameObject.Find("FoeBox").GetComponent<Animator>().SetTrigger("ShowBox");
	} //end ShowFoeBox

	/***************************************
     * Name: ShowPlayerBox
     * Plays animation for the player's 
     * pokemon box to appear
     ***************************************/
	public void ShowPlayerBox()
	{
		GameObject.Find("PlayerBox").GetComponent<Animator>().SetTrigger("ShowBox");
	} //end ShowPlayerBox

	/***************************************
     * Name: HideFoeBox
     * Plays animation for the foe's pokemon
     * status box to disappear
     ***************************************/
	public void HideFoeBox()
	{
		GameObject.Find("FoeBox").GetComponent<Animator>().SetTrigger("HideBox");
	} //end HideFoeBox

	/***************************************
     * Name: HidePlayerBox
     * Plays animation for the player's 
     * pokemon box to disappear
     ***************************************/
	public void HidePlayerBox()
	{
		GameObject.Find("PlayerBox").GetComponent<Animator>().SetTrigger("HideBox");
	} //end HidePlayerBox

	/***************************************
     * Name: Flash
     * Plays animation of a bright flash
     ***************************************/
	public void Flash()
	{
		GameObject.Find("Flash").GetComponent<Animator>().SetTrigger("Flash");
	} //end Flash

	/***************************************
	 * Name: IsProcessing
	 * Whether there is an animation in
	 * progress
	 ***************************************/
	public bool IsProcessing()
	{
		return processing;
	} //end IsProcessing
	#endregion
} //end class AnimationManager
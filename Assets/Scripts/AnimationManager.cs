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
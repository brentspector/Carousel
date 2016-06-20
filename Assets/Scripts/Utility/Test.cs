using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class Test : MonoBehaviour {

	public Image back;
	float lerper = 0f;

	public void Begin()
	{
		back.gameObject.SetActive(true);
		back.color = Color.white;
		lerper = 0f;
		StartCoroutine(Finished());
	}

	IEnumerator Finished()
	{
		while (back.color.a != 0)
		{
			back.color = new Color(back.color.r, back.color.g, back.color.b, Mathf.Lerp(1, 0, lerper));
			lerper += 0.15f;
			yield return null;
		}
	}
}


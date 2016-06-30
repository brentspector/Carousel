using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class Test : MonoBehaviour {

	List<int> theList;

	public void testFunction()
	{
		theList = new List<int>();
		theList.AddRange(Enumerable.Range(0,1000000));

		Stopwatch myStopwatch = new Stopwatch();
		myStopwatch.Start();
		for (int i = 0; i < 100000; i++)
		{
			ExtensionMethods.Swap(theList, 23521, 90731);
		} //end for
		myStopwatch.Stop();
		UnityEngine.Debug.Log(myStopwatch.ElapsedMilliseconds);
	} //end

}


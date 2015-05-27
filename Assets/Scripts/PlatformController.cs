using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {
	
	public int numInContact = 0;
	
	
	public int GetNumInContact()
	{
		return numInContact;
	}
	
	public void IncreaseNumInContact()
	{
		numInContact += 1;
	}
	
	public void DecreaseNumInContact()
	{
		numInContact -= 1;
		
		if (numInContact < 0)
			numInContact = 0;
	}
}

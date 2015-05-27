using UnityEngine;
using System.Collections;

/**
 * Controller class to keep track of the global score as
 * well as draw the score to the interface.
 */
public class ScoreController : MonoBehaviour {
	// Array indexes are matched by the PlayerIndex
	
	public int score = 0;
	
	void Update()
	{
		guiText.text = "Score: " + score;
	}
	
	public void IncreaseScore(int increaseBy)
	{
		score = score + increaseBy;
	}

	public void ResetScore(){
		score = 0;
	}
}

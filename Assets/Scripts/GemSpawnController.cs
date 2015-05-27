using UnityEngine;
using System.Collections;

public class GemSpawnController : MonoBehaviour 
{
	
	// Spawners containing the locations where the gems are to be instantiated.
	public GameObject[] gemSpawners;
	
	// Gem prefabs to be instantiated.
	public Transform redGem;
	public Transform blueGem;

	public LoggerScript logScript;
	
	int[] redSpawnOrderRound0 = {0, 8, 11, 4, 14, 7, 12, 2, 16, 5};
	int[] blueSpawnOrderRound0 = {5, 9, 16, 1, 13, 10, 15, 3, 11, 0};
	
	int[] redSpawnOrderRound1 = {0, 8, 11, 4, 14, 7, 12, 2, 16, 5};
	int[] blueSpawnOrderRound1 = {5, 9, 16, 1, 13, 10, 15, 3, 11, 0};
	
	int[] redSpawnOrderRound2 = {0, 8, 11, 4, 14, 7, 12, 2, 16, 5};
	int[] blueSpawnOrderRound2 = {5, 9, 16, 1, 13, 10, 15, 3, 11, 0};
	
	int[] redSpawnOrder;
	int[] blueSpawnOrder;
	
	int redSpawnIndex = 0;
	int blueSpawnIndex = 0;
	
	int lastRedSpawner = 0;
	int lastBlueSpawner = 0;
	
	// Use this for initialization
	void Start () 
	{
		NewRound ();
	}
	
	public void SpawnNewGem(int gemIndex)
	{
		//Debug.Log ("Index for gem to spawn: " + gemIndex);
		
		if (gemIndex == 0)
		{
			//Debug.Log("Spawning Red Gem");
			
			redSpawnIndex = (redSpawnIndex + 1) % redSpawnOrder.Length;
			lastRedSpawner = redSpawnOrder[redSpawnIndex];
			
			if (lastRedSpawner == lastBlueSpawner)
			{
				redSpawnIndex = (redSpawnIndex + 1) % redSpawnOrder.Length;
				lastRedSpawner = redSpawnOrder[redSpawnIndex];
				
				//Debug.Log("Trying to spawn red gem in blue's location - choosing another spawner");
			}
			
			// Use red spawner
			Instantiate(redGem, gemSpawners[lastRedSpawner].transform.position, Quaternion.identity);

			LogRedPos ();
			LogBluePos ();
		}
		else
		{
			//Debug.Log ("Spawning Blue Gem");
			
			blueSpawnIndex = (blueSpawnIndex + 1) % blueSpawnOrder.Length;
			lastBlueSpawner = blueSpawnOrder[blueSpawnIndex];
			
			if (lastBlueSpawner == lastRedSpawner)
			{
				blueSpawnIndex = (blueSpawnIndex + 1) % blueSpawnOrder.Length;
				lastBlueSpawner = blueSpawnOrder[blueSpawnIndex];
				
				//Debug.LogError("Trying to spawn blue gem in red's location - choosing another spawner");
			}
			
			// Use blue spawner
			Instantiate (blueGem, gemSpawners[lastBlueSpawner].transform.position, Quaternion.identity);

			LogRedPos ();
			LogBluePos ();
		}
	}

	public void NewRound(){
		redSpawnIndex = 0;
		blueSpawnIndex = 0;
		
		lastRedSpawner = 0;
		lastBlueSpawner = 0;

		GameObject bGem = GameObject.Find ("blue_gem(Clone)");
		if (bGem != null) {
			DestroyObject(bGem);
		}

		GameObject rGem = GameObject.Find ("red_gem(Clone)");
		if (rGem != null) {
			DestroyObject(rGem);
		}

		if (GameState.currRound == 0)
		{
			redSpawnOrder = redSpawnOrderRound0;
			blueSpawnOrder = blueSpawnOrderRound0;
		}
		else if (GameState.currRound == 1)
		{
			redSpawnOrder = redSpawnOrderRound1;
			blueSpawnOrder = blueSpawnOrderRound1;
		}
		else
		{
			redSpawnOrder = redSpawnOrderRound2;
			blueSpawnOrder = blueSpawnOrderRound2;
		}
		
		lastRedSpawner = redSpawnOrder[redSpawnIndex];
		lastBlueSpawner = blueSpawnOrder[blueSpawnIndex];
		
		// Starting gems.
		Instantiate(redGem, gemSpawners[lastRedSpawner].transform.position, Quaternion.identity);
		Instantiate(blueGem, gemSpawners[lastBlueSpawner].transform.position, Quaternion.identity);

		LogRedPos ();
		LogBluePos ();
	}

	public void LogBluePos(){
		Vector3 gPos = gemSpawners[lastBlueSpawner].transform.position;
		int x = (int)gPos.x + 10;
		int y = (int)((gPos.y + 4) * 2);
		logScript.SetGem2Pos(x, y);
	}

	public void LogRedPos(){
		Vector3 gPos = gemSpawners[lastRedSpawner].transform.position;
		int x = (int)gPos.x + 10;
		int y = (int)((gPos.y + 4) * 2);
		logScript.SetGem1Pos(x, y);
	}
}

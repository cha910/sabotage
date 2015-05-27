using UnityEngine;
using System.Collections;

public class GemController : MonoBehaviour {
	
	public LoggerScript logScript;
	
	// The player to which this gem belongs.
	public int playerIndex; // = 0;
	
	// How much this gem is worth.
	public int score = 1;

	bool collide = false;

	public AudioSource collect;
	public AudioSource steal;
	
	Transform player;
	ScoreController playerScore;
	ScoreController shadow;

	PowerupController powerups;

	void Awake(){
		logScript = GameObject.Find ("Logger").GetComponent<LoggerScript> ();
		collect = GameObject.Find ("Collect").GetComponent<AudioSource> ();
		steal = GameObject.Find ("Steal").GetComponent<AudioSource> ();
	}
	void Update()
	{
		if(!player){
			if (playerIndex == 0)
			{
				if(GameObject.Find("Player One(Clone)")!= null){
					player = GameObject.Find ("Player One(Clone)").transform;
				}
				playerScore = GameObject.Find ("Player One Score").GetComponent<ScoreController>();
				shadow = GameObject.Find ("Player One Score Shadow").GetComponent<ScoreController>();
				powerups = GameObject.Find("Powerups One").GetComponent<PowerupController>();
			}
			else
			{
				if(GameObject.Find("Player Two(Clone)")!= null){
					player = GameObject.Find ("Player Two(Clone)").transform;
				}
				playerScore = GameObject.Find ("Player Two Score").GetComponent<ScoreController>();
				shadow = GameObject.Find ("Player Two Score Shadow").GetComponent<ScoreController>();
				powerups = GameObject.Find("Powerups Two").GetComponent<PowerupController>();
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (!collide) {
			collide = true;
			if (other.gameObject.transform == player || (other.CompareTag ("GroundCheck") && other.transform.parent.gameObject.transform == player)) {
				// Play a good sound effect.
				collect.audio.Play ();
				// Give the player a point.
				playerScore.IncreaseScore (score);
				shadow.IncreaseScore (score);

				if(playerIndex == 0){
					logScript.SendMessage("SetScore1",playerScore.score);

					//P1 Collect
					if(PhotonNetwork.isMasterClient){
						StartCoroutine(logScript.EventLog(0, 4));
					}
				}else{
					logScript.SendMessage("SetScore2",playerScore.score);

					//P2 Collect
					if(PhotonNetwork.isMasterClient){
						StartCoroutine(logScript.EventLog(0, 5));
					}
				}
				// Notify the Gem Spawn Controller that this player
				// needs another gem.
				SpawnGem ();

				//logScript.SendMessage("EventLog");
			} else {
				// Play a bad sound effect.
				steal.audio.Play ();
				// Notify the Gem Spawn Controller that this player
				// needs another gem.
				Invoke ("SpawnGem", 2);
				transform.root.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
				transform.root.gameObject.GetComponent<BoxCollider2D> ().enabled = false;

				if(playerIndex == 0){
					//P2 Steal
					if(PhotonNetwork.isMasterClient){
						StartCoroutine(logScript.EventLog(0, 7));
					}
				}else{
					//P1 Steal
					if(PhotonNetwork.isMasterClient){
						StartCoroutine(logScript.EventLog(0, 6));
					}
				}

				//logScript.SendMessage("EventLog");
			}
		}
	}
	
	public void IncreaseGemMultiplier()
	{
		// Increase score.
		score = 2;
		
		// Increase gem size.
		transform.localScale = new Vector3(transform.localScale.x * 2.0f, transform.localScale.y * 2.0f, transform.localScale.z);

		powerups.SendMessage ("AssistUsed", true);
	}

	public void SpawnGem(){
		GameObject.Find ("Spawner").GetComponent<GemSpawnController>().SpawnNewGem(playerIndex);
		
		// Make the gem disappear.
		Destroy(transform.root.gameObject);
	}
}

using UnityEngine;
using System.Collections;

public class PowerupController : MonoBehaviour {

	public GameObject player;
	public GameObject opponent;

	public PowerupController opponentPowerup;

	public AudioSource sabotageAlert;
	public AudioSource assistAlert;
	
	public int playerIndex = 0;

	public GameObject sabotage;
	public GameObject assist;
	
	public GameObject powerupBar;
	public float powerupCooldownInSeconds = 12.0f;
	
	private float barSectionScale;
	
	private GameState game;

	private float elapsedTime = 0.0f;
	
	private bool sabotageActive = false;
	private bool assistActive = false;

	public LoggerScript logScript;
	
	// Use this for initialization
	void Start()
	{
		game = GameObject.Find("GameState").GetComponent<GameState>();

		if (playerIndex == 0) {
			opponentPowerup = GameObject.Find ("Powerups Two").GetComponent<PowerupController> ();
		} else {
			opponentPowerup = GameObject.Find ("Powerups One").GetComponent<PowerupController> ();
		}
		
		NewRound ();
	}
	
	void Update()
	{
		if (!player) {
			if(playerIndex == 0){
				player = GameObject.Find("Player One(Clone)");
			}else{
				player = GameObject.Find("Player Two(Clone)");
			}
		}
		if (!opponent) {
			if(playerIndex == 0){
				opponent = GameObject.Find("Player Two(Clone)");
			}else{
				opponent = GameObject.Find("Player One(Clone)");
			}
		}
		// Update time elapsed.
		if (! game.IsPaused()){
			elapsedTime += Time.smoothDeltaTime;
			
			if (elapsedTime >= powerupCooldownInSeconds){
				// Activate the powerups for this player.
				if(!assistActive){
					SetPowerupsActive();
				}
			}else{
				float newScale = elapsedTime / powerupCooldownInSeconds;
				powerupBar.transform.localScale = new Vector3(newScale, powerupBar.transform.localScale.y, powerupBar.transform.localScale.z);
			}
		}
	}

	public void Assist(){
		if(assistActive){
			// They pressed the button to use the assistive powerup.
			//Debug.Log ("ASSIST POWERUP USED");
			assistAlert.audio.Play();
			// Apply the powerup effect.
			//GameObject.Find(gemId).GetComponent<GemController>().IncreaseGemMultiplier();

			player.GetComponent<PlayerController>().Fly();
			sabotage.SetActive(false);
			sabotageActive = false;

			assist.SetActive(false);
			assistActive = false;

			SetPowerupsInactive();

			//P1 Assist
			if(PhotonNetwork.isMasterClient){
				if(playerIndex == 0){
					logScript.P1Assist();
					StartCoroutine(logScript.EventLog(0, 2));
				}else{
					logScript.P2Assist();
					StartCoroutine(logScript.EventLog(0, 3));
				}
			}
		}
	}

	public void Sabotage(){
		if (sabotageActive) {
			// They pressed the button to use the sabotage powerup.
			//Debug.Log ("SABOTAGE POWERUP USED");
			sabotageAlert.audio.Play();
			
			// Apply the powerup effect.
			opponent.GetComponent<PlayerController>().Stun();
			//player.GetPhotonView().RPC("Stun", PhotonTargets.Others);
			SetPowerupsInactive();

			//P1 Sabotage
			if(PhotonNetwork.isMasterClient){
				logScript.P1Sabotage();
				StartCoroutine(logScript.EventLog(0, 0));
				
			}
		}
	}

	public void SetPowerupsInactive(){
		sabotage.SetActive(false);
		sabotageActive = false;
		
		assist.SetActive(false);
		assistActive = false;
		
		elapsedTime = 0;
	}

	public void SetPowerupsActive(){
		sabotage.SetActive(true);
		sabotageActive = true;
		
		assist.SetActive(true);
		assistActive = true;

		if(PhotonNetwork.isMasterClient){
			if(playerIndex == 0){
				//P1 powerup bar fill
				StartCoroutine(logScript.EventLog(0, 8));
			}else{
				//P2 powerup bar fill
				StartCoroutine(logScript.EventLog(0, 9));
			}
		}
	}

	public void OpponentSabotage(){
		if (opponentPowerup.sabotageActive) {
			// They pressed the button to use the sabotage powerup.
			//Debug.Log ("Opponent SABOTAGE POWERUP USED");
			sabotageAlert.audio.Play();
			player.GetComponent<PlayerController>().Stun();
			opponentPowerup.SendMessage("SetPowerupsInactive");

			//P2 Sabotage
			if(PhotonNetwork.isMasterClient){
				logScript.P2Sabotage();
				StartCoroutine(logScript.EventLog(0, 1));
			}
		}
	}

	public void OpponentAssist(){
		if (opponentPowerup.assistActive) {
			// They pressed the button to use the assist powerup.
			//Debug.Log ("Opponent ASSIST POWERUP USED");
			assistAlert.audio.Play();
			opponent.GetComponent<PlayerController>().Fly();
			opponentPowerup.SendMessage("SetPowerupsInactive");

//			//P2 Assist
//			if(PhotonNetwork.isMasterClient){
//				logScript.P2Assist();
//				logScript.EventLog(0, 3);
//			}
		}
	}

	public void NewRound(){
		assist.SetActive(false);
		assistActive = false;
		sabotage.SetActive(false);
		sabotageActive = false;
		powerupCooldownInSeconds = 12.0f;
		elapsedTime = 0.0f;
		powerupBar.transform.localScale = new Vector3(0f, powerupBar.transform.localScale.y, powerupBar.transform.localScale.z);
	}
}



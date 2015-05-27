using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {
	
	//private static string START_MSG = "Press Enter \n to start!";
	
	public int rounds = 3;
	
	public GameObject playerOneScore;
	public GameObject playerTwoScore;
	
	public int pointsToWin = 10;
	
	public GameObject sign;
	public GameObject text;
	public GameObject playerLabel;
	public GameObject controls;
	public GameObject instructions;
	
	private bool paused = true;
	
	private bool countdownStarted = false;
	private bool instructionsShown = false;

	private bool logStart = false;
	private bool logEnd = false;
	
	private float countdownInSeconds = 4;
	
	private bool roundEnded = false;
	private int winnerPlayerIndex = -1;
	private string winnerMessage = "";
	
	int numPlayers;
	
	PhotonView pv;

	public LoggerScript logScript;
	
	/**
	 * For Logging Purposes.
	 */ 
	static public int currRound = 0;
	
	
	// Use this for initialization
	void Start () 
	{
		//Debug.Log("Initializing Round " + currRound);
		logScript.SetRound (currRound);
		
		paused = true;	
		
		pv = GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
		numPlayers = 0; 
		if (PhotonNetwork.room != null) {
			numPlayers = PhotonNetwork.room.playerCount;
		}
		if (numPlayers > 1) {
			if (currRound < rounds) {
				if (paused) {
					if(!instructionsShown){
						instructions.SetActive(true);
						text.guiText.enabled = false;
						if (Input.GetKeyDown ("return")) {
							//countdownStarted = true;
							pv.RPC ("HideInstructions", PhotonTargets.All);
						}
					}else if (! countdownStarted) {
						// Initially display the "Press A to Start" text.
						//text.guiText.fontSize = 80;
						controls.SetActive(true);
						if(PhotonNetwork.isMasterClient){
							playerLabel.guiText.text = "You Are Player One";
						}else{
							playerLabel.guiText.text = "You Are Player Two";
						}
						text.guiText.enabled = false;
						
						if (Input.GetKeyDown ("return")) {
							//countdownStarted = true;
							pv.RPC ("StartGame", PhotonTargets.All);
						}
					} else {
						text.guiText.enabled = true;
						countdownInSeconds = countdownInSeconds - Time.smoothDeltaTime;
						
						if (countdownInSeconds <= 0) {
							paused = false;
							
							countdownInSeconds = 4;
							
							text.guiText.enabled = false;
							sign.SetActive (false);
						} else if (countdownInSeconds < 1) {
							text.guiText.fontSize = 80;
							text.guiText.text = "BEGIN!";
							if(!logStart){
								logStart = true;
								logScript.SetRoundStart();
								if(PhotonNetwork.isMasterClient){
									StartCoroutine(logScript.EventLog(0, 10));
								}
							}
						} else {
							text.guiText.fontSize = 80;
							text.guiText.text = "" + (int)countdownInSeconds;
						}
					}
					
					if (roundEnded) {
						sign.gameObject.SetActive (true);
						text.guiText.fontSize = 48;
						text.guiText.text = winnerMessage + "\n Press enter \nto continue." ;
						text.guiText.enabled = true;

						if(!logEnd){
							logEnd = true;
							if(PhotonNetwork.isMasterClient){
								StartCoroutine(logScript.EventLog(0, 11));
								StartCoroutine(logScript.EventLog(1, 0));
							}
						}

						if (Input.GetKeyDown ("return")) {
							pv.RPC("NextRound", PhotonTargets.All);
							//print("Next");
						}
					}
				} else {
					// Check the scores to see if we have a winner yet.
					int p1Score = playerOneScore.GetComponent<ScoreController> ().score;
					int p2Score = playerTwoScore.GetComponent<ScoreController> ().score;
					
					if (p1Score >= pointsToWin && p2Score >= pointsToWin) {
						// It was a tie
						EndRound (2, "It was \n a tie!");
					} else if (p1Score >= pointsToWin) {
						// Player one wins
						EndRound (0, "Player One \n wins!");
					} else if (p2Score >= pointsToWin) {
						// Player two wins
						EndRound (1, "Player Two \n wins!");
					} else {
						// still playing
					}
				}
			} else {
				// Game is over. Switch to thank you screen.
				Application.LoadLevel ("End");
			}
		} else {
			if(countdownStarted || currRound != 0){
				ResetRound();
				currRound = 0;

				paused = true;
				countdownStarted = false;
				countdownInSeconds = 4;
				roundEnded = false;
			}
			sign.gameObject.SetActive (true);
			controls.SetActive(false);
			text.guiText.fontSize = 60;
			text.guiText.text = "Waiting for \n players.";
			text.guiText.enabled = true;
		}
	}
	
	public void EndRound(int winnerPlayerIndex, string winnerMessage){
		paused = true;
		roundEnded = true;
		
		this.winnerPlayerIndex = winnerPlayerIndex;
		this.winnerMessage = winnerMessage;

		if (winnerPlayerIndex == 0) {
			logScript.SetWinner(1);
		} else {
			logScript.SetWinner(2);
		}
	}
	
	public bool IsPaused(){
		return paused;
	}
	
	public void Pause(){
		controls.SetActive(true);
		paused = true;
	}
	
	public void Resume(){
		controls.SetActive(false);
		paused = false;
	}

	[RPC]
	public void HideInstructions(){
		instructionsShown = true;
		instructions.SetActive(false);
	}
	
	[RPC]
	public void StartGame(){
		controls.SetActive(false);
		countdownStarted = true;
		//print("StartGame");
	}
	
	[RPC]
	public void NextRound(){
		//reset countdown
		//print("Next Round");
		countdownInSeconds = 4;
		roundEnded = false;
		currRound++;
		paused = true;
		countdownStarted = false;
		ResetRound ();
	}

	public void ResetRound(){
		logEnd = false;
		logStart = false;

		logScript.SetRound (currRound);

		//reset powerup bars
		PowerupController powerup1 = GameObject.Find ("Powerups One").GetComponent<PowerupController>();
		PowerupController powerup2 = GameObject.Find ("Powerups Two").GetComponent<PowerupController>();
		powerup1.NewRound ();
		powerup2.NewRound ();
		
		//reset scores
		ScoreController p1Score = playerOneScore.GetComponent<ScoreController>();
		ScoreController p2Score = playerTwoScore.GetComponent<ScoreController>();
		ScoreController shadow1 = GameObject.Find ("Player One Score Shadow").GetComponent<ScoreController>();
		ScoreController shadow2 = GameObject.Find ("Player Two Score Shadow").GetComponent<ScoreController>();
		p1Score.ResetScore();
		p2Score.ResetScore();
		shadow1.ResetScore ();
		shadow2.ResetScore ();
		
		//reset players
		Transform p1Spawn = GameObject.Find ("P1Spawn").transform;
		Transform p2Spawn = GameObject.Find ("P2Spawn").transform;
		GameObject p1 = GameObject.Find ("Player One(Clone)");
		GameObject p2 = GameObject.Find ("Player Two(Clone)");
		if (p1 != null) {
			p1.transform.position = p1Spawn.position;
			p1.transform.rotation = p1Spawn.rotation;
			p1.GetComponent<PlayerController>().NewRound();
		}
		if (p2 != null) {
			p2.transform.position = p2Spawn.position;
			p2.transform.rotation = p2Spawn.rotation;
			p2.GetComponent<PlayerController>().NewRound();
		}

		//reset spawner
		GemSpawnController spawner = GameObject.Find ("Spawner").GetComponent<GemSpawnController>();
		spawner.NewRound ();

		//log round start
	}
}

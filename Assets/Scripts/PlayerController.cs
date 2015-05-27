using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public LoggerScript logScript;
		
	public int playerIndex = 0;
	public float stunDuration = 3.0f;
	public float flyDuration = 3.0f;

	public PowerupController powerup;
	public PowerupController opponentPowerup;

	public PlayerController opponentController;
	
	public float jumpForce = 240.0f;
	public float horizForce = 120.0f;
	
	public float speed = 1.0f;
	public float climbSpeed = 0.5f;
	public float facing = 1.0f;
	
	private GameState game;
	
	public bool facingRight = true;
	public bool jump = false;
	
	private Transform groundCheck;
	
	// The platform that we are in contact with (climbing up/down towards).
	//public GameObject platform;
	
	public bool grounded;
	
	public bool aboveLadder;
	public bool bottomLadder;
	public bool climbing;
	
	public bool stunned;
	public bool flying;
	
	private Animator anim;
	
	private float lastX;
	private float lastY;
	private float lastZ;
	
	//private bool prevMove = false;
	private Vector3 prevPosition;
	
	Vector3 bottomLeftWorldCoordinates;
	Vector3 topRightWorldCoordinates;
	Vector3 movementRangeMin;
	Vector3 movementRangeMax;

	PhotonView pv;
	
	// Use this for initialization
	void Awake () 
	{
		logScript = GameObject.Find ("Logger").GetComponent<LoggerScript> ();
		game = GameObject.Find("GameState").GetComponent<GameState>();
		
		jump = false;
		
		bottomLeftWorldCoordinates = Camera.main.ViewportToWorldPoint(Vector3.zero);
		topRightWorldCoordinates = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));
		
		movementRangeMin = bottomLeftWorldCoordinates + transform.renderer.bounds.extents;
		movementRangeMax = topRightWorldCoordinates - transform.renderer.bounds.extents;
		movementRangeMin.x = -10.0f;
		movementRangeMax.x = 10.0f;

		anim = GetComponent<Animator>();
		pv = GetComponent<PhotonView>();

		if (playerIndex == 0) {
			powerup = GameObject.Find ("Powerups One").GetComponent<PowerupController> ();
			opponentPowerup = GameObject.Find ("Powerups Two").GetComponent<PowerupController> ();
		} else {
			powerup = GameObject.Find ("Powerups Two").GetComponent<PowerupController> ();
			opponentPowerup = GameObject.Find ("Powerups One").GetComponent<PowerupController> ();
		}
		NewRound ();
	}
	
	// Update is called once per frame
	void Update () {
		if (opponentController == null) {
			GameObject p1 = GameObject.Find ("Player One(Clone)");
			GameObject p2 = GameObject.Find ("Player Two(Clone)");
			if (playerIndex == 0) {
				if(p2 != null){
					opponentController = p2.GetComponent<PlayerController> ();
				}
			}else{
				if(p1 != null){
					opponentController = p1.GetComponent<PlayerController> ();
				}
			}
		}

		if (flying) {
			SpriteRenderer sp = GetComponent<SpriteRenderer> ();
			sp.color = new Color(1, 1, 1, 0.4f);
		} else {
			SpriteRenderer sp = GetComponent<SpriteRenderer> ();
			sp.color = new Color(1, 1, 1, 1f);
		}

		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player One"), LayerMask.NameToLayer("Player Two"), true);

		if (! game.IsPaused()){
			if (! grounded){
				transform.rigidbody2D.isKinematic = false;
			}
				/*else
				{
					transform.rigidbody2D.isKinematic = true;
				}*/
				
			if (! stunned){
				//if (Input.GetAxis (horizontalAxis) < 0){
				if(Input.GetKeyDown(KeyCode.LeftArrow)){
					// Moving Left
					if(pv.isMine){
						MoveLeft();
					}
					pv.RPC("OpponentLeft", PhotonTargets.Others);
				//}else if (Input.GetAxis (horizontalAxis) > 0){
				}else if(Input.GetKeyDown(KeyCode.RightArrow)){
					// Moving Right	
					if(pv.isMine){
						MoveRight();
					}
					pv.RPC("OpponentRight", PhotonTargets.Others);
				//}else if (Input.GetAxis(verticalAxis) > 0.0f){
				}else if(Input.GetKeyDown(KeyCode.UpArrow)){
					//Moving Up
					if(pv.isMine){
						MoveUp();
					}
					pv.RPC("OpponentUp", PhotonTargets.Others);
				//}else if (Input.GetAxis(verticalAxis) < 0.0f){
				}else if(Input.GetKeyDown(KeyCode.DownArrow)){
					//Moving Down
					if(pv.isMine){
						MoveDown();
					}
					pv.RPC("OpponentDown", PhotonTargets.Others);
				}
			}else{
				// Player is stunned so do not accept in input from the player.
				
				stunDuration -= Time.smoothDeltaTime;
				
				if (stunDuration <= 0)
				{
					// Stun period is over.
					anim.SetBool ("Death", false);
					stunned = false;
				}
			}

			if(Input.GetKeyDown("a")){
				if (pv.isMine) {
					if(!stunned){
						powerup.SendMessage("Assist");
					}
				}else{
					if(!opponentController.stunned){
						pv.RPC("OpponentAssist", PhotonTargets.Others);
					}
				}
			}

			if(Input.GetKeyDown("s")){
				if (pv.isMine) {
					if(!stunned){
						powerup.SendMessage("Sabotage");
					}
				}else{
					if(!opponentController.stunned){
						pv.RPC("OpponentSabotage", PhotonTargets.Others);
					}
				}
			}

			if(flying){
				flyDuration -= Time.smoothDeltaTime;
				
				if (flyDuration <= 0)
				{
					// Fly period is over.
					//anim.SetBool ("Death", false);
					flying = false;
				}
			}
			
			anim.SetBool("Ladder", climbing);
			anim.SetFloat ("vSpeed", rigidbody2D.velocity.y);
			// Clamp player movement to the edges of the screen.
			transform.position = new Vector3(Mathf.Clamp(transform.position.x, movementRangeMin.x, movementRangeMax.x),
			                                 Mathf.Clamp(transform.position.y, -4.0f, movementRangeMax.y),
			                                 transform.position.z);
		}
	}
	
	void FixedUpdate(){
		if (! game.IsPaused()){
			if (climbing||flying){
				rigidbody2D.gravityScale = 0;
			}
			else{
				rigidbody2D.gravityScale = 1;
			}
		}
	}

	public void MoveRight(){
		if (! stunned) {
			if (grounded || climbing||flying) {
				anim.SetTrigger ("isWalking");
				if (!facingRight) {
					//facing = 1.0f;
					facingRight = true;
					transform.Rotate (0, 180, 0);
				}
			
				transform.Translate (speed, 0, 0);
			}
		}
		LogPosition ();
	}

	public void MoveLeft(){
		if (! stunned){
			if (grounded || climbing|| flying){
				anim.SetTrigger ("isWalking");
				if (facingRight){
					//facing = 1.0f;
					facingRight = false;
					transform.Rotate(0, 180, 0);
				}

				transform.Translate(speed, 0, 0);
			}
		}
		LogPosition ();
	}

	public void MoveUp(){
		if ((climbing && (! aboveLadder)) || flying){
			anim.SetTrigger("isClimbing");
			int platformLayer = LayerMask.NameToLayer("Platforms");
			int playerLayer = LayerMask.NameToLayer("Player One");
			
			if (playerIndex != 0){
				playerLayer = LayerMask.NameToLayer("Player Two");
			}
			
			Physics2D.IgnoreLayerCollision(platformLayer, playerLayer, true);
			
			prevPosition = transform.position;
			
			transform.Translate(0, climbSpeed, 0);
		}
		LogPosition ();
	}

	public void MoveDown(){
		if (aboveLadder|| flying){
			int platformLayer = LayerMask.NameToLayer("Platforms");
			int playerLayer = LayerMask.NameToLayer("Player One");
			if (playerIndex != 0){
				playerLayer = LayerMask.NameToLayer("Player Two");
			}
			
			Physics2D.IgnoreLayerCollision(platformLayer, playerLayer, true);
		}
		if (bottomLadder && (!flying)){
			int platformLayer = LayerMask.NameToLayer("Platforms");
			int playerLayer = LayerMask.NameToLayer("Player One");
			if (playerIndex != 0){
				playerLayer = LayerMask.NameToLayer("Player Two");
			}
			Physics2D.IgnoreLayerCollision(platformLayer, playerLayer, false);
		}
		
		if (climbing||flying){
			anim.SetTrigger("isClimbing");
			if (! bottomLadder){
				prevPosition = transform.position;
				
				transform.Translate(0, -1 * climbSpeed, 0);
			}
		}
		LogPosition ();
	}

	public void NewRound(){
		stunDuration = 3.0f;
		flyDuration = 3.0f;
		grounded = true;
		
		aboveLadder = false;
		bottomLadder = false;
		climbing = false;
		
		stunned = false;
		flying = false;

		anim.SetBool ("Death", false);
		anim.SetBool("Ladder", climbing);

		if (playerIndex == 0) {
			facingRight = true;
		} else {
			facingRight = false;
		}
		LogPosition ();
	}

	public void LogPosition(){
		int x = (int)transform.position.x +10;
		if (x < 0) {
			x = 0;
		}
		if (x > 20) {
			x = 20;
		}

		int y = (int)((transform.position.y + 4)*2);
		if (y < 0) {
			y = 0;
		}

		if (playerIndex == 0) {
			logScript.SetP1Pos(x, y);
		} else {
			logScript.SetP2Pos(x, y);
		}
	}

	//[RPC]
	public void Stun()
	{
		stunned = true;
		anim.SetBool ("Death", true);
		stunDuration = 3.0f;
	}

	public void Fly(){
		anim.SetTrigger("Explode");
		flying = true;
		flyDuration = 3.0f;
	}

	[RPC]
	public void OpponentAssist()
	{
		//print ("opponent assist");
		if (pv.isMine) {
			opponentPowerup.SendMessage("Assist");
		}
	}

	[RPC]
	public void OpponentSabotage()
	{
		if (pv.isMine) {
			powerup.SendMessage("OpponentSabotage");
		}
	}

	[RPC]
	public void OpponentLeft(){
		//print ("opponent Left");
		if (!pv.isMine) {
			MoveLeft();
		}
	}

	[RPC]
	public void OpponentRight(){
		//print ("opponent right");
		if (!pv.isMine) {
			MoveRight();
		}
	}

	[RPC]
	public void OpponentUp(){
		//print ("opponent Up");
		if (!pv.isMine) {
			MoveUp();
		}
	}

	[RPC]
	public void OpponentDown(){
		//print ("opponent Down");
		if (!pv.isMine) {
			MoveDown();
		}
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.transform.collider2D.CompareTag("Platform"))
		{
			transform.rigidbody2D.isKinematic = true;
		}
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
	}
}

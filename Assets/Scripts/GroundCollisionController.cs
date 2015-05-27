using UnityEngine;
using System.Collections;

public class GroundCollisionController : MonoBehaviour {
	
	PlayerController player;
	
	void Awake()
	{
		player = transform.GetComponentInParent<PlayerController>();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("AboveCheck"))
		{
			// Enable grounded on player.
			player.grounded = true;
			
			// Enable collision detection with platforms.
			int platformLayer = LayerMask.NameToLayer("Platforms");
			int playerLayer = LayerMask.NameToLayer("Player One");
			
			if (player.playerIndex != 0)
			{
				playerLayer = LayerMask.NameToLayer("Player Two");
			}
			
			Physics2D.IgnoreLayerCollision(platformLayer, playerLayer, false);
			
			float platformTop = other.transform.parent.gameObject.transform.collider2D.bounds.max.y;
		}
		
		if (other.CompareTag("AboveLadderCheck"))
		{
			//Debug.Log("Entering Above ladder");
			
			player.aboveLadder = true;
		}
		
		if (other.CompareTag("Ladder"))
		{
			//Debug.Log("Entering Ladder");
			
			player.climbing = true;
			
			
		}
		
		if (other.CompareTag("BottomLadderCheck"))
		{
			//Debug.Log("Entering Bottom ladder");
			
			player.bottomLadder = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("AboveLadderCheck"))
		{
			//Debug.Log("Exiting Above ladder");
			
			player.aboveLadder = false;
		}
		
		if (other.CompareTag("BottomLadderCheck"))
		{
			//Debug.Log("Exiting Bottom ladder");
			
			player.bottomLadder = false;
		}
		
		if (other.CompareTag("Ladder"))
		{
			//Debug.Log("Exiting ladder");
			
			player.climbing = false;
		}
		
		if (other.CompareTag("AboveCheck"))
		{
			//Debug.Log("Exiting Above");
			
			player.grounded = false;
		}
	}
}

using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	public GameObject P1Prefab;
	public GameObject P2Prefab;

	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings ("v1.0");
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void OnJoinedLobby()
	{
		//print (PhotonNetwork.GetRoomList());
		PhotonNetwork.JoinRandomRoom ();
	}

	public void OnJoinedRoom(){
		if (!PhotonNetwork.player.isMasterClient) {
			Transform pos2 = GameObject.FindGameObjectWithTag ("P2Spawn").transform;
			GameObject p2 = PhotonNetwork.Instantiate ("Player Two", pos2.position, pos2.rotation, 0);
			
			PlayerController controller = p2.GetComponent<PlayerController>();
			controller.enabled = true;
		}
	}

	public void OnCreatedRoom(){
		Transform pos1 = GameObject.FindGameObjectWithTag ("P1Spawn").transform;
		GameObject p1 = PhotonNetwork.Instantiate ("Player One", pos1.position, pos1.rotation, 0);

		PlayerController controller = p1.GetComponent<PlayerController>();
		controller.enabled = true;
	}


	public void OnPhotonRandomJoinFailed(){
		//print ("Join failed");
		RoomOptions newRoomOptions = new RoomOptions();
		newRoomOptions.isOpen = true;
		newRoomOptions.isVisible = true;
		newRoomOptions.maxPlayers = 2;
		string roomName = "Room" + PhotonNetwork.GetRoomList ().Length;
		PhotonNetwork.CreateRoom (roomName, newRoomOptions, TypedLobby.Default);
	}

	public void OnMasterClientSwitched() {
		if ( PhotonNetwork.isMasterClient ) {
			PhotonView p1 = GameObject.Find ("Player One(Clone)").GetPhotonView();
			PhotonView p2 = GameObject.Find ("Player Two(Clone)").GetPhotonView();
			if (p1 != null) {
				PhotonNetwork.Destroy(p1);
			}
			if (p2 != null) {
				PhotonNetwork.Destroy(p2);
			}
			Transform pos1 = GameObject.FindGameObjectWithTag ("P1Spawn").transform;
			GameObject player1 = PhotonNetwork.Instantiate ("Player One", pos1.position, pos1.rotation, 0);
			
			//print (PhotonNetwork.GetRoomList ());
			PlayerController controller = player1.GetComponent<PlayerController>();
			controller.enabled = true;
		}
	}
}

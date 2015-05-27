using UnityEngine;
using System.Collections;

public class LoggerScript : MonoBehaviour {
	static string post_url = "http://hci-mturk.usask.ca:7070/trial";

	int round;

	int roundStart;

	int p1Score;
	int p2Score;

	int p1x;
	int p1y;
	int p2x;
	int p2y;

	int g1x;
	int g1y;
	int g2x;
	int g2y;

	int p1Assist = 0;
	int p1Sabotage = 0;

	int p2Assist = 0;
	int p2Sabotage = 0;

	int winner = 0;

	int p1id = 1;
	int p2id = 2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	public IEnumerator EventLog(int mode, int eventType){

		WWWForm form = new WWWForm ();
		int curTime = (int) (Time.time * 1000);
 		int elapsedTime = curTime - roundStart;

		string [] events = new string[] {"P1 Sabotage", "P2 Sabotage", 
			"P1 Assist", "P2 Assist", "P1 Collect", "P2 Collect", 
			"P1 Steal", "P2 Steal","P1 Powerup Bar Fills", 
			"P2 Powerup Bar Fills", "Round Start", "Round End"};

		form.AddField ("mode", mode);

		form.AddField ("p1id", p1id);
		form.AddField ("p2id", p2id);
		if (mode == 0) {
			//event log
			//form.AddField("pid", System_info.PID.ToString ());
			form.AddField ("event", events[eventType]);
			form.AddField ("startTime", roundStart);
			form.AddField ("curTime", curTime);
			form.AddField ("elapsedTime", elapsedTime);
			form.AddField ("round", round);
			form.AddField ("p1score", p1Score);
			form.AddField ("p2score", p2Score);
			form.AddField ("p1x", p1x);
			form.AddField ("p1y", p1y);
			form.AddField ("p2x", p2x);
			form.AddField ("p2y", p2y);
			form.AddField ("g1x", g1x);
			form.AddField ("g1y", g1y);
			form.AddField ("g2x", g2x);
			form.AddField ("g2y", g2y);

			print("Event: " + events[eventType]);
			print("Start Time: " + roundStart);
			print("Current Time: " + curTime);
			print("Elapsed Time: " + elapsedTime);
			print("Round: " + round);
			print ("P1score: " + p1Score);
			print("P2score: " + p2Score);
			print("P1x: " + p1x);
			print("P1y: " + p1y);
			print("P2x: " + p2x);
			print("P2y: " + p2y);
			print("G1x: " + g1x);
			print("G1y: " + g1y);
			print("G2x: " + g2x);
			print("G2y: " + g2y);
		} else {
			//summary log 
			form.AddField ("startTime", roundStart);
			form.AddField ("curTime", curTime);
			form.AddField ("elapsedTime", elapsedTime);
			form.AddField("round", round);
			form.AddField("p1score", p1Score);
			form.AddField("p2score", p2Score);
			form.AddField("p1assist", p1Assist);
			form.AddField("p1sabotage", p1Sabotage);
			form.AddField("p2assist", p2Assist);
			form.AddField("p2sabotage", p2Sabotage);
			form.AddField("winner", winner);

			print("Start Time: " + roundStart);
			print("Current Time: " + curTime);
			print("Elapsed Time: " + elapsedTime);
			print("Round: " + round);
			print("P1score: " + p1Score);
			print("P2score: " + p2Score);
			print("P1assist: " + p1Assist);
			print("P1sabotage: " + p1Sabotage);
			print("P2assist: " + p2Assist);
			print("P2sabotage: " + p2Sabotage);
			print("Winner: " + winner);
		}
//
//		print ("\n");
		WWW w = new WWW (post_url, form);

		yield return w;
	}
	
	public void SetScore1(int score){
		p1Score = score;
	}

	public void SetScore2(int score){
		p2Score = score;
	}

	public void SetP1Pos(int x, int y){
		p1x = x;
		p1y = y;
	}

	public void SetP2Pos(int x, int y){
		p2x = x;
		p2y = y;
	}

	public void SetGem1Pos(int x, int y){
		g1x = x;
		g1y = y;
	}

	public void SetGem2Pos(int x, int y){
		g2x = x;
		g2y = y;
	}

	public void SetRound(int r){
		round = r;
	}

	public void SetRoundStart(){
		roundStart = (int)(Time.time * 1000);
		SetScore1 (0);
		SetScore2 (0);
		SetWinner (0);
		ResetPowerups ();
	}

	public void SetWinner(int w){
		winner = w;
	}

	public void P1Assist(){
		p1Assist++;
		print ("P1 Assist: " + p1Assist);
	}

	public void P1Sabotage(){
		p1Sabotage++;
		print ("P1 Sabotage: " + p1Sabotage);
	}

	public void P2Assist(){
		p2Assist++;
		print ("P2 Assist: " + p2Assist);
	}

	public void P2Sabotage(){
		p2Sabotage++;
		print ("P2 Sabotage: " + p2Sabotage);
	}

	public void ResetPowerups(){
		p1Assist = 0;
		p1Sabotage = 0;
		p2Assist = 0;
		p2Sabotage = 0;
	}
}

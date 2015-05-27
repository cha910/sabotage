using UnityEngine;
using System.Collections;

public class GameMenuController : MonoBehaviour {

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update ()
    {
        // Check for input to start.

        if (Input.GetKeyDown("return"))
        {
            // Start the main game.
            Application.LoadLevel("Main");
        }
    }
}
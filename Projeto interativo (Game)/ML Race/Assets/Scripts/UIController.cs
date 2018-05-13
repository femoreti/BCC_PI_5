using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public static UIController instance;
    public Text score;

	// Use this for initialization
	void Awake () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void updateScore(int scoreValue)
    {
        score.text = scoreValue.ToString() + "m";
    }
}

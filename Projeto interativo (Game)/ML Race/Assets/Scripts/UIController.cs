using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    public static UIController instance;

    public GameObject _gameOverScreen;
    public Text score, GameOverScore;

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

    public void OnGameOver(int scoreValue)
    {
        _gameOverScreen.SetActive(true);
        GameOverScore.text = scoreValue.ToString() + "m";
    }
}

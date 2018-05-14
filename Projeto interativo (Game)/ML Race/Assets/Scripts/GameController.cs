using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public static GameController Instance;

    public ControlPlayer Player;
    public List<GameObject> blockPrefabs;
    public int initialBlocks = 2;

    private GameObject lastBlock;
    private List<GameObject> _currBlocks = new List<GameObject>();
    public Transform roadContainer;
    public float gameSpeed = 0;
    public int maxSpeed = 30;

    private float initialTime, finalAccTime;
    private int score = 0, scoreLimit = 1000;
    private int initialSpeed;

    [HideInInspector]
    public bool PAUSE;

	// Use this for initialization
	void Awake()
    {
        Instance = this;

        Application.targetFrameRate = 60;

        initialSpeed = maxSpeed;

        Reset();
    }

    void InsertInitialRoad()
    {
        _currBlocks.Clear();

        while (_currBlocks.Count < 5)
        {
            GameObject go = Instantiate(blockPrefabs[0], roadContainer);
            //go.transform.position = Vector3.zero;
            go.GetComponent<Blocks>().controlBlockRef = this;
            if (lastBlock != null)
                go.GetComponent<Blocks>().otherBlockPos = lastBlock.GetComponent<Blocks>().endBlock;

            if (_currBlocks.Count == 0)
            {
                go.transform.position = Vector3.zero;
            }
            else
            {
                go.transform.position = lastBlock.GetComponent<Blocks>().endBlock.position;
            }

            lastBlock = go;
            _currBlocks.Add(go);
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Reset();

        if (PAUSE)
        {

            if (gameSpeed > 0)
            {
                float delta = CubicEaseIn(Time.time / finalAccTime);
                gameSpeed = gameSpeed - delta;
            }
            else
                gameSpeed = 0;
            return;
        }

		while(_currBlocks.Count < initialBlocks)
        {
            GameObject go = Instantiate(blockPrefabs[Random.Range(1, blockPrefabs.Count)], roadContainer);
            go.GetComponent<Blocks>().controlBlockRef = this;
            if(lastBlock != null)
                go.GetComponent<Blocks>().otherBlockPos = lastBlock.GetComponent<Blocks>().endBlock;

            if (_currBlocks.Count == 0)
            {
                go.transform.position = Vector3.zero;
            }
            else
            {
                go.transform.position = lastBlock.GetComponent<Blocks>().endBlock.position;
            }

            lastBlock = go;
            _currBlocks.Add(go);
        }

        if (gameSpeed < maxSpeed)
        {
            float delta = CubicEaseIn(Time.time / finalAccTime);
            gameSpeed = (1 - delta) * gameSpeed + (maxSpeed * delta);
        }
        else
            gameSpeed = maxSpeed;

        float elapsedTime = Time.time - initialTime;
        score = Mathf.RoundToInt(((gameSpeed * Mathf.Pow(elapsedTime, 2)) / 2) / 10f);

        //Debug.Log(elapsedTime);
        if (score > 0 && score > scoreLimit)
        {
            maxSpeed += 5;
            scoreLimit *= 2;
            if (maxSpeed % 20 == 0)
                initialBlocks += 5;
            finalAccTime = Time.time + 1;
        }
        UIController.instance.updateScore(score);
    }

    private static float CubicEaseIn(float t)
    {
        return 1f * (t /= 1f) * t * t;
    }

    public void RemoveBlockFromList(GameObject go)
    {
        Destroy(_currBlocks[0]);
        _currBlocks.RemoveAt(0);
    }

    public void GameOver()
    {
        PAUSE = true;
        finalAccTime = Time.time + 1;

        UIController.instance.OnGameOver(score);
        Debug.Log("GameOver");
    }

    public void Reset()
    {
        Debug.Log("Game Reset");
        int numChildren = roadContainer.childCount;
        for (int i = 0; i < numChildren; i++)
        {
            Destroy(roadContainer.GetChild(i).gameObject);
        }

        InsertInitialRoad();

        UIController.instance._gameOverScreen.SetActive(false);

        Player.Reset();
        maxSpeed = initialSpeed;
        gameSpeed = 0;

        initialTime = Time.time;
        finalAccTime = Time.time + 1f;
        PAUSE = false;
    }
}

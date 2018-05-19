using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct GameBlocks
{
    public List<GameObject> gameBlocks;
}
public class GameController : MonoBehaviour {

    public static GameController Instance;

    public ControlPlayer Player;
    
    public List<GameBlocks> GameBlocks = new List<GameBlocks>();
    public int totalBlocks = 10;

    private GameObject lastBlock;
    private List<GameObject> _currBlocks = new List<GameObject>();
    public Transform roadContainer;
    public float gameSpeed = 0;
    public int maxSpeed = 30;

    private float initialTime, finalAccTime;
    private int score = 0, scoreLimit = 1000;
    private int initialSpeed, initialBlocks;

    public Shader replacement;
    [Range(-0.01f, 0.01f)]
    public float curve = 0;
    [Range(-0.01f, 0.01f)]
    public float climb = 0;

    private float timeToTestCurve;
    private float randomCurves = 0.3f;
    private bool isInCurve = false;

    [HideInInspector]
    public bool PAUSE;

	// Use this for initialization
	void Awake()
    {
        Instance = this;

        Application.targetFrameRate = 60;

        initialSpeed = maxSpeed;
        initialBlocks = totalBlocks;

        Camera.main.SetReplacementShader(replacement, "RenderType");

        Reset();
    }

    void InsertInitialRoad()
    {
        _currBlocks.Clear();

        while (_currBlocks.Count < 5)
        {
            GameObject go = Instantiate(GameBlocks[Global.Predicted].gameBlocks[0], roadContainer);
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

    private int curveState = 0;
    public float CurveLimit = 0.003f;
    private int randomDirection = 1;

    private void DoCurve()
    {
        if(isInCurve)
        {
            if (curveState == 0)
            {
                curve = Mathf.Lerp(curve, (CurveLimit + 0.0005f) * randomDirection, 0.1f * Time.deltaTime);
                //curve = Delta * 0.005f;
                if(randomDirection == 1)
                {
                    if (curve >= CurveLimit)
                    {
                        curve = CurveLimit;
                        curveState = 1;
                    }
                }
                else
                {
                    if (curve <= (CurveLimit * randomDirection))
                    {
                        curve = CurveLimit * randomDirection;
                        curveState = 1;
                    }
                }
            }
            else if(curveState == 1)
            {
                curve = Mathf.Lerp(curve, -0.009f * randomDirection, 0.05f * Time.deltaTime);
                //curve = (1 - Delta) * 0.005f;
                if(randomDirection == 1)
                {
                    if (curve <= 0f)
                    {
                        curve = 0f;
                        isInCurve = false;
                        timeToTestCurve = Time.time + 3f;
                    }
                }
                else
                {
                    if (curve >= 0f)
                    {
                        curve = 0f;
                        isInCurve = false;
                        timeToTestCurve = Time.time + 3f;
                    }
                }
            }
        }
        else
        {
            if(Time.time > timeToTestCurve)
            {
                if (Random.value <= randomCurves)
                {
                    isInCurve = true;
                    curve = 0f;
                    curveState = 0;

                    if(Random.value >= 0.5f)
                    {
                        randomDirection = 1;
                    }
                    else
                    {
                        randomDirection = -1;
                    }
                }
                else
                    timeToTestCurve = Time.time + 3f;
            }
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            Reset();

        Shader.SetGlobalFloat("_climb", climb);
        Shader.SetGlobalFloat("_curve", curve);

        if (PAUSE)
        {

            if (gameSpeed > 0)
            {
                //float delta = CubicEaseIn(Time.time / finalAccTime);
                //gameSpeed = gameSpeed - delta;
                gameSpeed -= Time.deltaTime * 15f;
            }
            else
                gameSpeed = 0;
            return;
        }

        DoCurve();

        while (_currBlocks.Count < totalBlocks)
        {
            int blockIndex = (Random.value > 0.05f) ? Random.Range(1, GameBlocks[Global.Predicted].gameBlocks.Count) : 0;

            GameObject go = Instantiate(GameBlocks[Global.Predicted].gameBlocks[blockIndex], roadContainer);
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
                totalBlocks += 5;
            finalAccTime = Time.time + 1;
        }
        UIController.instance.updateScore(score);
    }

    private static float CubicEaseIn(float t)
    {
        float result = 1f * (t /= 1f) * t * t;
        return (result < 1) ? result : 1;
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

        timeToTestCurve = Time.time + 10f;
        isInCurve = false;
        curve = 0;

        //climb = 0;

        totalBlocks = initialBlocks;
        initialTime = Time.time;
        finalAccTime = Time.time + 1f;
        PAUSE = false;
    }

    public void ClickMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}

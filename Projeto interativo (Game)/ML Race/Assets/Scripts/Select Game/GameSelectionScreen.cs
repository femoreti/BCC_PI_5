using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectionScreen : MonoBehaviour
{
    public GameObject webCamBars, resultObj, wrongPred;
    public ControlWebcam controlWebcam;
    public ReadPicServerSide server;
    public ObjModels models;
    public Text contador, textResult;
    public int CamTimer = 10;
    //public RawImage _wrongPredImg;

    private float initialTimer;
    private bool canTookPic = false;

    private int predictedObj = -1;

	// Use this for initialization
	void Start () {
        contador.text = "5";
	}

    public void OnPreparePicture()
    {
        controlWebcam.ResumeWebcam();

        models.TurnModelsOff();
        webCamBars.SetActive(true);
        wrongPred.SetActive(false);
        resultObj.SetActive(false);
        controlWebcam.StartCam();

        canTookPic = true;
        initialTimer = Time.time + CamTimer;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            OnPreparePicture();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            OnCorrectPrediction();

        if (!canTookPic)
            return;

        if (Time.time < initialTimer)
        {
            contador.text = Mathf.RoundToInt(initialTimer - Time.time).ToString();
        }
        else
        {
            if(canTookPic)
            {
                StartCoroutine(tookPicture());
                canTookPic = false;
            }
        }
	}

    private IEnumerator tookPicture()
    {
        contador.text = "";

        controlWebcam.OnTakePic();
        controlWebcam.PauseWebcam();

        //yield return new WaitForSecondsRealtime(2f);

        server.callback = CallbackServer;
        server.OnReadPic();

        yield break;
        
    }

    public void CallbackServer(object param)
    {
        Debug.Log(param);

        string str = "";
        predictedObj = (int)param;
        switch (predictedObj)
        {
            case 0:
                str = "um: CARRO";
                break;
            case 1:
                str = "uma: MOTO";
                break;

            case 2:
                str = "um: BARCO";
                break;

            case 3:
                str = "um: AVIÃO";
                break;
        }

        models.TurnModelOn(predictedObj);
        controlWebcam.StopWebcam();
        resultObj.SetActive(true);
        textResult.text = "Acredito que é " + str;
    }

    public void OnWrongPrediction()
    {
        resultObj.SetActive(false);
        wrongPred.SetActive(true);

        //_wrongPredImg.texture = controlWebcam.lastPicture;
    }

    public void OnCorrectPrediction()
    {
        string str = "";
        switch (predictedObj)
        {
            case 0:
                str = "car";
                break;
            case 1:
                str = "motorbike";
                break;

            case 2:
                str = "boat";
                break;

            case 3:
                str = "plane";
                break;
        }

        string path = Application.dataPath + "/../../../Entrega 3/assets/" + str;

        string[] files = Directory.GetFiles(path);

        controlWebcam.SaveLastImg(path, str + "_ ("+ (files.Length + 1) +")");

        server.OnUpdate(predictedObj);

        Global.Predicted = predictedObj;

        Debug.Log("Inicia Jogo, ID: " + predictedObj);
    }

    public void SelectCorretImg(int index)
    {
        string str = "";
        switch (index)
        {
            case 0:
                str = "car";
                break;
            case 1:
                str = "motorbike";
                break;

            case 2:
                str = "boat";
                break;

            case 3:
                str = "plane";
                break;
        }

        string path = Application.dataPath + "/../../../Entrega 3/assets/" + str;

        string[] files = Directory.GetFiles(path);

        controlWebcam.SaveLastImg(path, str + "_ (" + (files.Length + 1) + ")");

        server.OnUpdate(index);

        Global.Predicted = index;
    }
}

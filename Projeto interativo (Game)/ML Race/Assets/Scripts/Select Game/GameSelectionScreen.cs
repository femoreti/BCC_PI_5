using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSelectionScreen : MonoBehaviour
{
    public GameObject webCamBars, resultObj;
    public ControlWebcam controlWebcam;
    public ReadPicServerSide server;

    public Text contador, textResult;
    private float initialTimer;
    private bool canTookPic = false;

	// Use this for initialization
	void Start () {
        contador.text = "5";
	}

    public void OnPreparePicture()
    {
        controlWebcam.ResumeWebcam();

        webCamBars.SetActive(true);
        resultObj.SetActive(false);
        controlWebcam.StartCam();

        canTookPic = true;
        initialTimer = Time.time + 5;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            OnPreparePicture();

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
                contador.text = "";

                controlWebcam.OnTakePic();
                controlWebcam.PauseWebcam();

                server.callback = CallbackServer;
                server.OnReadPic();
                canTookPic = false;
            }
        }
	}

    public void CallbackServer(object param)
    {
        Debug.Log(param);

        string str = "";
        int result = (int)param;
        switch (result)
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

        resultObj.SetActive(true);
        textResult.text = "Acredito que é " + str;
    }
}

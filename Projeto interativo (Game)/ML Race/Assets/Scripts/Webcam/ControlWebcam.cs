using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ControlWebcam : MonoBehaviour {
    private RawImage rawImage;
    private WebCamTexture webcamTexture;

    private void Awake()
    {
    }

    // Use this for initialization
    void Start ()
    {
        //StartCam();
    }

    //private void Update()
    //{
        
    //}

    public void StartCam()
    {
        gameObject.SetActive(true);

        rawImage = GetComponent<RawImage>();

        if (WebCamTexture.devices.Length == 0)
            return;
        if(webcamTexture == null)
        {
            webcamTexture = new WebCamTexture();
            rawImage.texture = webcamTexture;
            webcamTexture.Play();
        }
    }

    public void PauseWebcam()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
            webcamTexture.Pause();
    }

    public void ResumeWebcam()
    {
        if (webcamTexture != null && !webcamTexture.isPlaying)
            webcamTexture.Play();
    }

    public void StopWebcam()
    {
        if(webcamTexture != null)
        {
            webcamTexture.Stop();
            webcamTexture = null;
        }

        gameObject.SetActive(false);
    }

    public void OnTakePic()
    {
        int width = Screen.width, height = Screen.height;

        //if(webcamTexture != null)
        //{
        //    width = webcamTexture.width;
        //    height = webcamTexture.height;
        //}

        float ratio = (float)Screen.width / (float)Screen.height;
        float targetRatio = 4f / 3f;
        float multiplier = 1;

        int newWidth = width;
        int newHeight = height;

        float x = 0;
        float y = 0;

        if (ratio > targetRatio)
        {
            multiplier = targetRatio / ratio;
            newWidth = Mathf.RoundToInt(width * multiplier);
            //newHeight = Mathf.RoundToInt(height * multiplier);
            x = (width - newWidth) / 2;
        }


        RenderTexture rt = new RenderTexture(newWidth, newHeight, 24);
        Camera.main.targetTexture = rt;

        Texture2D screenShot = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        File.WriteAllBytes(Application.dataPath + "/../../../Entrega 3/predictions/SavedScreen.jpg", screenShot.EncodeToJPG());

        Debug.Log(Application.dataPath + "/../../../Entrega 3/predictions/SavedScreen.jpg");
    }
}

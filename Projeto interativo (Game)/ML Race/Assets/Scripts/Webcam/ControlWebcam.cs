using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ControlWebcam : MonoBehaviour {
    public RawImage rawImage;
    private WebCamTexture webcamTexture;
    // Use this for initialization
    void Start ()
    {
        if (WebCamTexture.devices.Length == 0)
            return;
        webcamTexture = new WebCamTexture();
        rawImage.texture = webcamTexture;
        webcamTexture.Play();
    }
	
	// Update is called once per frame
	void Update () {

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            OnTakePic();
        }
		
	}

    public void OnTakePic()
    {
        int width = 100, height = 100;

        if(webcamTexture != null)
        {
            width = webcamTexture.width;
            height = webcamTexture.height;
        }

        RenderTexture rt = new RenderTexture(width, height, 24);
        Camera.main.targetTexture = rt;

        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        File.WriteAllBytes(Application.dataPath + "/../../../Entrega 3/predictions/SavedScreen.jpg", screenShot.EncodeToJPG());

        Debug.Log(Application.dataPath + "/../../../Entrega 3/predictions/SavedScreen.jpg");
    }
}

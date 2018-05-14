using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ControlWebcam : MonoBehaviour {
    public RawImage rawImage;
    private WebCamTexture webcamTexture;

    public RawImage pic; 

    // Use this for initialization
    void Start () {
        pic.gameObject.SetActive(false);

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
        RenderTexture rt = new RenderTexture(webcamTexture.width, webcamTexture.height, 24);
        Camera.main.targetTexture = rt;

        Texture2D screenShot = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, webcamTexture.width, webcamTexture.height), 0, 0);
        Camera.main.targetTexture = null;
        Destroy(rt);

        File.WriteAllBytes(Application.dataPath + "/../../../Entrega 3/predictions/SavedScreen.jpg", screenShot.EncodeToJPG());
        return;

        Texture2D snap = new Texture2D(webcamTexture.width, webcamTexture.height);
        snap.SetPixels(webcamTexture.GetPixels());
        snap.Apply();

        //Color[] c = mTexture.GetPixels(0, 0, 200, 200);
        //Texture2D m2Texture = new Texture2D(200, 200);
        //m2Texture.SetPixels(c);
        //m2Texture.Apply();

        File.WriteAllBytes(Application.dataPath + "/../SavedScreen.jpg", snap.EncodeToJPG());

        Destroy(snap);
    }
}

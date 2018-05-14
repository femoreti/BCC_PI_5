using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadPicServerSide : MonoBehaviour {

	// Use this for initialization
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(OnReadPic());
	}
	
	public IEnumerator OnReadPic()
    {
        Debug.Log("initialize");

        UnityWebRequest webRequest = UnityWebRequest.Get("http://127.0.0.1:5000/predict?q=SavedScreen.jpg");
        yield return webRequest.SendWebRequest();

        if(webRequest.isHttpError || webRequest.isNetworkError || webRequest.downloadHandler.text == "404")
        {
            Debug.LogError("OH NOES");
        }
        else
        {
            Debug.Log("Result = " + webRequest.downloadHandler.text);
        }
    }
}

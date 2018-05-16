using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ReadPicServerSide : MonoBehaviour
{
    public Delegates.OneParam callback;

	// Use this for initialization
	void Update () {
#if UNITY_EDITOR
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //    StartCoroutine(OnReadPic());
#endif
    }

    public void OnReadPic()
    {
        StartCoroutine(OnReadPicRoutine());
    }
	
	private IEnumerator OnReadPicRoutine()
    {
        Debug.Log("Start Prediction");

        //while (true)
        //{
            //ControlWebcam.instance.OnTakePic();
            //yield return new WaitForEndOfFrame();

        UnityWebRequest webRequest = UnityWebRequest.Get("http://127.0.0.1:5000/predict?q=SavedScreen.jpg");
        yield return webRequest.SendWebRequest();

        if (webRequest.isHttpError || webRequest.isNetworkError || webRequest.downloadHandler.text == "404")
        {
            Debug.LogError("OH NOES");
        }
        else
        {
            Debug.Log("Result = " + webRequest.downloadHandler.text);
            int result = int.Parse(webRequest.downloadHandler.text);
            if(callback != null)
            {
                callback(result);
                callback = null;
            }
        }

        //yield return new WaitForSecondsRealtime(1f);
        //}
    }
}

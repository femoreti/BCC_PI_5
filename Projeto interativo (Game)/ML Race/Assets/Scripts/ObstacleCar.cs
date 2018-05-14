using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCar : MonoBehaviour {
    private float speed = 3;
	// Use this for initialization
	void Start () {
        //speed = 0;
        //yield return new WaitForSeconds(Random.Range(0, 2));
        ////speed = Random.Range(1, speed);
        //speed = 5;

    }
	
	// Update is called once per frame
	void Update () {
        if (GameController.Instance.PAUSE)
            return;

        if (transform.position.y > -1)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            //    GetComponent<Rigidbody>().velocity = Vector3.zero;
            //    transform.localPosition = Vector3.zero;
            //    transform.localEulerAngles = Vector3.zero;
        }
    }
}

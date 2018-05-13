using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleCar : MonoBehaviour {
    private float speed = 5;
	// Use this for initialization
	void Start () {
        speed = Random.Range(1, speed);
	}
	
	// Update is called once per frame
	void Update () {
        if (GameController.Instance.PAUSE)
            return;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

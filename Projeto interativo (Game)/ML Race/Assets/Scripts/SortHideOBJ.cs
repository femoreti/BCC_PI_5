using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortHideOBJ : MonoBehaviour
{
    public float chance = 0.3f;
	// Use this for initialization
	void Start () {
        gameObject.SetActive((Random.value <= chance));

        //if (gameObject.activeSelf)
        //    transform.SetParent(InsertNextBlock.Instance.Obstacles.transform,true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

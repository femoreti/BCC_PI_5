using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks : MonoBehaviour {

    public float speed = 10;
    public GameController controlBlockRef;
    public Transform endBlock, otherBlockPos;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(otherBlockPos != null)
            transform.position = otherBlockPos.position;
        else
        {
            transform.Translate(Vector3.forward * -GameController.Instance.gameSpeed * Time.deltaTime);
        }

        float posY = Camera.main.WorldToScreenPoint(endBlock.position).y;
        //Debug.Log(posY);

        if (posY < -20)
            controlBlockRef.RemoveBlockFromList(gameObject);

    }
}

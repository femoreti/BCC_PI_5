using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPlayer : MonoBehaviour
{
    public List<GameObject> models = new List<GameObject>();
    public List<Transform> positions = new List<Transform>();
    private int currIndex = 2;

	// Use this for initialization
	void Awake ()
    {
        transform.position = new Vector3(positions[currIndex].position.x, transform.position.y, transform.position.z);

        GameObject go = Instantiate(models[Global.Predicted]);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (GameController.Instance.PAUSE)
            return;

		if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currIndex < positions.Count-1)
                currIndex++;
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currIndex > 0)
                currIndex--;
        }


        transform.position = Vector3.LerpUnclamped(transform.position, new Vector3(positions[currIndex].position.x, transform.position.y, transform.position.z), 10 * Time.deltaTime);
        //transform.position = ;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (GameController.Instance.PAUSE)
            return;

        if (collision.gameObject.tag == "Cars")
        {
            Debug.Log("colidiu");
            GameController.Instance.GameOver();
        }
    }

    public void Reset()
    {
        currIndex = 2;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.localEulerAngles = Vector3.zero;
        transform.position = new Vector3(positions[currIndex].position.x, positions[currIndex].position.y, positions[currIndex].position.z);
    }
}

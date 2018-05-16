using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjModels : MonoBehaviour {
    public List<GameObject> models = new List<GameObject>();
	// Use this for initialization
	void Awake () {
        TurnModelsOff();
    }
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * (15 * Time.deltaTime));
	}

    public void TurnModelsOff()
    {
        for (int i = 0; i < models.Count; i++)
        {
            models[i].gameObject.SetActive(false);
        }
    }

    public void TurnModelOn(int index)
    {
        TurnModelsOff();

        models[index].SetActive(true);
    }
}

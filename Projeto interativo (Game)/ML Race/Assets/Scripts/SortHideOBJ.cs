using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortHideOBJ : MonoBehaviour
{
    public float chance = 0.3f;
    public List<GameObject> randomCars = new List<GameObject>();
    public bool movingObj = false;
	// Use this for initialization
	void Start () {
        gameObject.SetActive((Random.value <= chance));

        if(randomCars.Count > 0)
        {
            GameObject go = Instantiate(randomCars[Random.Range(0, randomCars.Count)]);
            go.transform.SetParent(this.transform);
            go.transform.localScale = Vector3.one * 0.7f;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;

            if (movingObj)
            {
                go.AddComponent<ObstacleCar>();
                go.GetComponent<Rigidbody>().isKinematic = true;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}

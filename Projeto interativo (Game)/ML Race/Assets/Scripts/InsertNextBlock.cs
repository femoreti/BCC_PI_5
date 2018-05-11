using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertNextBlock : MonoBehaviour {

    public GameObject blockPrefab;
    public int initialBlocks = 2;

    private GameObject lastBlock;
    private List<GameObject> _currBlocks = new List<GameObject>();

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            Destroy(_currBlocks[0]);
            _currBlocks.RemoveAt(0);
        }

		while(_currBlocks.Count < initialBlocks)
        {
            GameObject go = Instantiate(blockPrefab);
            go.GetComponent<Blocks>().controlBlockRef = this;
            if(lastBlock != null)
                go.GetComponent<Blocks>().otherBlockPos = lastBlock.transform.GetChild(0).GetChild(0).transform;

            if (_currBlocks.Count == 0)
            {
                go.transform.position = Vector3.zero;
            }
            else
            {
                go.transform.position = lastBlock.transform.GetChild(0).GetChild(0).transform.position;
            }

            lastBlock = go;
            _currBlocks.Add(go);
        }
	}

    public void RemoveBlockFromList(GameObject go)
    {
        Destroy(_currBlocks[0]);
        _currBlocks.RemoveAt(0);
    }
}

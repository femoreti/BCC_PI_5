using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertNextBlock : MonoBehaviour {

    public static InsertNextBlock Instance;

    public List<GameObject> blockPrefabs;
    public int initialBlocks = 2;

    private GameObject lastBlock;
    private List<GameObject> _currBlocks = new List<GameObject>();

    public GameObject Obstacles;

	// Use this for initialization
	void Awake()
    {
        Instance = this;

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
            GameObject go = Instantiate(blockPrefabs[Random.Range(0, blockPrefabs.Count)]);
            go.GetComponent<Blocks>().controlBlockRef = this;
            if(lastBlock != null)
                go.GetComponent<Blocks>().otherBlockPos = lastBlock.GetComponent<Blocks>().endBlock;

            if (_currBlocks.Count == 0)
            {
                go.transform.position = Vector3.zero;
            }
            else
            {
                go.transform.position = lastBlock.GetComponent<Blocks>().endBlock.position;
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

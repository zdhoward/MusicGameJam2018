using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour {


    public GameObject RoadPrefab;
    public List<GameObject> RoadPool;
    public int RoadCount;

	// Use this for initialization
	void Awake () {
        RoadPool = new List<GameObject>();
        GenerateRoads();


	}

    public void CalculateRoadPositions()
    {

    }

    void GenerateRoads()
    {
        for(int i = 0; i < RoadCount; i++)
        {
            Vector3 pos = transform.position + new Vector3(3.1f, 0f) * i;

            RoadPool.Add(Instantiate(RoadPrefab, pos, Quaternion.identity, transform));
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

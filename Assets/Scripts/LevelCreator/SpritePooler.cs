using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePooler : MonoBehaviour {


    public float ParallaxDepth;
    public float ParallaxRatio = 1;
    public GameObject SpriteObject;
    public bool Randomize;

    List<GameObject> SpritePool;
    List<Vector3> RequiredLocations;
    List<bool> Randomized;
    float spriteWidth;

    public float SpriteWidthOverride;

    float leftmostX;
    float rightmostX;

    const float CAMLEFT = -20;
    const float CAMRIGHT = 20;
    float cameraRectLeft = CAMLEFT;
    float cameraRectRight = CAMRIGHT;

    float initParallaxDepth;
	// Use this for initialization
	void Start () {


        initParallaxDepth = ParallaxDepth;
        Randomized = new List<bool>();
        RequiredLocations = new List<Vector3>();
        SpritePool = new List<GameObject>();
        if(SpriteWidthOverride != 0)
        {
            spriteWidth = SpriteWidthOverride*transform.localScale.x;
        }
        else
            spriteWidth = SpriteObject.GetComponent<SpriteRenderer>().sprite.rect.width/100f*SpriteObject.transform.localScale.x;


        Debug.Log("Sprite width: " + spriteWidth);
        InitialGeneration();
        

    }

    void CalculatePositions()
    {
        cameraRectLeft = CAMLEFT + Camera.main.transform.position.x - ParallaxDepth;
        cameraRectRight = CAMRIGHT + Camera.main.transform.position.x - ParallaxDepth;
        ParallaxDepth = initParallaxDepth * Camera.main.transform.position.x;


        if (leftmostX > (cameraRectLeft + 10) * (1 - ParallaxRatio))
        {
            Vector3 pos = new Vector3(leftmostX - spriteWidth, transform.position.y, transform.position.z);

            if(Randomize)
            {
                pos += Vector3.up * Random.Range(-1f, 1f);
            }

            RequiredLocations[SpritePool.Count - 1] = pos;
            //SpritePool[SpritePool.Count - 1].transform.position = new Vector3(leftmostX - spriteWidth, transform.position.y, transform.position.z);
            Vector3 temp = RequiredLocations[SpritePool.Count - 1];

            RequiredLocations.RemoveAt(SpritePool.Count - 1);
            RequiredLocations.Insert(0, temp);
            leftmostX = leftmostX - spriteWidth;
            rightmostX = rightmostX - spriteWidth;
            Randomized[0] = false;
        }
        if (rightmostX  < (cameraRectRight - 10) * (1 - ParallaxRatio))
        {
            Vector3 pos = new Vector3(rightmostX + spriteWidth, transform.position.y, transform.position.z);

            if (Randomize)
            {
                pos += Vector3.up * Random.Range(-1f, 1f);
            }

            RequiredLocations[0] = pos;
            //SpritePool[0].transform.position = new Vector3(rightmostX + spriteWidth, transform.position.y, transform.position.z);
            rightmostX = rightmostX + spriteWidth;
            leftmostX = leftmostX + spriteWidth;
            Vector3 temp = RequiredLocations[0];
            RequiredLocations.RemoveAt(0);
            RequiredLocations.Add(temp);
            Randomized[Randomized.Count-1] = false;
        }

        

        for(int i = 0; i< SpritePool.Count; i++)
        {

            SpritePool[i].transform.position = (RequiredLocations[i] + (Vector3.right * ParallaxDepth)) + Vector3.right * (Camera.main.transform.position.x * ParallaxRatio);

        }
        
        

    }


    void InitialGeneration()
    {
        Vector3 pos = Vector3.zero;
        for (float i = cameraRectLeft; i < cameraRectRight; i += spriteWidth)
        {
            pos = transform.position + new Vector3(i, 0f);
            if (Randomize)
            {
                pos += Vector3.up * Random.Range(-1f, 1f);
            }

            SpritePool.Add(Instantiate(SpriteObject, pos, Quaternion.identity, transform));
            RequiredLocations.Add(pos);
            Randomized.Add(false);
            if (i == cameraRectLeft)
            {
                leftmostX = pos.x;
            }
        }
        rightmostX = pos.x;

        SpriteObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        CalculatePositions();
    }
}

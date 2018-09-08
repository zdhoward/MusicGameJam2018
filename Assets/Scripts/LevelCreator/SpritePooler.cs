using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritePooler : MonoBehaviour {


    public GameObject SpriteObject;

    List<GameObject> SpritePool;
    List<Vector3> RequiredLocations;
    float spriteWidth;

    public float SpriteWidthOverride;

    float leftmostX;
    float rightmostX;

    const float CAMLEFT = -20;
    const float CAMRIGHT = 20;
    float cameraRectLeft = CAMLEFT;
    float cameraRectRight = CAMRIGHT;

	// Use this for initialization
	void Start () {
        

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
        cameraRectLeft = CAMLEFT + Camera.main.transform.position.x;
        cameraRectRight = CAMRIGHT + Camera.main.transform.position.x;



        if (leftmostX > cameraRectLeft + 10)
        {
            SpritePool[SpritePool.Count - 1].transform.position = new Vector3(leftmostX - spriteWidth, transform.position.y, transform.position.z);
            GameObject temp = SpritePool[SpritePool.Count - 1];

            SpritePool.RemoveAt(SpritePool.Count - 1);
            SpritePool.Insert(0, temp);
            leftmostX = leftmostX - spriteWidth;
            rightmostX = rightmostX - spriteWidth;
        }
        if (rightmostX < cameraRectRight - 10)
        {
            SpritePool[0].transform.position = new Vector3(rightmostX + spriteWidth, transform.position.y, transform.position.z);
            rightmostX = rightmostX + spriteWidth;
            leftmostX = leftmostX + spriteWidth;
            GameObject temp = SpritePool[0];
            SpritePool.RemoveAt(0);
            SpritePool.Add(temp);
        }
        
        

    }


    void InitialGeneration()
    {
        Vector3 pos = Vector3.zero;
        for (float i = cameraRectLeft; i < cameraRectRight; i += spriteWidth)
        {
            pos = transform.position + new Vector3(i, 0f);
            SpritePool.Add(Instantiate(SpriteObject, pos, Quaternion.identity, transform));
            if(i == cameraRectLeft)
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

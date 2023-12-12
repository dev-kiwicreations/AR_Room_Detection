using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHandler : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject cube1, cube2;
    public bool isRightSpawn = false;
    // Start is called before the first frame update
    void Start()
    {
        spawnWallFromRight();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnWallFromRight()
    {
        wallPrefab.SetActive(true);
        wallPrefab.GetComponent<WallHandler>().cube1 = cube1.transform.position;
        wallPrefab.GetComponent<WallHandler>().cube2 = cube2.transform.position;
        if (!isRightSpawn)
        {
            wallPrefab.GetComponent<WallHandler>().changeParentLeft();
        }
        else
        {
            wallPrefab.GetComponent<WallHandler>().changeParentRight();
        }
    }
}

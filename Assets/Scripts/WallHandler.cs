using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHandler : MonoBehaviour
{
    public GameObject leftCorner, rightCorner;
    public Vector3 cube1, cube2;

    public GameObject TopCornerLeft;
    public GameObject TopCornerRight;

    // Start is called before the first frame update
    void Start()
    {
        //
        //changeParentLeft();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeParentLeft()
    {
        leftCorner.transform.parent = null;
        ARManager.instance.spawnedGameobject.transform.parent = leftCorner.transform;
        leftCorner.transform.position = cube1;
        Vector3 direction = cube2 - cube1;
        Quaternion rotation = Quaternion.LookRotation(direction);
        leftCorner.transform.rotation = Quaternion.Lerp(transform.rotation, rotation,1);
    }

    public void changeParentRight()
    {
        rightCorner.transform.parent = null;
        ARManager.instance.spawnedGameobject.transform.parent = rightCorner.transform;
        rightCorner.transform.position = cube1;
        Vector3 direction = cube2 - cube1;
        Quaternion rotation = Quaternion.LookRotation(direction);
        rightCorner.transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1);
    }
}

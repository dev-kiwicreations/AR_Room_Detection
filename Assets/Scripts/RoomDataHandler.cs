using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Data
{
    public Room room;
    public Door[] doors;
    public Window[] windows;
}

[Serializable]
public class Door
{
    public Position position;
    public float width;
    public float height;
}

[Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class Room
{
    public float width;
    public float length;
    public float height;
}

[Serializable]
public class Window
{
    public Position position;
    public float width;
    public float height;
}

public class RoomDataHandler : MonoBehaviour
{
    public static RoomDataHandler instance;

    //[SerializeField] TextAsset _RoomDataFile;

    string _RoomDataFileContent;

    public GameObject Room;

    string pdfFileType;

    public GameObject RoomSurroundingOutlinePrefab;
    public GameObject DoorOutlinePrefab;
    public GameObject WindowOutlinePrefab;
    public Data roomData;

    public GameObject _2DViewCamera;

    public Material SelectedWallMaterial;
    public Material OriginalWallMaterial;
    public Material ARWallMaterial;
    public Material ARDoorMaterial;
    public Material ARWindowMaterial;

    MeshRenderer selectedWall;
    bool isWallSelected = false;
    bool isCornerSelected = false;
    public int selectedWallIndex = 0;
    public int selectedCornerIndex = 0; //0 for left, 1 for right

    //UI
    public GameObject UploadButton;
    public TMPro.TMP_Text selectText;
    public TMPro.TMP_Text ErrorText;

    public GameObject aRManagerObj;
    ARManager aRManager;

    public bool invertDirection = false;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        aRManager = aRManagerObj.GetComponent<ARManager>();
    }

    public void PickFileFromMobileDirectory()
    {
        ErrorText.text = "";
        pdfFileType = NativeFilePicker.ConvertExtensionToFileType("txt"); // Returns "application/pdf" on Android and "com.adobe.pdf" on iOS

        if (NativeFilePicker.IsFilePickerBusy())
            return;

        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((path) =>
        {
            if (path == null)
            {
                Debug.Log("Operation cancelled");
                ErrorText.gameObject.SetActive(true);
                ErrorText.text = "you didn't pick any file, try again!";
            }
            else
            {
                Debug.Log("Picked file: " + path);
                StartCoroutine(loadData(path));
            }
        }, new string[] { pdfFileType });

        Debug.Log("Permission result: " + permission);
    }

    IEnumerator loadData(string path)
    {
        WWW www = new WWW("file:///" + path);
        while (!www.isDone)
            yield return null;

        if (!string.IsNullOrEmpty(www.error))
        {
            ErrorText.gameObject.SetActive(true);
            ErrorText.text = "something went wrong, try again!";
        }
        else
        {
            _RoomDataFileContent = www.text;
            StartTheProcessAfterFileIsPicked();
        }
    }
    public GameObject zoom;
    void StartTheProcessAfterFileIsPicked()
    {
        zoom.SetActive(true);
        bool isError = false;
        try
        {
            roomData = JsonUtility.FromJson<Data>(_RoomDataFileContent);
        }
        catch
        {
            isError = true;
            ErrorText.gameObject.SetActive(true);
            ErrorText.text = "The content of file you've uploaded is not in correct format, kindly try again!";
        }

        if (!isError)
        {
            CreateVirtualRoom();

            selectText.gameObject.SetActive(true);
            UploadButton.SetActive(false);
            ErrorText.gameObject.SetActive(false);

            //////adjusting 2D View according to different resolutions////////
            float tempPercentage = ((float)100 / Screen.height) * 100;
            float finalWidth = (tempPercentage / 100) * Screen.width;

            float tempPerc = (37 - finalWidth) / (56 - finalWidth);
            float finalFieldOfView = (18f - (12f * tempPerc)) / (1 - tempPerc);

            if (finalFieldOfView > 18f)
                finalFieldOfView = 18f;
            if (finalFieldOfView < 12f)
                finalFieldOfView = 12f;

            _2DViewCamera.GetComponent<Camera>().fieldOfView = finalFieldOfView;
            ///////////////////////////////////////////////////////////////////
        }
    }

    public void CreateVirtualRoom()
    {
        //wall 1
        GameObject wall1 = Instantiate(RoomSurroundingOutlinePrefab, new Vector3(roomData.room.width / 2f, 0, 0), Quaternion.identity, Room.transform);
        wall1.transform.localEulerAngles = new Vector3(0, 90, 0);
        //wall1.transform.localEulerAngles = new Vector3(0, 90, 180);
        wall1.transform.localScale = new Vector3(roomData.room.length, roomData.room.height, 0.03f);
        wall1.gameObject.tag = "WallI";
        wall1.GetComponent<WallHandler>().TopCornerLeft.transform.localScale = new Vector3(0.7f / wall1.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.x, 0.03f / wall1.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.y, 0.7f / wall1.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.z);
        wall1.GetComponent<WallHandler>().TopCornerLeft.transform.localPosition = new Vector3(0, 1, 0);
        wall1.GetComponent<WallHandler>().TopCornerRight.transform.localScale = new Vector3(0.7f / wall1.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.x, 0.03f / wall1.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.y, 0.7f / wall1.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.z);
        wall1.GetComponent<WallHandler>().TopCornerRight.transform.localPosition = new Vector3(0, 1, 0);
        Room.GetComponent<WallsRecord>().wallColliders.Add(wall1.GetComponent<BoxCollider>());

        //wall 2
        GameObject wall2 = Instantiate(RoomSurroundingOutlinePrefab, new Vector3(-roomData.room.width / 2f, 0, 0), Quaternion.identity, Room.transform);
        wall2.transform.localEulerAngles = new Vector3(0, 90, 0);
        wall2.transform.localScale = new Vector3(roomData.room.length, roomData.room.height, 0.03f);
        wall2.gameObject.tag = "Wall";
        wall2.GetComponent<WallHandler>().TopCornerLeft.transform.localScale = new Vector3(0.7f / wall2.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.x, 0.03f / wall2.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.y, 0.7f / wall2.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.z);
        wall2.GetComponent<WallHandler>().TopCornerLeft.transform.localPosition = new Vector3(0, 1, 0);
        wall2.GetComponent<WallHandler>().TopCornerRight.transform.localScale = new Vector3(0.7f / wall2.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.x, 0.03f / wall2.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.y, 0.7f / wall2.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.z);
        wall2.GetComponent<WallHandler>().TopCornerRight.transform.localPosition = new Vector3(0, 1, 0);
        Room.GetComponent<WallsRecord>().wallColliders.Add(wall2.GetComponent<BoxCollider>());

        //wall 3
        GameObject wall3 = Instantiate(RoomSurroundingOutlinePrefab, new Vector3(0, 0, roomData.room.length / 2f), Quaternion.identity, Room.transform);
        //wall3.transform.localEulerAngles = new Vector3(0, 0, 180);
        wall3.transform.localScale = new Vector3(roomData.room.width, roomData.room.height, 0.03f);
        wall3.gameObject.tag = "WallI";
        wall3.GetComponent<WallHandler>().TopCornerLeft.transform.localScale = new Vector3(0.7f / wall3.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.x, 0.03f / wall3.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.y, 0.7f / wall3.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.z);
        wall3.GetComponent<WallHandler>().TopCornerLeft.transform.localPosition = new Vector3(0, 1, 0);
        wall3.GetComponent<WallHandler>().TopCornerRight.transform.localScale = new Vector3(0.7f / wall3.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.x, 0.03f / wall3.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.y, 0.7f / wall3.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.z);
        wall3.GetComponent<WallHandler>().TopCornerRight.transform.localPosition = new Vector3(0, 1, 0);
        Room.GetComponent<WallsRecord>().wallColliders.Add(wall3.GetComponent<BoxCollider>());

        //wall 4
        GameObject wall4 = Instantiate(RoomSurroundingOutlinePrefab, new Vector3(0, 0, -roomData.room.length / 2f), Quaternion.identity, Room.transform);
        wall4.transform.localScale = new Vector3(roomData.room.width, roomData.room.height, 0.03f);
        wall4.gameObject.tag = "Wall";
        wall4.GetComponent<WallHandler>().TopCornerLeft.transform.localScale = new Vector3(0.7f / wall4.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.x, 0.03f / wall4.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.y, 0.7f / wall4.GetComponent<WallHandler>().TopCornerLeft.transform.lossyScale.z);
        wall4.GetComponent<WallHandler>().TopCornerLeft.transform.localPosition = new Vector3(0, 1, 0);
        wall4.GetComponent<WallHandler>().TopCornerRight.transform.localScale = new Vector3(0.7f / wall4.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.x, 0.03f / wall4.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.y, 0.7f / wall4.GetComponent<WallHandler>().TopCornerRight.transform.lossyScale.z);
        wall4.GetComponent<WallHandler>().TopCornerRight.transform.localPosition = new Vector3(0, 1, 0);
        Room.GetComponent<WallsRecord>().wallColliders.Add(wall4.GetComponent<BoxCollider>());

        //Cieling
        //GameObject ceiling = Instantiate(RoomSurroundingOutlinePrefab, new Vector3(0, roomData.room.height / 2f, 0), new Quaternion(0, 0, 90, 0), Room.transform);
        //ceiling.transform.localScale = new Vector3(roomData.room.width, 0.03f, roomData.room.length);

        ////floor
        //GameObject floor = Instantiate(RoomSurroundingOutlinePrefab, new Vector3(0, -roomData.room.height / 2f, 0), new Quaternion(0, 0, 90, 0), Room.transform);
        //floor.transform.localScale = new Vector3(roomData.room.width, 0.03f, roomData.room.length);

        //doors
        for (int i = 0; i < roomData.doors.Length; i++)
        {
            GameObject door = Instantiate(DoorOutlinePrefab, new Vector3(roomData.doors[i].position.x, roomData.doors[i].position.y - ((roomData.room.height - roomData.doors[i].height) / 2f), roomData.doors[i].position.z), Quaternion.identity, Room.transform);
            if (roomData.doors[i].position.x >= roomData.room.width / 2f || roomData.doors[i].position.x <= -roomData.room.width / 2f)
                door.transform.localEulerAngles = new Vector3(0, 90, 0);
            door.transform.localScale = new Vector3(roomData.doors[i].width, roomData.doors[i].height, 0.032f);
            Room.GetComponent<WallsRecord>().DoorMeshes.Add(door.GetComponent<MeshRenderer>());
        }

        //windows
        for (int i = 0; i < roomData.windows.Length; i++)
        {
            GameObject window = Instantiate(WindowOutlinePrefab, new Vector3(roomData.windows[i].position.x, -(roomData.room.height / 2) + roomData.windows[i].position.y, roomData.windows[i].position.z), Quaternion.identity, Room.transform);
            if (roomData.windows[i].position.x >= roomData.room.width / 2f || roomData.windows[i].position.x <= -roomData.room.width / 2f)
                window.transform.localEulerAngles = new Vector3(0, 90, 0);
            window.transform.localScale = new Vector3(roomData.windows[i].width, roomData.windows[i].height, 0.032f);
            Room.GetComponent<WallsRecord>().WindowMeshes.Add(window.GetComponent<MeshRenderer>());
        }

        Room.transform.localPosition = new Vector3(Room.transform.localPosition.x, -80 - ((roomData.room.height - 8) / 2), Room.transform.localPosition.z);
    }

    private void Update()
    {
        if (!isWallSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.tag.Equals("Wall"))
                        {
                            selectedWall = hit.collider.GetComponent<MeshRenderer>();
                            selectedWall.material = SelectedWallMaterial;
                            selectedWall.GetComponent<WallHandler>().TopCornerLeft.SetActive(true);
                            selectedWall.GetComponent<WallHandler>().TopCornerRight.SetActive(true);
                            isWallSelected = true;
                            selectText.text = "Now select a Corner";
                            int i = 0;
                            foreach (Collider c in Room.GetComponent<WallsRecord>().wallColliders)
                            {
                                if (c.Equals(hit.collider))
                                {
                                    selectedWallIndex = i;
                                    break;
                                }
                                i++;
                            }
                        }
                        else if (hit.collider.tag.Equals("WallI"))
                        {
                            invertDirection = true;

                            selectedWall = hit.collider.GetComponent<MeshRenderer>();
                            selectedWall.material = SelectedWallMaterial;
                            selectedWall.GetComponent<WallHandler>().TopCornerLeft.SetActive(true);
                            selectedWall.GetComponent<WallHandler>().TopCornerRight.SetActive(true);
                            isWallSelected = true;
                            selectText.text = "Now select a Corner";
                            int i = 0;
                            foreach (Collider c in Room.GetComponent<WallsRecord>().wallColliders)
                            {
                                if (c.Equals(hit.collider))
                                {
                                    selectedWallIndex = i;
                                    break;
                                }
                                i++;
                            }
                        }
                    }
                }
            }
        }
        if (!isCornerSelected)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null)
                    {
                        if (hit.collider.tag.Equals("RightCorner"))
                        {
                            hit.collider.GetComponent<MeshRenderer>().material = SelectedWallMaterial;
                            isCornerSelected = true;
                            selectText.text = "SELECTED!";
                            Debug.Log("RightCorner");
                            selectedCornerIndex = 1;
                            Invoke("OpenARCamera", 1f);
                        }
                        else if (hit.collider.tag.Equals("LeftCorner"))
                        {
                            hit.collider.GetComponent<MeshRenderer>().material = SelectedWallMaterial;
                            isCornerSelected = true;
                            selectText.text = "SELECTED!";
                            Debug.Log("LeftCorner");
                            selectedCornerIndex = 0;
                            Invoke("OpenARCamera", 1f);
                        }
                        if(invertDirection)
                        {
                            if(selectedCornerIndex == 1)
                            {
                                selectedCornerIndex = 0;
                            }
                            else if(selectedCornerIndex == 0)
                            {
                                selectedCornerIndex = 1;
                            }
                        }
                    }
                }
            }
        }
    }
    public GameObject FindCornerButton;
    void OpenARCamera()
    {
        zoom.SetActive(false);
        selectedWall.material = OriginalWallMaterial;
        selectedWall.GetComponent<WallHandler>().TopCornerLeft.SetActive(false);
        selectedWall.GetComponent<WallHandler>().TopCornerRight.SetActive(false);
        //selectText.gameObject.SetActive(false);
        _2DViewCamera.SetActive(false);
        ARManager.instance.ARCamera.SetActive(true);
        ARManager.instance.DirectionalLightForAR.SetActive(true);
        selectText.text = "Scan the area around selected corner and press button!";
        if(selectedCornerIndex == 0)
        {
            aRManager.direction = 1;

        }
        else if(selectedCornerIndex == 1)
        {
            aRManager.direction = 2;

        }
        FindCornerButton.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


public class ARManager : MonoBehaviour
{
    public static ARManager instance;

    public GameObject ARCamera;
    public GameObject DirectionalLightForAR;

    public ARRaycastManager raycastManager;
    public ARPlaneManager planeManager;

    Vector2 touchPosition;
    Vector2 touchPosition1 = Vector2.zero;
    Vector2 touchPosition2 = Vector2.zero;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public GameObject indexPrefab;
    GameObject index1;
    GameObject index2;

    public GameObject spawnedGameobject;

    bool isSpawned = false;

    public GameObject touchAndLineGameObjects;
    public LineRenderer lineRenderer;

    Pose hitPose;

    bool cornerFound = false;
    public int direction = -1;
    bool directionFound = false;
    public GameObject FindCornerButton;
    public GameObject SpawnRoomButton;

    public GameObject phoneIMGright;
    public GameObject phoneIMGleft;
    public GameObject animationParent;
    public GameObject arrowRight;
    public GameObject arrowLeft;

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

    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpawned && cornerFound)
        {
            //if (Input.touchCount > 0)
            {
                //Touch touch = Input.GetTouch(0);

                //if (touch.phase == TouchPhase.Began)
                {
                    //touchPosition = touch.position;

                    if (touchPosition1 == Vector2.zero)
                    {
                        if (index1 != null)
                        {
                            FindCornerButton.SetActive(false);
                            SpawnRoomButton.SetActive(true);
                            //touchPosition1 = index1.transform.position;
                            RoomDataHandler.instance.selectText.text = "Allign the line along the edge towards the higlighted direction and press button";
                        }
                    }
                    else
                    {
                        if (index2 != null)
                        {
                            touchPosition2 = index2.transform.position;
                            RoomDataHandler.instance.selectText.text = "Room is spawned!";
                        }
                    }

                    if(directionFound)
                    //if (touchPosition1 != Vector2.zero && touchPosition2 != Vector2.zero)
                    {
                        hitPose = hits[0].pose;
                        
                        spawnedGameobject = Instantiate(RoomDataHandler.instance.Room, hitPose.position, hitPose.rotation);
                        WallHandler wallhandler;
                        wallhandler = spawnedGameobject.GetComponent<WallsRecord>().wallColliders[RoomDataHandler.instance.selectedWallIndex].gameObject.GetComponent<WallHandler>();
                        wallhandler.cube1 = index1.transform.position;
                        wallhandler.cube2 = index2.transform.position;
                        if (RoomDataHandler.instance.invertDirection)
                        {
                            if (RoomDataHandler.instance.selectedCornerIndex == 0)
                                wallhandler.changeParentRight();
                            else
                                wallhandler.changeParentLeft();
                        }
                        else
                        {
                            if (RoomDataHandler.instance.selectedCornerIndex == 0)
                                wallhandler.changeParentLeft();
                            else
                                wallhandler.changeParentRight();
                        }

                        for (int i = 0; i < spawnedGameobject.GetComponent<WallsRecord>().wallColliders.Count; i++)
                        {
                            spawnedGameobject.GetComponent<WallsRecord>().wallColliders[i].gameObject.GetComponent<MeshRenderer>().material = RoomDataHandler.instance.ARWallMaterial;
                        }
                        for (int i = 0; i < spawnedGameobject.GetComponent<WallsRecord>().DoorMeshes.Count; i++)
                        {
                            spawnedGameobject.GetComponent<WallsRecord>().DoorMeshes[i].material = RoomDataHandler.instance.ARDoorMaterial;
                        }
                        for (int i = 0; i < spawnedGameobject.GetComponent<WallsRecord>().WindowMeshes.Count; i++)
                        {
                            spawnedGameobject.GetComponent<WallsRecord>().WindowMeshes[i].material = RoomDataHandler.instance.ARWindowMaterial;
                        }

                        planeManager.enabled = false;
                        TurnOffPlanes();

                        isSpawned = true;

                        //touchAndLineGameObjects.SetActive(false);
                    }
                }
            }
            if (raycastManager.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), hits, TrackableType.PlaneWithinPolygon))
            {
                hitPose = hits[0].pose;
                /*
                if (touchPosition1 == Vector2.zero)
                {
                    if (index1 == null)
                        index1 = Instantiate(indexPrefab, hitPose.position, hitPose.rotation, touchAndLineGameObjects.transform);
                    else
                    {
                        index1.SetActive(true);
                        index1.transform.position = hitPose.position;
                        index1.transform.rotation = hitPose.rotation;
                    }
                }
                */
                if(cornerFound)
                {
                    Vector3 cornerPosition = FindIntersection();
                    if (index1 == null)
                        index1 = Instantiate(indexPrefab, cornerPosition, Quaternion.identity);
                    else
                    {
                        index1.SetActive(true);
                        index1.transform.position = cornerPosition;
                        //index1.transform.rotation = hitPose.rotation;
                    }
                }
                if (touchPosition2 == Vector2.zero)
                {
                    if (!directionFound)
                    {
                        if (direction == 1)
                        {
                            animationParent.SetActive(true);
                            phoneIMGright.SetActive(true);
                            arrowRight.SetActive(true);

                        }
                        else if (direction == 2)
                        {
                            animationParent.SetActive(true);
                            phoneIMGleft.SetActive(true);
                            arrowLeft.SetActive(true);

                        }
                        else
                        {
                            Debug.Log("Error in getting direction");
                        }
                    }
                    lineRenderer.gameObject.SetActive(true);
                    lineRenderer.SetPosition(0, index1.transform.position);
                    if (index2 == null)
                        index2 = Instantiate(indexPrefab, hitPose.position, hitPose.rotation, touchAndLineGameObjects.transform);
                    else
                    {
                        index2.SetActive(true);
                        index2.transform.position = hitPose.position;
                        index2.transform.rotation = hitPose.rotation;
                    }
                    lineRenderer.SetPosition(1, index2.transform.position);
                }
            }
            else
            {
                if (touchPosition1 == Vector2.zero && index1 != null)
                {
                    index1.SetActive(false);
                }
                if (touchPosition2 == Vector2.zero && index2 != null)
                {
                    index2.SetActive(false);
                    lineRenderer.gameObject.SetActive(false);
                }
            }
        }
        if (!cornerFound) 
        {
            BigPlanes();
        }
    }

    void TurnOffPlanes()
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        SpawnRoomButton.gameObject.SetActive(false);
        RoomDataHandler.instance.selectText.text = "Room is spawned!";
    }

    ARPlane horizontal = null;
    ARPlane vertical = null;
    ARPlane vertical2 = null;
    void BigPlanes()
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane.alignment == PlaneAlignment.HorizontalUp && (horizontal == null || plane.extents.y > horizontal.extents.y))
            {
                horizontal = plane;
            }
            else if (plane.alignment == PlaneAlignment.Vertical)
            {
                if (vertical == null || plane.extents.x > vertical.extents.x)
                {
                    vertical2 = vertical;
                    vertical = plane;
                }
                else if (vertical2 == null || (plane.extents.x > vertical2.extents.x && plane.extents.x < vertical.extents.x))
                {
                    vertical2 = plane;
                }
            }
        }

        ApplyMaterials();
    }
    public Material redMaterial;
    public Material blueMaterial;
    public Material greenMaterial;
    public Material yellowMaterial;

    void ApplyMaterials()
    {
        // Apply materials to the planes
        if (horizontal != null)
        {
            ApplyMaterialToPlane(horizontal, yellowMaterial);
        }

        if (vertical != null)
        {
            ApplyMaterialToPlane(vertical, blueMaterial);
        }

        if (vertical2 != null)
        {
            ApplyMaterialToPlane(vertical2, greenMaterial);
        }
    }
    void ApplyMaterialToPlane(ARPlane plane, Material material)
    {
        Renderer planeRenderer = plane.GetComponent<Renderer>();

        if (planeRenderer != null)
        {
            // Assuming the ARPlane has a Renderer component
            planeRenderer.material = material;
        }
        else
        {
            Debug.LogError("Renderer not found on ARPlane.");
        }
    }

    public Vector3 FindIntersection()
    {
        Vector3 intersection = Vector3.zero;

        // Check if the required planes (horizontal, vertical, vertical2) are available
        if (horizontal != null && vertical != null && vertical2 != null)
        {
            // Get the normal vectors for each plane
            Vector3 normal1 = horizontal.normal;
            Vector3 normal2 = vertical.normal;
            Vector3 normal3 = vertical2.normal;

            // Calculate the intersection point using Cramer's rule
            Vector3 linePoint1, lineVector1, linePoint2, lineVector2;

            // Calculate the line intersection between horizontal and vertical planes
            PlanePlaneIntersection(out linePoint1, out lineVector1, horizontal.normal, horizontal.center, vertical.normal, vertical.center);

            // Calculate the line intersection between vertical and vertical2 planes
            PlanePlaneIntersection(out linePoint2, out lineVector2, vertical.normal, vertical.center, vertical2.normal, vertical2.center);

            // Calculate the intersection point of the three planes using the lines
            LineLineIntersection(out intersection, linePoint1, lineVector1, linePoint2, lineVector2);
        }
        else
        {
            Debug.LogWarning("Planes are missing. Cannot find intersection.");
        }
        if(intersection!= Vector3.zero)
        {
            
        }
        return intersection;
    }
    public void FindIntrsectionButton()
    {
        Vector3 iPoint = FindIntersection();
        if (iPoint != Vector3.zero)
        {
            cornerFound = true;

            GameObject index = Instantiate(indexPrefab, iPoint, Quaternion.identity);

            // Get the Renderer component of the instantiated index GameObject
            Renderer indexRenderer = index.GetComponent<Renderer>();

            // Set the material of the Renderer to the specified material
            indexRenderer.material = blueMaterial;
        }
    }
    public void SpawnRoomButtonMethod()
    {
        directionFound = true;
        animationParent.SetActive(false);
        phoneIMGright.SetActive(false);
        arrowRight.SetActive(false);
        phoneIMGleft.SetActive(false );
        arrowLeft.SetActive(false);
    }
    void PlayMoveCameraAnimation(int dir)
    {
        if(dir == 1)
        {

        }
        else if(dir == 2)
        {

        }
        else
        {
            Debug.Log("Error in directon, not 1 or 2");
        }
    }
    /*
    Vector3 FindMidpointAlongEdge()
    {
        // Get the center and extents of the horizontal plane
        Vector3 center = horizontal.center;
        Vector3 extents = horizontal.extents;

        // Check which side is the right side adjacent wall
        ARPlane rightAdjacentPlane = (Vector3.Dot(horizontal.normal, vertical.center - center) > 0) ? vertical : vertical2;

        // Check if rightAdjacentPlane is vertical or vertical2
        bool isRightAdjacentVertical = (rightAdjacentPlane == vertical);

        // Find the intersection line between horizontal and rightAdjacentPlane
        Vector3 intersectionLineDirection = Vector3.Cross(horizontal.normal, rightAdjacentPlane.normal);
        Vector3 intersectionLinePoint = Vector3.zero;

        if (Vector3.Dot(intersectionLineDirection, intersectionLineDirection) > 0)
        {
            // Calculate the intersection point
            Math3d.PlanePlaneIntersection(out intersectionLinePoint, out _, horizontal.normal, center, rightAdjacentPlane.normal, rightAdjacentPlane.center);
        }
        else
        {
            // Planes are parallel; handle this case as needed
            Debug.LogWarning("Planes are parallel. No unique intersection line.");
            return Vector3.zero;
        }

        // Find the midpoint along the intersection line
        Vector3 midpoint = FindMidpointAlongLine(center, intersectionLinePoint, isRightAdjacentVertical);

        return midpoint;
    }

    Vector3 FindMidpointAlongLine(Vector3 point1, Vector3 point2, bool towardsRight)
    {
        // Calculate the direction vector along the line
        Vector3 lineDirection = (point2 - point1).normalized;

        // Determine the sign based on the direction towards the right or left
        float sign = towardsRight ? 1.0f : -1.0f;

        // Calculate the midpoint along the line
        Vector3 midpoint = point1 + 0.5f * (point2 - point1) + sign * 0.5f * Vector3.Cross(lineDirection, Vector3.up);

        return midpoint;
    }
    */

    //Find the line of intersection between two planes.	The planes are defined by a normal and a point on that plane.
    //The outputs are a point on the line and a vector which indicates it's direction. If the planes are not parallel, 
    //the function outputs true, otherwise false.
    bool PlanePlaneIntersection(out Vector3 linePoint, out Vector3 lineVec, Vector3 plane1Normal, Vector3 plane1Position, Vector3 plane2Normal, Vector3 plane2Position)
    {

        linePoint = Vector3.zero;
        lineVec = Vector3.zero;

        //We can get the direction of the line of intersection of the two planes by calculating the 
        //cross product of the normals of the two planes. Note that this is just a direction and the line
        //is not fixed in space yet. We need a point for that to go with the line vector.
        lineVec = Vector3.Cross(plane1Normal, plane2Normal);

        //Next is to calculate a point on the line to fix it's position in space. This is done by finding a vector from
        //the plane2 location, moving parallel to it's plane, and intersecting plane1. To prevent rounding
        //errors, this vector also has to be perpendicular to lineDirection. To get this vector, calculate
        //the cross product of the normal of plane2 and the lineDirection.		
        Vector3 ldir = Vector3.Cross(plane2Normal, lineVec);

        float denominator = Vector3.Dot(plane1Normal, ldir);

        //Prevent divide by zero and rounding errors by requiring about 5 degrees angle between the planes.
        if (Mathf.Abs(denominator) > 0.006f)
        {

            Vector3 plane1ToPlane2 = plane1Position - plane2Position;
            float t = Vector3.Dot(plane1Normal, plane1ToPlane2) / denominator;
            linePoint = plane2Position + t * ldir;

            return true;
        }

        //output not valid
        else
        {
            return false;
        }
    }

    //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
    //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
    //same plane, use ClosestPointsOnTwoLines() instead.
    public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {

        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parrallel
        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineVec1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }
}

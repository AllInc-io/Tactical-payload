using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DrawController : Controller
{

    [SerializeField] private LayerMask ground;
    [SerializeField] private LayerMask obstaclesOnly;
    [SerializeField] private LayerMask groundAndObstacles;



    [SerializeField] private LayerMask characters;
    [SerializeField] private string canWalkTag;

    [Title("Settings")]
    [SerializeField] float minDistanceBetweenPoints = 0.3f;
    [SerializeField] float maxDistanceBetweenPoints = 0.6f;
    [SerializeField] float maxSlope = 45f;
    [SerializeField] bool iWantControlDebugLogs = false;
    [SerializeField] float minObstacleHeight = 0.2f;
    [SerializeField] float selectionSphereRadius = 1f;
    [SerializeField] float slowMoWhenDrawing = 0.03f;

    [Title("Temp")]
    //[SerializeField] Shapes.Polyline line;
    [SerializeField] DrawnLine line;
    /*[SerializeField] Color lineColor;
    [SerializeField] float lineThickness = 0.2f;*/

    Vector3 currentPoint;
    List<Vector3> nextPoints;
    bool isDrawing = false;
    Hero currentlySelectedCharacter;

    [HideInInspector] public bool deleteLine;

    bool wasStuckBehindWall = false;
    bool wasTooCloseToWall = false;


    public bool inForcedDraw;
    public Hero forcedHero;
    public Vector3 forcedDestination;
    public float forcedDestinationMargin;
    
    public override void Init()
    {
        base.Init();
        nextPoints = new List<Vector3>();
        currentPoint = Vector3.zero;

        R.get.ui.menuIngame.deleteDrawingButton.Hide();

    }

    private void Update()
    {

        if (!isOn || !R.get.game.isOn || Time.timeScale == 0) return;
         float characterWidth = currentlySelectedCharacter != null ? currentlySelectedCharacter.transform.lossyScale.z * 0.5f * (currentlySelectedCharacter.col as CapsuleCollider).radius : 0;
        if (Input.GetMouseButtonDown(0))
        {
            TryStartDrawing();
        }
        if(Input.GetMouseButton(0) && isDrawing)
        {
            //if (iWantControlDebugLogs) Debug.Log("New frame starting ! Last position is : " + nextPoints[nextPoints.Count - 1]);
            bool didSomething = false;
            Ray ray = R.get.mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground, QueryTriggerInteraction.UseGlobal) && Vector3.Angle(Vector3.up, hit.normal) < maxSlope)
            {
                currentPoint = hit.point;


                if (hit.collider.CompareTag(canWalkTag))
                {

                    if (nextPoints.Count == 0 ||
                    CheckIfCanPassBetweenTwoPoints(nextPoints[nextPoints.Count - 1] + Vector3.up * minObstacleHeight, currentPoint, characterWidth, out RaycastHit hitPoint))
                    {

                        if (((Vector3.Distance(nextPoints[nextPoints.Count - 1], currentPoint) >= minDistanceBetweenPoints || wasStuckBehindWall || wasTooCloseToWall)))
                            {
                            if ((wasStuckBehindWall || wasTooCloseToWall) && nextPoints.Count > 1)
                            {
                                if (Vector3.Distance(nextPoints[nextPoints.Count - 2], nextPoints[nextPoints.Count - 1]) >= maxDistanceBetweenPoints)
                                {
                                    //add multiple points between the points
                                    int pointsAmount = Mathf.CeilToInt(Vector3.Distance(nextPoints[nextPoints.Count - 2], nextPoints[nextPoints.Count - 1]) / maxDistanceBetweenPoints);


                                    Vector3 point1 = nextPoints[nextPoints.Count - 2];
                                    Vector3 point2 = nextPoints[nextPoints.Count - 1];

                                    for (int i = 1; i <= pointsAmount; i++)
                                    {
                                        float t = (float)(i) / pointsAmount;
                                        Vector3 intermediatePos = point1 + (point2 - point1) * t;
                                        if (i == 1)
                                        {
                                            nextPoints[nextPoints.Count - 1] = intermediatePos;
                                            line.MoveLastPoint(intermediatePos);
                                        }
                                        else
                                        {
                                            nextPoints.Add(intermediatePos);
                                            line.AddPoint(intermediatePos, false);
                                        }
                                        //Debug.Log("1 - Adding point " + intermediatePos);
                                    }

                                }
                                if (iWantControlDebugLogs) Debug.Log("Adding this point : " + nextPoints[nextPoints.Count - 1] + " to have a proper corner");
                                nextPoints.Add(nextPoints[nextPoints.Count - 1]);
                                //line.AddPoint(nextPoints[nextPoints.Count - 1], false);
                                //Debug.Log("2 - Adding point " + nextPoints[nextPoints.Count - 1]);
                                /*else
                                {
                                    nextPoints.Add(nextPoints[nextPoints.Count - 1]);
                                    line.AddPoint(nextPoints[nextPoints.Count - 1], false);
                                    Debug.Log("2 - Adding point " + nextPoints[nextPoints.Count - 1]);
                                }*/

                            }


                            if (Vector3.Distance(nextPoints[nextPoints.Count - 1], currentPoint) > maxDistanceBetweenPoints)
                            {
                                //add multiple points between the points
                                int pointsAmount = Mathf.CeilToInt(Vector3.Distance(nextPoints[nextPoints.Count - 1], currentPoint) / maxDistanceBetweenPoints);

                                for (int i = 0; i < pointsAmount; i++)
                                {
                                    float t = (float)(i + 1) / pointsAmount;
                                    Vector3 intermediatePos = nextPoints[nextPoints.Count - 1] + (currentPoint - nextPoints[nextPoints.Count - 1]) * t;
                                    nextPoints.Add(intermediatePos);
                                    line.AddPoint(intermediatePos, false);
                                    //Debug.Log("3 - Adding point " + intermediatePos);
                                }
                            }
                            else
                            {
                                nextPoints.Add(currentPoint);
                                line.AddPoint(currentPoint, !(wasStuckBehindWall || wasTooCloseToWall));
                                //Debug.Log("4 - Adding point " + currentPoint);
                            }


                        }

                        currentlySelectedCharacter.visionCircle.transform.position = currentPoint + Vector3.up * 0.1f;
                        line.MoveLastPoint(currentPoint);
                        wasStuckBehindWall = false;
                        wasTooCloseToWall = false;
                        didSomething = true;

                    }

                }
                
                if (!didSomething)
                {
                    if (iWantControlDebugLogs) Debug.Log("Was blocked by something");

                    wasStuckBehindWall = true;

                    Collider[] results = Physics.OverlapCapsule(hit.point + (characterWidth + minObstacleHeight) * Vector3.up, hit.point + (characterWidth + minObstacleHeight * 2f) * Vector3.up, characterWidth, groundAndObstacles);



                    
                    Plane plane = new Plane(Vector3.up, nextPoints[nextPoints.Count >= 2 ? nextPoints.Count - 2 : nextPoints.Count - 1]);
                    plane.Raycast(ray, out float enter);
                    Vector3 goal = R.get.mainCamera.transform.position + ray.direction * enter;

                    if (results.Length > 0) wasTooCloseToWall = true;
                    Vector3 toCheckPoint = (results.Length > 0 && results[0].GetType() != typeof(MeshCollider)) ? results[0].ClosestPoint(nextPoints[nextPoints.Count - 1]) : goal;

                    if (!CheckIfCanPassBetweenTwoPoints(nextPoints[nextPoints.Count - 1] + Vector3.up * minObstacleHeight, toCheckPoint, characterWidth, out RaycastHit onTheWayHit))
                    //if (Physics.Raycast(previousToGoal, out RaycastHit onTheWayHit, Vector3.Distance(nextPoints[nextPoints.Count - 1], hit.point), obstaclesOnly, QueryTriggerInteraction.Collide))   
                    {


                        //alt way because the other one needs an obstacle, and now sometimes it's just that there is no ground 

                        Vector3 idealDir = goal - nextPoints[nextPoints.Count - 1];
                        idealDir.y = 0;

                        //perpendicular to the surface it's hitting
                        Vector3 perpendicularDir = new Vector3(-idealDir.z, 0, idealDir.x);
                        if (Vector3.Angle(goal - nextPoints[nextPoints.Count - 1], perpendicularDir) > 90) perpendicularDir = -perpendicularDir;


                        dir1 = idealDir;
                        dir2 = perpendicularDir;

                        //tries many points until it finds one that works
                        int i = 0;

                        int precision = 30;
                        bool found = false;
                        Vector3 point = nextPoints[nextPoints.Count - 1];
                        while (i < precision && !found)
                        {
                            Vector3 newDir = Vector3.Lerp(idealDir, perpendicularDir, (float)i / precision);

                            //if (Vector3.Angle(goal - nextPoints[nextPoints.Count - 1], newDir) > 90) newDir = -newDir;


                            if (LineLineIntersection(out point, new Vector3(goal.x, nextPoints[nextPoints.Count - 1].y, goal.z), -new Vector3(-newDir.z, 0, newDir.x), nextPoints[nextPoints.Count - 1], new Vector3(newDir.x, 0, newDir.z)))
                            {
                                pointToGizmo = point;
                                if (CheckIfCanPassBetweenTwoPoints(nextPoints[nextPoints.Count - 1], point, characterWidth, out RaycastHit pointHit)) found = true;

                            }


                            i++;
                        }

                        if (!found)
                        {
                            i = 0;
                            perpendicularDir = -perpendicularDir;
                            dir2 = perpendicularDir;
                            while (i < precision && !found)
                            {
                                Vector3 newDir = Vector3.Lerp(idealDir, perpendicularDir, (float)i / precision);

                                //if (Vector3.Angle(goal - nextPoints[nextPoints.Count - 1], newDir) > 90) newDir = -newDir;


                                if (LineLineIntersection(out point, new Vector3(goal.x, nextPoints[nextPoints.Count - 1].y, goal.z), -new Vector3(newDir.z, 0, -newDir.x), nextPoints[nextPoints.Count - 1], new Vector3(newDir.x, 0, newDir.z)))
                                {
                                    pointToGizmo = point;
                                    if (CheckIfCanPassBetweenTwoPoints(nextPoints[nextPoints.Count - 1] + Vector3.up * minObstacleHeight, point, characterWidth, out RaycastHit pointHit)) found = true;

                                }
                                /*if (LineLineIntersection(out point, new Vector3(goal.x, nextPoints[nextPoints.Count - 1].y, goal.z), -new Vector3(newDir.z, 0, -newDir.x), nextPoints[nextPoints.Count - 1], new Vector3(newDir.x, 0, newDir.z)))
                                {
                                    pointToGizmo = point;
                                    if (CheckIfCanPassBetweenTwoPoints(nextPoints[nextPoints.Count - 1] + Vector3.up * minObstacleHeight, point, characterWidth, out RaycastHit pointHit)) found = true;

                                }*/


                                i++;
                            }
                        }




                        Vector3 previousPos = nextPoints[nextPoints.Count - 1];
                        Collider[] nouveauxResults = Physics.OverlapCapsule(point + (characterWidth + minObstacleHeight) * Vector3.up, point + (characterWidth + minObstacleHeight * 2f) * Vector3.up, characterWidth, groundAndObstacles);
                        if (nouveauxResults.Length == 0 && found)
                        {

                            if (!wasStuckBehindWall)
                            {
                                Vector3 pointToAdd = (results.Length > 0 ? toCheckPoint : nextPoints[nextPoints.Count - 1]);
                                nextPoints.Add(point);
                                line.MoveLastPoint(point);
                                line.AddPoint(point, false);
                                nextPoints[nextPoints.Count - 1] = point;

                                if (iWantControlDebugLogs) Debug.Log("5 - Adding point " + point);
                            }

                            
                            nextPoints[nextPoints.Count - 1] = point;
                            line.MoveLastPoint(point);
                            currentlySelectedCharacter.visionCircle.transform.position = point + Vector3.up * 0.1f;
                            if (iWantControlDebugLogs) Debug.Log("Couldn't go where wanted so moved last point to :" + point);

                            //silly check because it's buggy and that is what i would call a massive bandaid 
                            if (nextPoints.Count > 2 && !CheckIfCanPassBetweenTwoPoints(nextPoints[nextPoints.Count - 1] + Vector3.up * minObstacleHeight, nextPoints[nextPoints.Count - 2] + Vector3.up * minObstacleHeight, characterWidth, out RaycastHit hitPoint))
                            {
                                nextPoints[nextPoints.Count - 1] = previousPos;
                                line.MoveLastPoint(previousPos);
                                currentlySelectedCharacter.visionCircle.transform.position = previousPos + Vector3.up * 0.1f;
                                if (iWantControlDebugLogs) Debug.Log("that point was impossible so cancel");
                            }

                        }
                        


                        #region deprec
                        /*
                                                }

                                                else

                                                {
                                                    //trying not to use this anymore
                                                    return;


                                                    //perpendicular to the surface it's hitting
                                                    Vector3 dir = new Vector3(-onTheWayHit.normal.z, 0, onTheWayHit.normal.x);
                                                    if (Vector3.Angle(goal - nextPoints[nextPoints.Count - 1], dir) > 90) dir = -dir;

                                                    if (!wasStuckBehindWall)
                                                    {
                                                        Vector3 pointToAdd = (results.Length > 0 ? toCheckPoint : nextPoints[nextPoints.Count - 1]) + onTheWayHit.normal * characterWidth;
                                                        nextPoints.Add(pointToAdd);
                                                        line.MoveLastPoint(pointToAdd);
                                                        line.AddPoint(pointToAdd, false);
                                                        nextPoints[nextPoints.Count - 1] = pointToAdd;

                                                        Debug.Log("5 - Adding point " + pointToAdd);
                                                    }

                                                    if (LineLineIntersection(out Vector3 point, new Vector3(goal.x, nextPoints[nextPoints.Count - 1].y, goal.z), -new Vector3(onTheWayHit.normal.x, 0, onTheWayHit.normal.z), nextPoints[nextPoints.Count - 1], new Vector3(dir.x, 0, dir.z)))
                                                    {
                                                        Collider[] nouveauxResults = Physics.OverlapSphere(point, characterWidth * 0.9f, obstaclesOnly);
                                                        if (nouveauxResults.Length <= 0)
                                                        {
                                                            if (CheckIfCanPassBetweenTwoPoints(nextPoints[nextPoints.Count - 1] + Vector3.up * verticalOffsetToDetectObstacles, point, characterWidth, out RaycastHit ifCantPass))
                                                            {
                                                                nextPoints[nextPoints.Count - 1] = point;
                                                                line.MoveLastPoint(point);
                                                                currentlySelectedCharacter.visionCircle.transform.position = point;

                                                            }
                                                        }
                                                        else
                                                        {
                                                            pointToGizmo = nouveauxResults[0].transform.position;
                                                        }

                                                    }

                                                }*/


                        /*
                        //perpendicular to the surface it's hitting
                        Vector3 dir = new Vector3(-onTheWayHit.normal.z, 0, onTheWayHit.normal.x);
                        if (Vector3.Angle(goal - nextPoints[nextPoints.Count - 1], dir) > 90) dir = -dir;

                        if (!wasStuckBehindWall)
                        {
                            Vector3 pointToAdd = (results.Length > 0 ? toCheckPoint : nextPoints[nextPoints.Count - 1]) + onTheWayHit.normal * characterWidth;
                            nextPoints.Add(pointToAdd);
                            line.MoveLastPoint(pointToAdd);
                            line.AddPoint(pointToAdd, false);
                            nextPoints[nextPoints.Count - 1] = pointToAdd;

                            Debug.Log("5 - Adding point " + pointToAdd);
                        }

                        if (LineLineIntersection(out Vector3 point, new Vector3(goal.x, nextPoints[nextPoints.Count - 1].y, goal.z), -new Vector3(onTheWayHit.normal.x, 0, onTheWayHit.normal.z), nextPoints[nextPoints.Count - 1], new Vector3(dir.x, 0, dir.z)))
                        {
                            Collider[] nouveauxResults = Physics.OverlapSphere(point, characterWidth * 0.9f, obstaclesOnly);
                            if (nouveauxResults.Length <= 0)
                            {
                                if(CheckIfCanPassBetweenTwoPoints(nextPoints[nextPoints.Count - 1] + Vector3.up * verticalOffsetToDetectObstacles, point, characterWidth, out RaycastHit ifCantPass))
                                {
                                    nextPoints[nextPoints.Count - 1] = point;
                                    line.MoveLastPoint(point);
                                    currentlySelectedCharacter.visionCircle.transform.position = point;

                                }
                            }
                            else
                            {
                                pointToGizmo = nouveauxResults[0].transform.position;
                            }

                        }

                        wasStuckBehindWall = true;
                        */
                        #endregion

                    }
                    else if (results.Length > 0)
                    {
                        if (iWantControlDebugLogs) Debug.Log("There was something in the way, " + results[0].name);
                        /*
                        if (Vector3.Distance(nextPoints[nextPoints.Count - 1], currentPoint) > maxDistanceBetweenPoints)
                        {
                            //add multiple points between the points
                            int pointsAmount = Mathf.CeilToInt(Vector3.Distance(nextPoints[nextPoints.Count - 1], currentPoint) / maxDistanceBetweenPoints);

                            for (int i = 0; i < pointsAmount; i++)
                            {
                                float t = (float)(i + 1) / pointsAmount;
                                Vector3 intermediatePos = nextPoints[nextPoints.Count - 1] + (currentPoint - nextPoints[nextPoints.Count - 1]) * t;
                                nextPoints.Add(intermediatePos);
                                line.AddPoint(intermediatePos, false);
                                //Debug.Log("3 - Adding point " + intermediatePos);
                            }
                        }
                        else
                        {
                            nextPoints.Add(currentPoint);
                            line.AddPoint(currentPoint, !(wasStuckBehindWall || wasTooCloseToWall));
                            //Debug.Log("4 - Adding point " + currentPoint);
                        }*/
                    }
                   
                }
                
            }

            /*line.positionCount = nextPoints.Count;
            line.SetPositions(nextPoints.ToArray());*/
        }
        if(Input.GetMouseButtonUp(0) && isDrawing)
        {

            if (deleteLine) GiveUpDrawing();
            else
            {
                if (inForcedDraw && Vector3.Distance(nextPoints[nextPoints.Count - 1], forcedDestination) >= forcedDestinationMargin) GiveUpDrawing();
                else
                {
                    if(inForcedDraw)
                    {
                        R.get.ui.menuIngame.HidePointer();
                        inForcedDraw = false;
                        
                    }

                    if (nextPoints.Count > 2 && !CheckIfCanPassBetweenTwoPoints(nextPoints[nextPoints.Count - 1] + Vector3.up * minObstacleHeight, currentPoint + Vector3.up * minObstacleHeight, characterWidth, out RaycastHit hitPoint))
                    {
                        nextPoints[nextPoints.Count - 1] = nextPoints[nextPoints.Count - 2];
                        line.MoveLastPoint(nextPoints[nextPoints.Count - 2]);
                    }
                    else if (nextPoints.Count >= 1)
                    {
                        if (!CheckIfCanPassBetweenTwoPoints(nextPoints[nextPoints.Count - 1] + Vector3.up * minObstacleHeight, currentPoint + Vector3.up * minObstacleHeight, characterWidth, out hitPoint))
                        {
                            GiveUpDrawing();
                        }
                    }
                    else if (wasStuckBehindWall)
                    {
                        Vector3 previousPoint = nextPoints[nextPoints.Count - 1];
                        if (nextPoints.Count >= 2 && Vector3.Distance(nextPoints[nextPoints.Count - 2], previousPoint) > maxDistanceBetweenPoints)
                        {
                            //add multiple points between the points
                            int pointsAmount = Mathf.CeilToInt(Vector3.Distance(nextPoints[nextPoints.Count - 2], previousPoint) / maxDistanceBetweenPoints);
                            Debug.Log(pointsAmount);
                            for (int i = 0; i < pointsAmount; i++)
                            {

                                float t = (float)(i + 1) / pointsAmount;
                                Vector3 intermediatePos = nextPoints[nextPoints.Count - 2] + (previousPoint - nextPoints[nextPoints.Count - 2]) * t;
                                if (i == 0) nextPoints[nextPoints.Count - 1] = intermediatePos;
                                else nextPoints.Add(intermediatePos);
                                line.AddPoint(intermediatePos, false);
                            }
                        }
                    }
                    R.get.haptics.Haptic(HapticForce.Medium);
                    if (nextPoints.Count > 0) currentlySelectedCharacter.SetPath(line.GetPoints());
                }
                
            }


            deleteLine = false;
            //currentlySelectedCharacter.visionCircle.transform.localPosition = Vector3.zero + Vector3.up * 0.1f;
            line.gameObject.SetActive(false);
            isDrawing = false;

            Time.timeScale = 1f;


            wasStuckBehindWall = false;
            wasTooCloseToWall = false;

            R.get.ui.menuIngame.deleteDrawingButton.Hide();
        }
    }


    bool throwingBombMode;

    Vector3 pointToGizmo;

    Vector3 dir1;
    Vector3 dir2;
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pointToGizmo, 0.2f);


        if (nextPoints == null || nextPoints.Count < 2) return;
        Gizmos.DrawLine(nextPoints[nextPoints.Count - 1], nextPoints[nextPoints.Count - 1] + dir1);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(nextPoints[nextPoints.Count - 1], nextPoints[nextPoints.Count - 1] + dir2);

 
    }

    void TryStartDrawing()
    {

        Ray ray = R.get.mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] results = Physics.SphereCastAll(ray, selectionSphereRadius, Mathf.Infinity, characters, QueryTriggerInteraction.UseGlobal);
        if (results.Length > 0)
        {
            if(Physics.Raycast(ray,out RaycastHit centerHit, Mathf.Infinity, ground, QueryTriggerInteraction.UseGlobal))
            {
                RaycastHit closestHit = results[0];
                foreach (RaycastHit hit in results)
                {
                    if (Vector3.Distance(hit.point, centerHit.point) < Vector3.Distance(closestHit.point, centerHit.point)) closestHit = hit;
                }

                currentlySelectedCharacter = closestHit.collider.GetComponent<Hero>();
                if (inForcedDraw && currentlySelectedCharacter != forcedHero)
                {
                    currentlySelectedCharacter = null;
                    return;
                }

                R.get.haptics.Haptic(HapticForce.Selection);

                isDrawing = true;

                nextPoints.Clear();

                line.Init();


                line.gameObject.SetActive(true);

                line.AddPoint(closestHit.collider.transform.position);


                line.SetColor(currentlySelectedCharacter.lineColor);


                nextPoints.Add(currentlySelectedCharacter.transform.position);
                currentlySelectedCharacter.StopMoving();

                currentlySelectedCharacter.visionCircle.gameObject.SetActive(true);

                R.get.ui.menuIngame.deleteDrawingButton.Show();
                Time.timeScale = slowMoWhenDrawing;
            }
            
        }
    }

    public void SetForcedDraw(Hero forcedHero, Vector3 destination, float margin)
    {
        inForcedDraw = true;
        this.forcedHero = forcedHero;
        forcedDestination = destination;
        forcedDestinationMargin = margin;
    }

    public bool CheckIfCanPassBetweenTwoPoints(Vector3 point1, Vector3 point2, float charaWidth, out RaycastHit hitPoint)
    {

        hitPoint = new RaycastHit();
        hitPoint.point = point1;

        /*if (Mathf.Abs(point2.y - point1.y) >= maxDistanceBetweenPoints)
        {
            if(iWantControlDebugLogs) Debug.Log("Too high of a diff between points" + Mathf.Abs(point2.y - point1.y));
            return false;
        }*/


        float angle = GetSlopeBetweenPoints(point1, point2);

        if (Mathf.Abs(angle) > maxSlope)
        {
            if(iWantControlDebugLogs) Debug.Log("Slope is too high : " + angle);
            return false;
        }
        else if(point1.y != point2.y)
        {
            //check that there is no big drop i guess
            //to do

            int pointsAmount = Mathf.CeilToInt(Vector3.Distance(point1, point2) / maxDistanceBetweenPoints);
            Vector3[] pointsToCheck = new Vector3[pointsAmount];


            for (int i = 0; i < pointsAmount; i++)
            {

                float t = (float)(i + 1) / pointsAmount;
                Vector3 intermediatePos = point1 + (point2 - point1) * t;
                if (Physics.Raycast(intermediatePos + Vector3.up, Vector3.down, out RaycastHit slopeCheckHitPoint, 1f + minObstacleHeight, groundAndObstacles)) pointsToCheck[i] = slopeCheckHitPoint.point;
                else return false;
            }

            for(int j = 0; j < pointsAmount - 1; j++)
            {
                if (Mathf.Abs(GetSlopeBetweenPoints(pointsToCheck[j], pointsToCheck[j + 1])) > maxSlope) return false;
            }
        }

        //if the height difference is too big 
        //should actually be based on distance and a max slope etc but. one thing at a time

        if (!Physics.Raycast(point2 + Vector3.up, Vector3.down, 1 + minObstacleHeight, ground, QueryTriggerInteraction.Collide)) return false;
        //check if there is actually floor where the point wants to go

        Collider[] results = Physics.OverlapCapsule(point2 + (charaWidth + minObstacleHeight) * Vector3.up, point2 + (charaWidth + minObstacleHeight ) * 2f  * Vector3.up, charaWidth, groundAndObstacles);
        if (!Physics.Raycast(point2, point1 - point2, out hitPoint, Vector3.Distance(point2, point1), groundAndObstacles) &&
        results.Length == 0)
        {
            return true;
        }
        else
        {
            if (iWantControlDebugLogs) Debug.Log("Point was : " + point2 + " .Blocked by " + results.Length + " obstacles " + (results.Length > 0 ? results[0].name : hitPoint.collider.name));
            return false;
        }
    }

    public float GetSlopeBetweenPoints(Vector3 point1, Vector3 point2)
    {
        Vector3 dirBetweenPoints = point2 - point1;
        return Vector3.Angle(new Vector3(dirBetweenPoints.x, 0, dirBetweenPoints.z), dirBetweenPoints);
    }

    
    public void GiveUpDrawing()
    {

        isDrawing = false;

        nextPoints.Clear();

        line.Init();


        line.gameObject.SetActive(false);

        if (currentlySelectedCharacter != null)
        {
            currentlySelectedCharacter.StopMoving();
            currentlySelectedCharacter.visionCircle.gameObject.SetActive(false);
            currentlySelectedCharacter = null;
        }



        R.get.haptics.Haptic(HapticForce.Failure);

        

        Time.timeScale = 1f;
    }

    /// <summary>
    /// Calculates the intersection of two given lines
    /// </summary>
    /// <param name="intersection">returned intersection</param>
    /// <param name="linePoint1">start location of the line 1</param>
    /// <param name="lineDirection1">direction of line 1</param>
    /// <param name="linePoint2">start location of the line 2</param>
    /// <param name="lineDirection2">direction of line2</param>
    /// <returns>true: lines intersect, false: lines do not intersect</returns>

    public static bool LineLineIntersection(out Vector3 intersection,
        Vector3 linePoint1, Vector3 lineDirection1,
        Vector3 linePoint2, Vector3 lineDirection2)
    {

        /// source: https://forum.unity.com/threads/is-there-a-way-to-check-whether-two-vectors-intersect.69637/
        /// 
        Vector3 lineVec3 = linePoint2 - linePoint1;
        Vector3 crossVec1and2 = Vector3.Cross(lineDirection1, lineDirection2);
        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineDirection2);
        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

        //is coplanar, and not parallel
        if (Mathf.Abs(planarFactor) < 0.0001f
                && crossVec1and2.sqrMagnitude > 0.0001f)
        {
            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
            intersection = linePoint1 + (lineDirection1 * s);
            return true;
        }
        else
        {
            intersection = Vector3.zero;
            return false;
        }
    }
    
}

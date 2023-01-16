using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using DG.Tweening;
using Sirenix.OdinInspector;

public class DrawnLine : MonoBehaviour
{
    [SerializeField] Polyline line;
    [SerializeField] int detailLevel = 5;

    List<Vector3> controlPoints;

    [SerializeField] SpriteRenderer onEndPoint;

    [SerializeField] float dashesSize = 0.5f;
    public void Init()
    {
        controlPoints = new List<Vector3>();
        line.points.Clear();
        line.meshOutOfDate = true;

        onEndPoint.color = line.Color;
    }

    public void AddPoint(Vector3 pos, bool doBezier = true)
    {
        doBezier = false; //TEMP TEST

        if (line.points.Count > 0)
        {
            line.points.RemoveAt(line.points.Count - 1);
        }

        Vector3 dir;
        if (line.points.Count > 1)
        {
            if (line.points.Count == 2) dir = (controlPoints[controlPoints.Count - 1] - controlPoints[controlPoints.Count - 2]).normalized;
            else if (doBezier)
            {
                dir = ((controlPoints[controlPoints.Count - 1] - controlPoints[controlPoints.Count - 2]).normalized + (controlPoints[controlPoints.Count - 1] - controlPoints[controlPoints.Count - 3]).normalized) / 2f;
                Vector3 pos2 = controlPoints[controlPoints.Count - 1] + dir * (Vector3.Distance(controlPoints[controlPoints.Count - 1], pos) / 2f);
                controlPoints.Add(pos2);
            }
        }

        controlPoints.Add(pos);

        if (controlPoints.Count <= 2) line.AddPoint(pos);
        else if(doBezier)
        {
            int pointCount = controlPoints.Count;
            //quadratic bezier curve as said by https://www.gamedeveloper.com/business/how-to-work-with-bezier-curve-in-games-with-unity
            //B(t) = (1-t)2P0 + 2(1-t)tP1 + t2P2 , 0 < t < 1

            Vector3 p0 = controlPoints[pointCount - 3];
            Vector3 p1 = controlPoints[pointCount - 2];
            Vector3 p2 = controlPoints[pointCount - 1];

            
            for (int i = 0; i < detailLevel; i++)
            {
                float t = (float)i / detailLevel;
                Vector3 bezierPos = Mathf.Pow(1f - t, 2f) * p0 + 2 * (1f - t) * t * p1 + Mathf.Pow(t, 2f) * p2;
                line.AddPoint(bezierPos);
            }
        }
        else
        {
            
            Vector3 p1 = controlPoints[controlPoints.Count - 2];
            Vector3 p2 = controlPoints[controlPoints.Count - 1];

            dir = (p2 - p1);

            for (int i = 0; i < detailLevel; i++)
            {
                float t = (float)i / detailLevel;
                Vector3 intermediatePos = p1 + dir * t;
                line.AddPoint(intermediatePos);
            }
        }
        line.AddPoint(pos);

    }

    public void InsertPoint(int index, Vector3 pos)
    {
        controlPoints.Insert(index, pos);

        line.points.Insert(index, new PolylinePoint(pos));
        line.meshOutOfDate = true;
    }

    public void RemovePoint(int index)
    {
        if(controlPoints.Count > index + 1) controlPoints.RemoveAt(index);

        if (line.points.Count > index + 1)
        {
            line.points.RemoveAt(index);
            line.meshOutOfDate = true;
        }
    }

    public void MoveLastPoint(Vector3 pos)
    {
        //controlPoints[controlPoints.Count - 1] = pos;
        line.points[line.points.Count - 1] = new PolylinePoint(pos);
        line.meshOutOfDate = true;
        onEndPoint.transform.position = pos + Vector3.up * 0.1f;

    }

    public void MoveFirstPoint(Vector3 pos)
    {
        controlPoints[0] = pos;

        line.points[0] = new PolylinePoint(pos);
        line.meshOutOfDate = true;
    }

    public Vector3 GetPoint(int index)
    {
        return controlPoints[index];
    }

    public List<Vector3> GetPoints()
    {
        List<Vector3> allPoints = new List<Vector3>();
        foreach(PolylinePoint point in line.points)
        {  
            allPoints.Add(point.point);
        }
        return allPoints;
    }

    public void SetPoints(List<Vector3> points, bool pointilles = true)
    {
        controlPoints.Clear();
        controlPoints.AddRange(points);

        line.points.Clear();

        float pointilleEvery = dashesSize;
        float pointilleDistance = 0;
        foreach (Vector3 controlPoint in controlPoints)
        {
            if (pointilleDistance < pointilleEvery || !pointilles) line.AddPoint(controlPoint); else line.AddPoint(controlPoint, default, 0);
            if(line.Count > 2) pointilleDistance += Vector3.Distance(line.points[line.Count - 2].point, line.points[line.Count - 1].point);
            if (pointilleDistance >= pointilleEvery * 2f) pointilleDistance = 0;
        }
        line.meshOutOfDate = true;

        onEndPoint.transform.position = line.points[line.Count - 1].point + Vector3.up * 0.1f;
        onEndPoint.color = line.Color;

    }

    public void SetColor(Color color)
    {
        line.Color = color;
        onEndPoint.color = color;
    }

    public void SetThickness(float thickness)
    {
        line.Thickness = thickness;
    }

}

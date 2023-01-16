using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class WallWithLines : MonoBehaviour
{
    [SerializeField] Transform mesh;

    [SerializeField] float distanceLine = 0.5f;


    [SerializeField] Transform leftLine;
    [SerializeField] Transform rightLine;
    [SerializeField] Transform upLine;
    [SerializeField] Transform downLine;

    public void SetWidth(float scale)
    {
        mesh.localScale = new Vector3( scale, mesh.localScale.y, mesh.localScale.z);

        leftLine.localPosition = new Vector3(-(scale + leftLine.transform.localScale.x) / 2f - distanceLine, 0 ,0);
        leftLine.localScale = new Vector3(leftLine.localScale.x, mesh.localScale.z + distanceLine * 2f + leftLine.transform.localScale.x / 2f, leftLine.localScale.y);

        rightLine.localPosition = new Vector3((scale  + rightLine.transform.localScale.x) / 2f + distanceLine, 0 ,0);
        rightLine.localScale = new Vector3(rightLine.localScale.x, mesh.localScale.z + distanceLine * 2f + rightLine.transform.localScale.x / 2f, rightLine.localScale.y);

        upLine.localPosition = new Vector3(0 , 0, mesh.localScale.z / 2f + distanceLine);
        upLine.localScale = new Vector3(scale + distanceLine * 2f, upLine.localScale.y, upLine.localScale.z);

        downLine.localPosition = new Vector3(0, 0, -mesh.localScale.z / 2f - distanceLine);
        downLine.localScale = new Vector3(scale + distanceLine * 2f, upLine.localScale.y, upLine.localScale.z);

    }

    public void SetDepth(float scale)
    {
        mesh.localScale = new Vector3(mesh.localScale.x, mesh.localScale.y, scale);

        leftLine.localPosition = new Vector3(-(scale + leftLine.transform.localScale.x) / 2f - distanceLine, 0, 0);
        leftLine.localScale = new Vector3(leftLine.localScale.x, mesh.localScale.z + distanceLine * 2f + leftLine.transform.localScale.x / 2f, leftLine.localScale.y);

        rightLine.localPosition = new Vector3((scale + rightLine.transform.localScale.x) / 2f + distanceLine, 0, 0);
        rightLine.localScale = new Vector3(rightLine.localScale.x, mesh.localScale.z + distanceLine * 2f + rightLine.transform.localScale.x / 2f, rightLine.localScale.y);

        upLine.localPosition = new Vector3(0, 0, mesh.localScale.z / 2f + distanceLine);
        upLine.localScale = new Vector3(scale + distanceLine * 2f, upLine.localScale.y, upLine.localScale.z);

        downLine.localPosition = new Vector3(0, 0, -mesh.localScale.z / 2f - distanceLine);
        downLine.localScale = new Vector3(scale + distanceLine * 2f, upLine.localScale.y, upLine.localScale.z);
    }

    [Button]
    private void CleanScale()
    {
        Vector3 finalScale = mesh.transform.lossyScale;

        transform.localScale = Vector3.one;
        mesh.transform.localScale = finalScale;


        leftLine.localPosition = new Vector3(-(finalScale.x + leftLine.transform.localScale.x) / 2f - distanceLine, 0, 0);
        leftLine.localScale = new Vector3(leftLine.localScale.x, mesh.localScale.z + distanceLine * 2f + leftLine.transform.localScale.x / 2f, leftLine.localScale.y);

        rightLine.localPosition = new Vector3((finalScale.x + rightLine.transform.localScale.x) / 2f + distanceLine, 0, 0);
        rightLine.localScale = new Vector3(rightLine.localScale.x, mesh.localScale.z + distanceLine * 2f + rightLine.transform.localScale.x / 2f, rightLine.localScale.y);

        upLine.localPosition = new Vector3(0, 0, mesh.localScale.z / 2f + distanceLine);
        upLine.localScale = new Vector3(finalScale.x + distanceLine * 2f, upLine.localScale.y, upLine.localScale.z);

        downLine.localPosition = new Vector3(0, 0, -mesh.localScale.z / 2f - distanceLine);
        downLine.localScale = new Vector3(finalScale.x + distanceLine * 2f, upLine.localScale.y, upLine.localScale.z);

    }

}

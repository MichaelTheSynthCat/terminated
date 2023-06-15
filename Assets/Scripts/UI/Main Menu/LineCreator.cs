using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineCreator : MonoBehaviour
{
    public List<GameObject> points;
    private LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = points.Count + 1;
        lineRenderer.SetPosition(0, gameObject.transform.position);
        for(int i=0; i<points.Count; i++)
        {
            lineRenderer.SetPosition(i+1, points[i].GetComponent<RectTransform>().position);
        }
    }
}

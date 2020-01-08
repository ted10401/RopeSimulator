using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[ExecuteInEditMode]
public class RopeLineRenderer : MonoBehaviour
{
    public Transform startTransform;
    public Transform endTransform;

    private LineRenderer m_lineRenderer;
    private Vector3[] m_positions;

    private void Awake()
    {
        m_lineRenderer = GetComponent<LineRenderer>();
        m_lineRenderer.positionCount = 2;
        m_positions = new Vector3[2];
    }

    private void Update()
    {
        UpdatePositions();
    }

    private void UpdatePositions()
    {
        if(m_positions[0] == startTransform.position &&
            m_positions[1] == endTransform.position)
        {
            return;
        }

        m_positions[0] = startTransform.position;
        m_positions[1] = endTransform.position;
        m_lineRenderer.SetPositions(m_positions);
    }
}

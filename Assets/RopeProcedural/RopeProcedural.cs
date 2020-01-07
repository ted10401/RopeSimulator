using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class RopeProcedural : MonoBehaviour
{
    [SerializeField] private bool m_updateEveryFrame = true;
    [SerializeField] private Transform m_startTransform;
    [SerializeField] private Transform m_endTransform;
    [SerializeField] [Range(1, 100)] private int segment = 20;
    [SerializeField] private float curvature = 1f;
    [SerializeField] [Range(3, 8)] private int radiusSegment = 6;
    [SerializeField] private float radius = 0.2f;

    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;
    private float m_segmentNormalized;
    private Vector3 m_startPosition;
    private Vector3 m_endPosition;
    private Vector3 m_localStartPosition;
    private Vector3 m_localEndPosition;

    private void OnValidate()
    {
        UpdateMesh();
    }

    private void Update()
    {
        if(!m_updateEveryFrame)
        {
            return;
        }

        UpdateMesh();
    }

    private void UpdateMesh()
    {
        if (m_startTransform == null ||
            m_endTransform == null)
        {
            return;
        }

        if (m_meshFilter == null)
        {
            m_meshFilter = GetComponent<MeshFilter>();
        }

        if(m_meshRenderer == null)
        {
            m_meshRenderer = GetComponent<MeshRenderer>();
        }

        m_meshFilter.sharedMesh = GenerateMesh();
    }

    private Mesh GenerateMesh()
    {
        UpdateParameters();

        Mesh mesh = new Mesh();
        mesh.name = "Rope Procedural Mesh";

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        float uvLength = 0f;
        
        for(int i = 0; i <= segment; i++)
        {
            Vector3[] segmentVertices = GetVertices(i);
            for(int j = 0; j < segmentVertices.Length; j++)
            {
                vertices.Add(segmentVertices[j]);
                normals.Add((segmentVertices[j] - GetVertex(i)).normalized);
                uvs.Add(new Vector2(uvLength, (float)j / (segmentVertices.Length - 1)));

                if(i < segment)
                {
                    int index = j + i * (radiusSegment + 1);
                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + radiusSegment + 1);

                    triangles.Add(index);
                    triangles.Add(index + radiusSegment + 1);
                    triangles.Add(index + radiusSegment);
                }
            }

            uvLength += GetSegmentLength(i);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

    private void UpdateParameters()
    {
        m_segmentNormalized = 1f / segment;
        m_startPosition = m_startTransform.position;
        m_localStartPosition = transform.InverseTransformPoint(m_startPosition);
        m_endPosition = m_endTransform.position;
        m_localEndPosition = transform.InverseTransformPoint(m_endPosition);
    }

    private Vector3[] GetVertices(int i)
    {
        Vector3 vertex = GetVertex(i);
        Vector3 direction;

        if(i == 0)
        {
            direction = GetVertex(1) - GetVertex(0);
        }
        else if(i == segment)
        {
            direction = GetVertex(i) - GetVertex(i - 1);
        }
        else
        {
            direction = GetVertex(i + 1) - GetVertex(i - 1);
        }

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.Cross(Vector3.down, m_localEndPosition - m_localStartPosition));

        List<Vector3> vertices = new List<Vector3>();
        float angleStep = 360f / radiusSegment;
        for(int x = 0; x <= radiusSegment; x++)
        {
            float angle = angleStep * x * Mathf.Deg2Rad;
            vertices.Add(vertex + rotation * new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0));
        }

        return vertices.ToArray();
    }

    private Vector3 GetVertex(int i)
    {
        Vector3 bias = (m_localEndPosition - m_localStartPosition) / segment;
        return m_localStartPosition + bias * i + Vector3.up * GetHeight(i);
    }

    private float GetHeight(int i)
    {
        i = Mathf.Clamp(i, 0, segment);
        return (Mathf.Pow(m_segmentNormalized * i * 2 - 1, 2) - 1) * curvature;
    }

    private float GetSegmentLength(int i)
    {
        return (GetVertex(i + 1) - GetVertex(i)).magnitude;
    }
}

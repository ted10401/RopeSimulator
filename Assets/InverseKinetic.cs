using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinetic : MonoBehaviour
{
    public Transform startTransform;
    public Transform endTransform;
    public int segmentCount = 100;
    public float segmentLength = 0.2f;
    public List<Segment> segments;

    private void Awake()
    {
        segments = new List<Segment>();
        for (int i = 0; i < segmentCount; i++)
        {
            if (i == 0)
            {
                var seg = new Segment(startTransform.position.x, startTransform.position.y, segmentLength, 30);
                segments.Add(seg);
            }
            else
            {
                Segment seg = new Segment(segments[i - 1], segmentLength, 30);
                segments.Add(seg);
            }
        }
    }

    private void Update()
    {
        for (int i = segments.Count - 1; i >= 0; i--)
        {
            if (i == segments.Count - 1)
            {
                segments[i].Follow(endTransform.position);
            }
            else
            {
                segments[i].Follow(segments[i + 1].a);
            }
            segments[i].update();

        }


        segments[0].SetA(startTransform.position);
        segments[0].update();

        for (int i = 1; i < segments.Count; i++)
        {
            segments[i].SetA(segments[i - 1].b);
            segments[i].update();
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            segments[i].Show();
        }
    }
}
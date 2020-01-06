using UnityEngine;
using System;

[Serializable]
public class Segment
{
    public Vector2 a;
    public Vector2 b;
    public float len;
    public float angle;

    public Segment(Segment parent_, float len_, float angle_)
    {
        a = new Vector2(parent_.b.x, parent_.b.y);
        len = len_;
        angle = angle_;
        caculateB();
    }

    public void SetA(Vector2 a_)
    {
        a = a_;
        caculateB();
    }

    public void Follow(Vector2 pos)
    {
        Vector3 dir = Vector3.Normalize(pos - a);
        angle = Mathf.Atan2(dir.y, dir.x);
        dir = dir * len * -1f;
        a = pos + new Vector2(dir.x, dir.y);

    }

    public Segment(float x, float y, float len_, float angle_)
    {
        a = new Vector2(x, y);
        len = len_;
        angle = angle_;
        caculateB();
    }

    public void Show()
    {
        Gizmos.DrawLine(a, b);
        Gizmos.DrawSphere(a, len * 0.1f);
    }

    public void caculateB()
    {
        float dx = len * Mathf.Cos(angle);
        float dy = len * Mathf.Sin(angle);
        b = new Vector2(a.x + dx, a.y + dy);
    }

    public void update()
    {
        caculateB();
    }
}
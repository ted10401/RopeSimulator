using UnityEngine;

public class Rope3D : MonoBehaviour
{
    private class Segment3D
    {
        public Segment3D parent;
        public Segment3D child;
        public Vector3 position;
        public float length;
        public float angle;

        public Segment3D(Segment3D parent, Vector3 position, float length)
        {
            this.parent = parent;
            this.position = position;
            this.length = length;
        }

        public void Follow(Vector3 position)
        {
            PointAt(position);
            MoveTo(position);
            if (parent != null)
            {
                parent.Follow(this.position);
            }
        }

        private void PointAt(Vector3 position)
        {
            angle = Mathf.Atan2(position.y - this.position.y, position.x - this.position.x);
        }

        private void MoveTo(Vector3 position)
        {
            this.position.x = position.x - length * Mathf.Cos(angle);
            this.position.y = position.y - length * Mathf.Sin(angle);
        }

        public void SetPosition(Vector3 position)
        {
            if (this.position == position)
            {
                return;
            }

            this.position = position;
            if (child != null)
            {
                Vector3 pos = position;
                pos.x += length * Mathf.Cos(angle);
                pos.y += length * Mathf.Sin(angle);
                child.SetPosition(pos);
            }
        }

        public void Draw()
        {
            Vector3 pos = position;
            Gizmos.DrawSphere(pos, 0.1f * length);

            Vector3 pos2 = pos;
            pos2.x += length * Mathf.Cos(angle);
            pos2.y += length * Mathf.Sin(angle);
            Gizmos.DrawLine(pos, pos2);

            if (parent != null)
            {
                parent.Draw();
            }
        }
    }

    public Transform startTransform;
    public Transform targetTransform;
    public bool followMousePosition = true;
    public bool fixedStart = false;
    public int segmentCount = 150;
    public float segmentLength = 0.1f;
    private Segment3D[] m_segments;

    private void Awake()
    {
        m_segments = new Segment3D[segmentCount];
        for (int i = 0; i < segmentCount; i++)
        {
            if (i == 0)
            {
                m_segments[i] = new Segment3D(null, startTransform.position, segmentLength);
            }
            else
            {
                m_segments[i] = new Segment3D(m_segments[i - 1], Vector3.zero, segmentLength);
            }
        }

        for (int i = segmentCount - 2; i >= 0; i--)
        {
            m_segments[i].child = m_segments[i + 1];
        }
    }

    private void Update()
    {
        if (followMousePosition)
        {
            targetTransform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        m_segments[segmentCount - 1].Follow(targetTransform.position);

        if (fixedStart)
        {
            m_segments[0].SetPosition(startTransform.position);
        }
    }

    private void OnDrawGizmos()
    {
        if (!enabled)
        {
            return;
        }

        if (m_segments == null || m_segments.Length == 0)
        {
            return;
        }

        if (startTransform)
        {
            Gizmos.DrawSphere(startTransform.position, 0.1f);
        }

        if (targetTransform)
        {
            Gizmos.DrawSphere(targetTransform.position, 0.1f);
        }

        m_segments[segmentCount - 1].Draw();
    }
}

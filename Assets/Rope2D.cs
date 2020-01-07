using UnityEngine;

public class Rope2D : MonoBehaviour
{
    private class Segment2D
    {
        public Segment2D parent;
        public Segment2D child;
        public float x;
        public float y;
        public float length;
        public float angle;

        public Segment2D(Segment2D parent, float x, float y, float length)
        {
            this.parent = parent;
            this.x = x;
            this.y = y;
            this.length = length;
        }

        public void Follow(float x, float y)
        {
            PointAt(x, y);
            MoveTo(x, y);
            if(parent != null)
            {
                parent.Follow(this.x, this.y);
            }
        }

        private void PointAt(float x, float y)
        {
            angle = Mathf.Atan2(y - this.y, x - this.x);
        }

        private void MoveTo(float x, float y)
        {
            this.x = x - length * Mathf.Cos(angle);
            this.y = y - length * Mathf.Sin(angle);
        }

        public void SetPosition(float x, float y)
        {
            if(this.x == x && this.y == y)
            {
                return;
            }

            this.x = x;
            this.y = y;
            if (child != null)
            {
                Vector3 pos = new Vector3(x, y);
                pos.x += length * Mathf.Cos(angle);
                pos.y += length * Mathf.Sin(angle);
                child.SetPosition(pos.x, pos.y);
            }
        }

        public void Draw()
        {
            Vector3 pos = new Vector3(x, y);
            Gizmos.DrawSphere(pos, 0.1f * length);

            Vector3 pos2 = pos;
            pos2.x += length * Mathf.Cos(angle);
            pos2.y += length * Mathf.Sin(angle);
            Gizmos.DrawLine(pos, pos2);

            if(parent != null)
            {
                parent.Draw();
            }
        }
    }

    public Transform startTransform;
    public Transform targetTransform;
    public bool fixedStart = false;
    public int segmentCount = 150;
    public float segmentLength = 0.1f;
    private Segment2D[] m_segments;

    private void Awake()
    {
        m_segments = new Segment2D[segmentCount];
        for(int i = 0; i < segmentCount; i++)
        {
            if(i == 0)
            {
                m_segments[i] = new Segment2D(null, startTransform.position.x, startTransform.position.y, segmentLength);
            }
            else
            {
                m_segments[i] = new Segment2D(m_segments[i - 1], 0, 0, segmentLength);
            }
        }

        for (int i = segmentCount - 2; i >= 0; i--)
        {
            m_segments[i].child = m_segments[i + 1];
        }
    }

    private void Update()
    {
        targetTransform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        m_segments[segmentCount - 1].Follow(targetTransform.position.x, targetTransform.position.y);

        if(fixedStart)
        {
            m_segments[0].SetPosition(startTransform.position.x, startTransform.position.y);
        }
    }

    private void OnDrawGizmos()
    {
        if(m_segments == null || m_segments.Length == 0)
        {
            return;
        }

        m_segments[segmentCount - 1].Draw();
    }
}

using UnityEngine;

public class Rope2D : MonoBehaviour
{
    private class Segment2D
    {
        public Segment2D parent;
        public Segment2D child;
        public Vector2 position;
        public float length;
        public float angle;

        public Segment2D(Segment2D parent, Vector2 position, float length)
        {
            this.parent = parent;
            this.position = position;
            this.length = length;
        }

        public void Follow(Vector2 position)
        {
            PointAt(position);
            MoveTo(position);
            if(parent != null)
            {
                parent.Follow(this.position);
            }
        }

        private void PointAt(Vector2 position)
        {
            angle = Mathf.Atan2(position.y - this.position.y, position.x - this.position.x);
        }

        private void MoveTo(Vector2 position)
        {
            this.position = position - GetBias();
        }

        private Vector2 GetBias()
        {
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * length;
        }

        public void SetPosition(Vector2 position)
        {
            if(this.position == position)
            {
                return;
            }

            this.position = position;
            if (child != null)
            {
                Vector2 pos = position + GetBias();
                child.SetPosition(pos);
            }
        }

        public void Draw()
        {
            Vector2 pos = position;
            Gizmos.DrawSphere(pos, 0.1f * length);

            Vector2 pos2 = position + GetBias();
            Gizmos.DrawLine(pos, pos2);

            if(parent != null)
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
    private Segment2D[] m_segments;

    private void Awake()
    {
        m_segments = new Segment2D[segmentCount];
        for(int i = 0; i < segmentCount; i++)
        {
            if(i == 0)
            {
                m_segments[i] = new Segment2D(null, startTransform.position, segmentLength);
            }
            else
            {
                m_segments[i] = new Segment2D(m_segments[i - 1], Vector2.zero, segmentLength);
            }
        }

        for (int i = segmentCount - 2; i >= 0; i--)
        {
            m_segments[i].child = m_segments[i + 1];
        }
    }

    private void Update()
    {
        if(followMousePosition)
        {
            targetTransform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        m_segments[segmentCount - 1].Follow(targetTransform.position);

        if(fixedStart)
        {
            m_segments[0].SetPosition(startTransform.position);
        }
    }

    private void OnDrawGizmos()
    {
        if(!enabled)
        {
            return;
        }

        if(m_segments == null || m_segments.Length == 0)
        {
            return;
        }

        if(startTransform)
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

using System.Collections.Generic;
using UnityEngine;

public class RopeCharacterJoint : MonoBehaviour
{
    public GameObject parentObject;
    public CharacterJoint partPrefab;

    public int length = 1;
    public float partDistance = 0.5f;

    public bool reset;
    public bool spawn;
    public bool snapFirst;
    public bool snapLast;

    private List<GameObject> m_parts;

    private void Awake()
    {
        if(m_parts == null)
        {
            m_parts = new List<GameObject>();
        }

        Clear();
    }

    private void Clear()
    {
        foreach(GameObject obj in m_parts)
        {
            GameObject.Destroy(obj);
        }

        m_parts.Clear();
    }

    private void Update()
    {
        if(reset)
        {
            reset = false;
            Clear();
        }

        if(spawn)
        {
            spawn = false;
            Spawn();
        }
    }

    private void Spawn()
    {
        Clear();

        int count = (int)(length / partDistance);
        Vector3 position;
        CharacterJoint temp = null;
        for(int i = 0; i < count; i++)
        {
            position = transform.position;
            position.y -= partDistance * (i + 1);

            temp = Instantiate(partPrefab, position, Quaternion.identity, parentObject.transform);
            temp.name = parentObject.transform.childCount.ToString();
            m_parts.Add(temp.gameObject);

            if(i == 0)
            {
                if (snapFirst)
                {
                    temp.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                }

                Destroy(temp);
            }
            else
            {
                temp.connectedBody = parentObject.transform.Find((parentObject.transform.childCount - 1).ToString()).GetComponent<Rigidbody>();
            }
        }

        if(snapLast)
        {
            parentObject.transform.Find(parentObject.transform.childCount.ToString()).GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }
}

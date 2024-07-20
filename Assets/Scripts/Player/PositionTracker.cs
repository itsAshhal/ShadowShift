using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShadowShift.Player
{
    /// <summary>
    /// Helps the gameObject to start from a location and then stick to that location
    /// regardless of being the child of its parent
    /// </summary>
    public class PositionTracker : MonoBehaviour
    {
        [SerializeField] Transform m_parent;
        [SerializeField] bool m_unParentOnAwake = true;
        Vector2 m_distance;
        Rigidbody2D m_rb;

        void Awake()
        {
            // Calculate the distance as a relative offset
            m_distance = transform.position - m_parent.position;

            m_rb = GetComponent<Rigidbody2D>();

            if (m_unParentOnAwake) this.transform.SetParent(null);
        }

        void Update()
        {
            // Maintain the calculated distance for your position
            //transform.position = (Vector2)m_parent.position + m_distance;

            m_rb.MovePosition((Vector2)m_parent.position + m_distance);
        }
    }
}

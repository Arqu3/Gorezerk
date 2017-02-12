using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Projectile : MonoBehaviour
{
    //Public vars
    public float m_Speed = 10.0f;

    //Component vars
    protected Rigidbody2D m_Rigidbody;

	protected virtual void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
	}
}

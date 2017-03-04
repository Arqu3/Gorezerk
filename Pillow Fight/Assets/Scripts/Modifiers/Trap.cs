using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Trap : MonoBehaviour
{

    //Component vars
    private Rigidbody2D m_Rigidbody;

	void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!m_Rigidbody.isKinematic)
            m_Rigidbody.isKinematic = true;


    }
}

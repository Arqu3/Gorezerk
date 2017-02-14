using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class CanvasLookAtCamera : MonoBehaviour
{

    //Lookat vars
    private Transform m_LookAt;

	void Start()
    {
        m_LookAt = Camera.main.transform;
	}
	
	void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - m_LookAt.position);
    }
}

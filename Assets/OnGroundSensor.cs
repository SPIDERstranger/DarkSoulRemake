using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundSensor : MonoBehaviour
{
    public CapsuleCollider cal;
    public float offset = 0.07f;
    private float radius;
    private Vector3 point1;
    private Vector3 point2;

    private void Awake()
    {
        radius = cal.radius-0.05f;

    }

    private void FixedUpdate()
    {
        point1 = transform.position + transform.up * (radius-offset);
        point2 = transform.position + transform.up * (cal.height -  radius - offset);
        Collider[] collisions = Physics.OverlapCapsule(point1, point2, radius, LayerMask.GetMask("Ground"));


        if(collisions.Length!=0)
        {
            SendMessageUpwards("IsGround");
        }
        else
        {
            SendMessageUpwards("IsNotGround");

        }
        
    }

}

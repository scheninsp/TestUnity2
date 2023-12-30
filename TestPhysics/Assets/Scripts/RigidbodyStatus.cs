using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyStatus : MonoBehaviour
{
    public Vector3 cached_velocity;
    public Vector3 cached_angular_velocity;

    Rigidbody rgd;

    void Start()
    {
        rgd = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        cached_velocity = rgd.velocity;
        cached_angular_velocity = rgd.angularVelocity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForce1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var rgd = GetComponent<Rigidbody>();
        Vector3 pos = new Vector3(rgd.worldCenterOfMass.x, 4, rgd.worldCenterOfMass.z);
        Vector3 force = new Vector3(0, 0, 10);

        Debug.DrawRay(pos, force, Color.red, 0.1f);
        //rgd.AddForceAtPosition(force, pos);

        Vector3 I = rgd.inertiaTensor;
        Quaternion Ir = rgd.inertiaTensorRotation;
        Vector3 cent = rgd.centerOfMass;
        Vector3 cent_w = rgd.worldCenterOfMass;

        //Debug.Log("finish");

        //Vector3 torque = Vector3.Cross(pos - cent_w, force);
        //rgd.AddTorque(torque);

        Vector3 r_local = transform.InverseTransformVector(pos - cent_w);
        Vector3 f_local = transform.InverseTransformVector(force);
        Vector3 torque_local = Vector3.Cross(r_local, f_local);
        rgd.AddRelativeTorque(torque_local);
    }

}

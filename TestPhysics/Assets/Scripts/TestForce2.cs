using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class TestForce2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var rgd = GetComponent<Rigidbody>();
        Vector3 pos = new Vector3(rgd.worldCenterOfMass.x, 4, rgd.worldCenterOfMass.z);
        Vector3 force = new Vector3(0, 0, 10);

        Debug.DrawRay(pos, force, Color.red, 1f);
        //rgd.AddForceAtPosition(force, pos);

        Vector3 I = rgd.inertiaTensor;
        Quaternion Ir = rgd.inertiaTensorRotation;
        Vector3 cent = rgd.centerOfMass;
        Vector3 cent_w = rgd.worldCenterOfMass;


        Vector3 torque = Vector3.Cross(pos - cent_w, force);
        Vector3 torque_local = transform.InverseTransformDirection(torque);

        //rgd.AddTorque(torque);
        float[][] mat = new float[][] {
            new float[]{I.x, 0, 0},
            new float[]{ 0, I.y, 0},
            new float[]{0, 0, I.z}};

        Matrix Im = new Matrix(mat);
        Matrix Irm = new Matrix(Ir);

        Matrix It = Irm * Im * Irm.Transpose();

        //float[][] mat2 = new float[][] {
        //    new float[]{2, 1, 0},
        //    new float[]{ 0, 1, 0},
        //    new float[]{0, 1, 2}};
        //Matrix testm = new Matrix(mat2);
        //Matrix testm_T = testm.Transpose();

        Vector3 wa_local = (It.Inverse() * torque_local).ToVec3();
        Vector3 wa = transform.TransformDirection(wa_local);

        rgd.angularVelocity = rgd.angularVelocity + wa * Time.deltaTime;

        Debug.Log("finish");


    }

}
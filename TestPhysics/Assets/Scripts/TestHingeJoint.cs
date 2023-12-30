using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class TestHingeJoint : MonoBehaviour
{
    
    public Transform A;
    public Transform B;

    //to rotate
    public float angle_Ax;
    public float angle_Ay;
    public float angle_Bx;
    public float angle_By;

    Quaternion qAi;
    Quaternion qBi;
    Vector3 XOrig;

    //Temp
    Vector3 XA;
    Vector3 XB;

    Vector3 PinPointLocalToA = new Vector3(1, 2, 3);
    Vector3 PinPointLocalToB = new Vector3(4, 5, 6);

    void Start()
    {
        Test1();
    }

    //Test RotateToAlignAxes
    void Test1()
    {
        qAi = A.rotation;
        qBi = B.rotation;

        //ApplyXYRotation(A, angle_Ax, angle_Ay);  //自由旋转，没用了
        //ApplyXYRotation(B, angle_Bx, angle_By);

        Vector3 OffsetA, OffsetB, Correction;
        GetPositionCorrection(A, B, out OffsetA, out OffsetB, out Correction);

        ApplyPushToRotateBody(A, Correction, OffsetA);
        ApplyPushToRotateBody(B, -Correction, OffsetB);

        UpdateLocalRotateAxes(out XA, out XB);
        Quaternion q = GetAxisDiffQ(XA, XB);
        float angle = Mathf.Rad2Deg * Mathf.Acos(q.w);  //angle between XA, XB
        //XA = q * XA;
        RotateToAlignAxes(XA, XB);

        Debug.Log("Finished");
    }
    
    //测试 dq 的意义，对比直接乘 dq 和作为 infinitesimal 使用的差异
    void Test2()
    {
        Vector3 XA = A.rotation * new Vector3(1, 0, 0);
        Quaternion q = Quaternion.AngleAxis(1, XA);
        A.rotation = q * A.rotation;
        ApplyInfiniteSimalQuaternion(B, q);
    }

    private void Update()
    {
        Debug.DrawRay(A.position, XA, Color.red);
        Debug.DrawRay(B.position, XB, Color.blue);
    }


    void GetPositionCorrection(Transform A, Transform B, out Vector3 OutBodyToA, out Vector3 OutBodyToB, out Vector3 Correction)
    {
        OutBodyToA = A.rotation * PinPointLocalToA;
        OutBodyToB = B.rotation * PinPointLocalToB;
        Vector3 PinPointOnA = A.position + OutBodyToA;
        Vector3 PinPointOnB = B.position + OutBodyToB;
        Correction = PinPointOnB - PinPointOnA;
    }


    void ApplyXYRotation(Transform A, float angle_Ax, float angle_Ay)
    {
        Vector3 Axi = A.rotation * new Vector3(1, 0, 0);
        Vector3 Ayi = A.rotation * new Vector3(0, 1, 0);
        Quaternion Adqx = Quaternion.AngleAxis(angle_Ax, Axi);
        Quaternion Adqy = Quaternion.AngleAxis(angle_Ay, Ayi);
        Quaternion Adq = Adqy * Adqx;  //这里先绕x轴转，再绕y轴转。zxy这样顺序下不会影响x
        A.rotation = Adq * A.rotation;
    }

    void UpdateLocalRotateAxes(out Vector3 XA, out Vector3 XB)
    {
        XA = A.rotation * Quaternion.Inverse(qAi) * qBi * new Vector3(1, 0, 0);
        XB = B.rotation * Quaternion.Inverse(qBi) * qBi * new Vector3(1, 0, 0);
    }

    void RotateToAlignAxes(Vector3 XA, Vector3 XB)
    {
        Vector3 ACrossB = Vector3.Cross(XA, XB);
        float Magnitude = ACrossB.magnitude;
        Vector3 Axis = Vector3.zero;
        if (Magnitude > 1e-8)
        {
            Axis = ACrossB.normalized;
        }
        float EffectiveInvMass = Vector3.Dot(Axis, Axis);
        if (EffectiveInvMass < 1e-12)
        {
            return;
        }
        float DeltaLambda = Magnitude / EffectiveInvMass;
        Vector3 Push = Axis * DeltaLambda * 0.5f;
        Quaternion DeltaQ = new Quaternion(Push.x, Push.y, Push.z, 0);

        //Quaternion Push = GetAxisDiffQ(XA, XB);
        //Quaternion DeltaQ = new Quaternion(Push.x, Push.y, Push.z, 0);
        ApplyRotationDelta(A, DeltaQ);
        ApplyRotationDelta(B, new Quaternion(-DeltaQ.x, -DeltaQ.y, -DeltaQ.z, -DeltaQ.w));
    }

    void ApplyRotationDelta(Transform T, Quaternion DeltaQ)
    {
        Quaternion Delta = DeltaQ * T.rotation;

        //如果去掉 *0.5 就是旋转 DeltaQ.w ，乘了以后就是旋转 DeltaQ.w/2 
        float x = Delta.x * 0.5f;
        float y = Delta.y * 0.5f;
        float z = Delta.z * 0.5f;
        float w = Delta.w * 0.5f;

        //float x = Delta.x;
        //float y = Delta.y;
        //float z = Delta.z;
        //float w = Delta.w;

        Quaternion q = new Quaternion(T.rotation.x + x, T.rotation.y + y, T.rotation.z + z, T.rotation.w + w);
        T.rotation = q.normalized;
    }

    void ApplyPushToRotateBody(Transform T, Vector3 Push, Vector3 Offset)
    {
        Vector3 Omega = 1.0f * Vector3.Cross(Offset, Push);
        Quaternion DeltaQ = new Quaternion(Omega.x, Omega.y, Omega.z, 0.0f);
        ApplyRotationDelta(T, DeltaQ);
    }

    void ApplyInfiniteSimalQuaternion(Transform T, Quaternion q)
    {
        Quaternion DeltaQ = new Quaternion(q.x, q.y, q.z, 0);
        ApplyRotationDelta(T, DeltaQ);
    }

    //get the quaternion to rotate xa to xb
    //https://arrowinmyknee.com/2021/02/10/how-to-get-rotation-from-two-vectors/
    Quaternion GetAxisDiffQ(Vector3 xa, Vector3 xb)
    {
        Vector3 axis = Vector3.Cross(xa, xb).normalized;
        float cos_theta = Vector3.Dot(xa.normalized, xb.normalized);
        float half_cos = Mathf.Sqrt(0.5f * (1.0f + cos_theta));   //half angle formula : cos(a/2) = sqrt((1-cos(a))/2)
        float half_sin = Mathf.Sqrt(0.5f * (1.0f - cos_theta));
        return new Quaternion(axis.x * half_sin, axis.y * half_sin, axis.z * half_sin, half_cos); 
    }

}

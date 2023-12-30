using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;


public class PhysicsMaterialImpl : MonoBehaviour
{
    float friction_static = 0.1f;
    float friction_dynamic = 0.1f;
    float restitution = 0.1f;

    ContactPoint[] contact_cache = null;

    float custom_gravity = 9.8f;
    float min_v_dynamic_friction = 0.2f;
    float perp_friction_drop = 0.1f;

    float timer = 0f;
    float cur_time;
    PhysicsMaterialValues data;

    private void Start()
    {
        data = GetComponent<PhysicsMaterialValues>();
        if (data != null)
        {
            SetData();
        }
    }

    private void SetData()
    {
        friction_static = data.friction_static;
        friction_dynamic = data.friction_dynamic;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time == cur_time) { return; }//同一帧内多个Collider 只能触发一次 OnCollisionEnter

        contact_cache = collision.contacts;

        SetData();
        timer = 0f;
        cur_time = Time.time;

        Collider hit = collision.collider;

        Vector3 v = Vector3.zero;
        Vector3 wa = Vector3.zero;
        var rgd_status = transform.GetComponent<RigidbodyStatus>();
        var rgd = transform.GetComponent<Rigidbody>();
        if (rgd_status != null)
        {
            v = rgd_status.cached_velocity;
            wa = rgd_status.cached_angular_velocity;
        }
        else
        {
            Debug.LogError($"RigidbodyStatus is not found on gameObject {transform.name}");
        }

        ProcessFriction(collision, rgd, v, wa);
    }

    private void OnCollisionStay(Collision collision)
    {
        contact_cache = collision.contacts;

        cur_time = Time.time;
        timer += Time.deltaTime;

        Collider hit = collision.collider;

        Vector3 v = Vector3.zero;
        Vector3 wa = Vector3.zero;
        var rgd_status = transform.GetComponent<RigidbodyStatus>();
        var rgd = transform.GetComponent<Rigidbody>();
        if (rgd_status != null)
        {
            v = rgd_status.cached_velocity;
            wa = rgd_status.cached_angular_velocity;
        }
        else
        {
            Debug.LogError($"RigidbodyStatus is not found on gameObject {transform.name}");
        }


        ProcessFriction(collision, rgd, v, wa);
    }


    private void ProcessFriction(Collision collision, Rigidbody rgd, Vector3 v, Vector3 wa)
    {
        float sep_mg = rgd.mass * custom_gravity / collision.contactCount;
        Vector3 wacc_sum = Vector3.zero;
        foreach (var contact in collision.contacts)
        {
            Vector3 wacc = Vector3.zero;

            Vector3 normal_out = -contact.normal;
            Vector3 vp = rgd.GetPointVelocity(contact.point);
            Vector3 v_parallel = vp - Vector3.Dot(vp, normal_out) * normal_out;

            float friction_other = 0;
            float friction = 0;

            var phymat_sim_other = contact.otherCollider.transform.GetComponent<PhysicsMaterialValues>();
            if (phymat_sim_other != null)
            {
                friction_other = phymat_sim_other.friction_dynamic;
                friction = (friction_dynamic + friction_other) / 2;
            }

            float support_force = sep_mg / Vector3.Dot(Vector3.up, contact.normal);
            Vector3 friction_force = sep_mg * friction * (-v_parallel.normalized);
            Debug.DrawRay(contact.point, friction_force, Color.red);

            wacc = CalculateAngularAcc(rgd, contact.point, friction_force);

            wacc_sum += wacc;
        }

        rgd.angularVelocity = rgd.angularVelocity + wacc_sum * Time.fixedDeltaTime;
        rgd.velocity = rgd.velocity;
    }


    private Vector3 CalculateAngularAcc(Rigidbody rgd, Vector3 pos, Vector3 force)
    {
        Vector3 cent_w = rgd.worldCenterOfMass;
        Vector3 torque = Vector3.Cross(pos - cent_w, force);

        Vector3 I = rgd.inertiaTensor;
        Quaternion Ir = rgd.inertiaTensorRotation;

        float[][] mat = new float[][] {
        new float[]{I.x, 0, 0},
        new float[]{ 0, I.y, 0},
        new float[]{0, 0, I.z}};

        Matrix Im = new Matrix(mat);
        Matrix Irm = new Matrix(Ir);

        Matrix It = Irm * Im;

        Vector3 wa = (It.Inverse() * torque).ToVec3();

        return wa;
    }


    public ContactPoint[] GetContacts()
    {
        return contact_cache;
    }

    private void OnDrawGizmos()
    {
        if(contact_cache != null)
        {
            foreach (var contact in contact_cache)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(contact.point, 0.05f);
                //Debug.DrawRay(contact.point, -contact.normal * 4, Color.blue, 1.0f);
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DeactivateRagdoll();
    }


    public void ActivateRagdoll()
    {
        foreach(var rb in rigidbodies)
        {
            rb.isKinematic = false;
        }
        animator.enabled = false;
    }

    public void DeactivateRagdoll()
    {
        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = true;
        }
        animator.enabled = true;
    }

    public void ApplyForce(Vector3 force)
    {
        var rigidBody = animator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
        rigidBody.AddForce(force, ForceMode.VelocityChange);
    }
}

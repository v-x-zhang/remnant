using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public bool thisObjectIsUI;

    public Transform transformToFollow;

    public bool copyRotation;
    public bool copyPosition;

    [Space(10)]
    public bool useLerping;
    public float lerpSpeed;


    // Update is called once per frame
    void LateUpdate()
    {
        if (copyPosition)
        {
            CopyPosition();
        }

        if (copyRotation)
        {
            CopyRotation();
        }
    }

    void CopyRotation()
    {
       
        if (useLerping)
        {
            Vector3.Lerp(transform.eulerAngles, transformToFollow.eulerAngles, lerpSpeed);
        }
        else
        {
            transform.eulerAngles = transformToFollow.eulerAngles;
        }
    }

    void CopyPosition() 
    {
        if (thisObjectIsUI)
        {
            Vector3 uiPos = Camera.main.WorldToScreenPoint(transformToFollow.transform.position);
            transform.position = uiPos;
            return;
        }

        if (useLerping)
        {
            Vector3.Lerp(transform.position, transformToFollow.position, lerpSpeed);

        }
        else
        {
            transform.position = transformToFollow.position;
        }
    }

}

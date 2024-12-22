using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    
    public void SetTargetTransform(Transform targetTrasnform)
    {
        this.targetTransform = targetTrasnform;
    }

    private void LateUpdate()
    {
        if (targetTransform == null) return;

        transform.position = targetTransform.position;
        transform.rotation = targetTransform.rotation;
    }
}

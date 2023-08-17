using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AimConstraintHelper : MonoBehaviour
{
    private MultiAimConstraint multiAimConstraint;
    private Transform targetTransform;
    private RigBuilder rig;

    // Start is called before the first frame update
    void Start()
    {
        multiAimConstraint = GetComponent<MultiAimConstraint>();
        targetTransform = GameObject.FindGameObjectWithTag("FollowTarget").transform;
        
        WeightedTransform weightedTransform = new WeightedTransform(targetTransform, 1.0f); // 1.0f is the weight
        
        // Create a new WeightedTransformArray
        WeightedTransformArray newSourceObjects = new WeightedTransformArray(1);
        newSourceObjects[0] = weightedTransform;
        
        // Assign newSourceObjects back to multiAimConstraint.data.sourceObjects
        multiAimConstraint.data.sourceObjects = newSourceObjects;

        rig = GetComponentInParent<RigBuilder>();
        Debug.Log(rig.ToString());
        rig.Build();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

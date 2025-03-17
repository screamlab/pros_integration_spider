using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpiderController : MonoBehaviour
{
    public GameObject[] joints;

    public void StopAllJointRotations()
    {
        for (int i = 0; i < joints.Length; i++)
        {;
            UpdateRotationState(RotationDirection.None, joints[i]);
        }
    }

    public void RotateJoint(int jointIndex, RotationDirection direction)
    {
        // StopAllJointRotations();
        UpdateRotationState(direction, joints[jointIndex]);
    }

    // HELPERS

    static void UpdateRotationState(RotationDirection direction, GameObject joint)
    {
        ArticulationJointController jointController = joint.GetComponent<ArticulationJointController>();
        jointController.rotationState = direction;
        // Debug.Log(jointController + " move " + direction);
    }
}

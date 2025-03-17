using Unity.Robotics.UrdfImporter.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class RobotController : MonoBehaviour
{
    [System.Serializable]
    public struct Joint
    {
        public GameObject robotPart;
    }

    public Joint[] joints;

    private void Start()
    {
        // 刪除物體上的 Controller script
        Controller existingController = GetComponent<Controller>();
        if (existingController != null)
        {
            // Debug.LogWarning("Existing Controller script found, removing it.");
            Destroy(existingController);
        }
        AutoAssignJoints();
        // Debug: 確認是否正確添加了 joints
        // Debug.Log("Number of joints assigned: " + joints.Length);
    }

    private void ListAllQualifiedChildObjects(Transform parent, List<Joint> assignedJoints)
    {
        foreach (Transform child in parent)
        {
            // 檢查是否包含 "Collisions" 和 "Visuals" 子物件
            Transform collisions = child.Find("Collisions");
            Transform visuals = child.Find("Visuals");
            ArticulationBody articulationBody = child.GetComponent<ArticulationBody>();
            // foreach (ArticulationJointType jointType in Enum.GetValues(typeof(ArticulationJointType)))
            // {
            //     Debug.Log("ArticulationJointType: " + jointType);
            // }

            // 如果名稱為 base_link，只遞迴其子物件，不加入 jointController
            if (child.name == "base_link" || child.name.ToLower().Contains("wheel"))
            {
                // Debug.Log("Skipping base_link but checking its children");
                if (child.childCount > 0)
                {
                    // 遞迴檢查 base_link 的子物件
                    ListAllQualifiedChildObjects(child, assignedJoints);
                }
                continue; // 繼續到下一個子物件
            }

            if (collisions != null && visuals != null && articulationBody.jointType != ArticulationJointType.FixedJoint)
            {
                // Debug.Log("Qualified Child: " + child.name);

                // 檢查是否已有 ArticulationJointController script，沒有則添加
                ArticulationJointController jointController = child.gameObject.GetComponent<ArticulationJointController>();
                if (jointController == null)
                {
                    // Debug.Log("Adding ArticulationJointController to: " + child.name);
                    jointController = child.gameObject.AddComponent<ArticulationJointController>();
                    ArticulationBody articulationBody_child = child.GetComponent<ArticulationBody>();
                    if (articulationBody != null)
                    {
                        ArticulationDrive xDrive = articulationBody_child.xDrive;
                        xDrive.stiffness = 1000000f;
                        xDrive.damping = 1000f;
                        articulationBody_child.xDrive = xDrive;
                    }
                }

                // 添加到 assignedJoints 列表
                Joint newJoint = new Joint
                {
                    robotPart = child.gameObject
                };
                assignedJoints.Add(newJoint);
            }

            // 如果該子物件還有子物件，遞迴遍歷
            if (child.childCount > 0)
            {
                ListAllQualifiedChildObjects(child, assignedJoints);
            }
        }
    }


    private void AutoAssignJoints()
    {
        List<Joint> assignedJoints = new List<Joint>();

        // 遍歷自身所有子物件
        ListAllQualifiedChildObjects(transform, assignedJoints);

        // 將找到的所有符合條件的 joints 轉換為陣列並賦值給 joints
        joints = assignedJoints.ToArray();

        // Debug: 確認自動分配的 joints 數量
        // Debug.Log("Total joints assigned: " + joints.Length);
    }

    // CONTROL

    public void StopAllJointRotations()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            GameObject robotPart = joints[i].robotPart;
            UpdateRotationState(RotationDirection.None, robotPart);
        }
    }

    public void RotateJoint(int jointIndex, RotationDirection direction)
    {
        Joint joint = joints[jointIndex];
        UpdateRotationState(direction, joint.robotPart);
    }

    // HELPERS

    static void UpdateRotationState(RotationDirection direction, GameObject robotPart)
    {
        ArticulationJointController jointController = robotPart.GetComponent<ArticulationJointController>();
        jointController.rotationState = direction;
    }
}

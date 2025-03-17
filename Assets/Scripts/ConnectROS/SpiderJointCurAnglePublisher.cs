/*
Publish 16 joint angles from Unity to Rosbridge
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderJointCurAnglePublisher : MonoBehaviour
{
    public ArticulationBody[] articulationBodies;
    private float[] xDriveTargets = new float[16];
    public ConnectRosBridge connectRos;
    public string topicname = "/spider_joint_cur_angle";

    void Start()
    {
        if (connectRos != null)
        {
            connectRos.Advertise(topicname, "std_msgs/Float32MultiArray");
        }
        else
        {
            Debug.LogWarning("connectRos is not assigned.");
        }
    }

    void Update()
    {
        if (articulationBodies != null && articulationBodies.Length == 16 && connectRos != null)
        {
            for (int i = 0; i < articulationBodies.Length; i++)
            {
                ArticulationDrive xDrive = articulationBodies[i].xDrive;
                xDriveTargets[i] = xDrive.target;
            }
            PublishPositionToROS(xDriveTargets);
        }
        else
        {
            Debug.LogWarning("ArticulationBody or connectRos is not correctly assigned.");
        }
    }

    void PublishPositionToROS(float[] xDriveTargets)
    {
        connectRos.PublishFloat32MultiArray(topicname, xDriveTargets);
    }
}


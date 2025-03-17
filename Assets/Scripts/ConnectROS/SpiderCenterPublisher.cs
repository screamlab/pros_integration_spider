// This file can track the 3D position of a GameObject in Unity
// And publish the position information to rosbridge, in float array[3] form

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderCenterPublisher : MonoBehaviour
{
    public GameObject base_link;
    public ConnectRosBridge connectRos;
    public string topicname = "/spider_center";

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
        if (base_link != null && connectRos != null)
        {
            Vector3 spider_center = base_link.transform.position;
            PublishPositionToROS(spider_center);
        }
        else
        {
            Debug.LogWarning("base_link or connectRos is not assigned.");
        }
    }

    void PublishPositionToROS(Vector3 position)
    {
        float[] publish_data = { position.x, position.y, position.z };
        connectRos.PublishFloat32MultiArray(topicname, publish_data);
    }
}


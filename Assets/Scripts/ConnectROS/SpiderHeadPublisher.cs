/*
This file can publish the spider's head global position to ROSBridge
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderHeadPublisher : MonoBehaviour
{
    public GameObject head_point;
    public ConnectRosBridge connectRos;
    public string topicname = "/spider_head";

    void Start()
    {
        if (head_point == null || connectRos == null)
        {
            Debug.LogWarning("Head or connectRos is not assigned.");
        }
        else
        {
            connectRos.Advertise(topicname, "std_msgs/Float32MultiArray");
        }
    }

    void Update()
    {
        if (head_point != null && connectRos != null)
        {
            Vector3 head = head_point.transform.position;
            PublishPositionToROS(head);
        }
    }

    void PublishPositionToROS(Vector3 position)
    {
        float[] publish_data = { position.x, position.y, position.z };
        connectRos.PublishFloat32MultiArray(topicname, publish_data);
    }
}

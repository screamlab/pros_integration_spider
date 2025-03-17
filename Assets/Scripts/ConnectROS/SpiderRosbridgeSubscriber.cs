using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using WebSocketSharp;
using TMPro;

public class SpiderRosbridgeSubscriber : MonoBehaviour
{
    public ConnectRosBridge connectRos;
    public string topicName = "spider_joint_trajectory_point";
    public GameObject spider;

    SpiderController spiderController;
    public float[] data = new float[16];
    const int ROTATION_THRESHOLD = 3;
    
    // standRoutine, all joints are 0 degree pose
    float [] standRoutine = new float[16];
    // float[] standRoutine = new float[16] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

    Transform spider_Transform;
    Vector3 spider_rotationAngles;

    // Start is called before the first frame update
    void Start()
    {
        Array.Copy(standRoutine, data, data.Length);
        connectRos.ws.OnMessage += OnWebSocketMessage;
        SubscribeToTopic(topicName);
        spiderController = spider.GetComponent<SpiderController>();
        Debug.Log("Spider Subscriber start.");
        spider_Transform = spider.transform.Find("base_link");
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = spiderController.joints.Length - 1; i >= 0; i--)
        {
            float inputVal = CountInputVal(i);
            RotationDirection direction = GetRotationDirection(inputVal);
            spiderController.RotateJoint(i, direction);
            
            // Debug.Log("the " + i + " th dir is = " + direction);
        }
        spider_rotationAngles = spider_Transform.eulerAngles;
        // Debug.Log(spot_rotationAngles);
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string jsonString = e.Data;
        var genericMessage = JsonUtility.FromJson<GenericRosMessage>(jsonString);
        if (genericMessage.topic == topicName)
        {
            RobotNewsMessageJointTrajectory message = JsonUtility.FromJson<RobotNewsMessageJointTrajectory>(jsonString);
            HandleJointTrajectoryMessage(message);
        }
    }

    private void SubscribeToTopic(string topic)
    {
        string typeMsg = "trajectory_msgs/msg/JointTrajectoryPoint";
        string subscribeMessage = "{\"op\":\"subscribe\",\"id\":\"1\",\"topic\":\"" + topic + "\",\"type\":\"" + typeMsg + "\"}";
        connectRos.ws.Send(subscribeMessage);
    }

    private void HandleJointTrajectoryMessage(RobotNewsMessageJointTrajectory message)
    {
        data = message.msg.positions;
        
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = data[i] * Mathf.Rad2Deg;
            // Debug.Log("received data" + "data[" + i + "]" + " = " + data[i]);
        }
    }

    float CountInputVal(int index)
    {
        ArticulationJointController jointController = spiderController.joints[index].GetComponent<ArticulationJointController>();
        // Debug.Log(jointController);
        float currentAngle = jointController.CurrentPrimaryAxisRotation();
        float targetAngle = data[index];
        float mappedTargetAngle = MapTargetAngle(currentAngle, targetAngle);
        
        if (Math.Abs(mappedTargetAngle - currentAngle) >= ROTATION_THRESHOLD)
        {
            if (mappedTargetAngle > currentAngle) return 1;
            if (currentAngle > mappedTargetAngle) return -1;
        }
        return 0;
    }

    float MapTargetAngle(float currentRotation, float targetAngle)
    {
        float normalizedCurrentRotation = currentRotation % 360f;
        if (normalizedCurrentRotation < 0) normalizedCurrentRotation += 360f;

        int cycleOffset = Mathf.FloorToInt(currentRotation / 360f);
        float adjustedTargetAngle = targetAngle + (cycleOffset * 360f);

        if (Mathf.Abs(adjustedTargetAngle - currentRotation) > 180f)
        {
            if (adjustedTargetAngle > currentRotation)
                adjustedTargetAngle -= 360f;
            else
                adjustedTargetAngle += 360f;
        }

        return adjustedTargetAngle;
    }

    static RotationDirection GetRotationDirection(float inputVal)
    {
        if (inputVal > 0)
            return RotationDirection.Positive;
        else if (inputVal < 0)
            return RotationDirection.Negative;
        else
            return RotationDirection.None;
    }

    [System.Serializable]
    public class GenericRosMessage
    {
        public string op;
        public string topic;
    } 

    [System.Serializable]
    public class RobotNewsMessageJointTrajectory
    {
        public string op;
        public string topic;
        public JointTrajectoryPointMessage msg;
    }


    [System.Serializable]
    public class JointTrajectoryPointMessage
    {
        public float[] positions;
        public float[] velocities;
        public float[] accelerations;
        public float[] effort;
        public TimeFromStart time_from_start;
    }

    [System.Serializable]
    public class TimeFromStart
    {
        public int sec;
        public int nanosec;
    }
}

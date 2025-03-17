using System.Collections.Generic; // 加入 List 所需的命名空間
using UnityEngine;
using WebSocketSharp;
using System;
using TMPro;

public class ArmTransferS2 : MonoBehaviour
{
    public ConnectRosBridge connectRos;
    public float[] jointPositions;
    public bool haveGripper = true;
    string inputTopic = "/robot_arm";
    List<float> data = new List<float>(); // 動態調整大小的 List
    public GameObject robot;
    RobotController robotController;
    const int ROTATION_THRESHOLD = 3;
    bool manual;


    void Start()
    {
        robotController = robot.GetComponent<RobotController>();
        connectRos.ws.OnMessage += OnWebSocketMessage;
        SubscribeToTopic(inputTopic);
    }

    void Update()
    {
        for (int i = robotController.joints.Length - 1; i >= 0; i--)
        {
            if (i < data.Count)
            {
                float inputVal = CountInputVal(i);
                RotationDirection direction = GetRotationDirection(inputVal);
                robotController.RotateJoint(i, direction);
            }
        }
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string jsonString = e.Data;
        var genericMessage = JsonUtility.FromJson<GenericRosMessage>(jsonString);
        if (genericMessage.topic == inputTopic)
        {

            RobotNewsMessageJointTrajectory message = JsonUtility.FromJson<RobotNewsMessageJointTrajectory>(jsonString);
            HandleJointTrajectoryMessage(message);
        }
    }

    private void HandleJointTrajectoryMessage(RobotNewsMessageJointTrajectory message)
    {
        jointPositions = message.msg.positions;

        // 清空並重新設置 data 列表
        data.Clear();

        // 動態計算 data
        // Debug.Log("jointPositions.Length : " + jointPositions.Length);
        for (int i = 0; i < jointPositions.Length; i++)
        {
            if (jointPositions[i] > 0)
                jointPositions[i] = jointPositions[i] * Mathf.Rad2Deg;
            if(i == jointPositions.Length - 1 && haveGripper == true){
                data.Add(jointPositions[i]);
                data.Add(jointPositions[i]);
                break;
            }
            else{
                data.Add(jointPositions[i]);
            }
        }
    }

    private void SubscribeToTopic(string topic)
    {
        string typeMsg = "trajectory_msgs/msg/JointTrajectoryPoint";
        string subscribeMessage = "{\"op\":\"subscribe\",\"id\":\"1\",\"topic\":\"" + topic + "\",\"type\":\"" + typeMsg + "\"}";
        connectRos.ws.Send(subscribeMessage);
    }

    float CountInputVal(int index)
    {
        ArticulationJointController jointController = robotController.joints[index].robotPart.GetComponent<ArticulationJointController>();
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

    public float[] GetCurrentJointPositions(){
        return data.ToArray();
    }

    public void UpdateJointPositions(float[] positions){
        data.Clear();
        for (int i = 0; i < robotController.joints.Length; i++)
        {
            data.Add(positions[i]);
        }
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

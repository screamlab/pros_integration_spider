using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class SpiderTowardAngleResetSubscriber : MonoBehaviour
{
    private const float SPIDER_INIT_TOWARD_ANGLE = 0.0f;
    private float spiderTowardAngle = SPIDER_INIT_TOWARD_ANGLE;

    // Note: The topic name in the script may not match the topic name in the Unity Inspector. 
    // Please refer to the topic name in the Unity Inspector for accuracy.
    [SerializeField] private string SpiderTowardAngleTopic = "/spider_toward_angle";

    // The Spider's child GameObjects whose ArticulationBodies need to be temporarily disabled during the rotation of the spider.
    [SerializeField] private GameObject[] articulationBodyGameObjects = new GameObject[17];
    [SerializeField] private ConnectRosBridge rosBridgeConnector;
    [SerializeField] private GameObject spider;
    [SerializeField] private GameObject trainingPausePublisher;

    void Start()
    { 
        if (spider.transform.rotation.y != SPIDER_INIT_TOWARD_ANGLE)
        {
            Debug.LogError("SPIDER_INIT_TOWARD_ANGLE does not match the spider's initial rotation.");
        }

        rosBridgeConnector.ws.OnMessage += OnWebSocketMessage;
        SubscribeToTopic(SpiderTowardAngleTopic);
    }  

    /// <summary>
    /// Resets the Unity spider's orientation to the specified angle. This occurs after the Unity scene reset and before the pros_AI training begins.
    /// </summary>
    /// <remarks>
    /// During the pros_AI training reset phase, it is crucial to ensure that the Unity spider is fully reset to the correct angle before starting another training round. 
    /// To guarantee this, the pros_AI system waits for the "BreakPoint: false" signal, which confirms that the Unity spider has fully reset to the correct angle.
    /// </remarks>
    void Update()
    {
        // The default value of spiderTowardAngle is SPIDER_INIT_TOWARD_ANGLE, indicating no rotation is needed. 
        // Therefore, SPIDER_INIT_TOWARD_ANGLE can be used to represent that the subscriber has not received a new spiderTowardAngle value.
        if (spiderTowardAngle != SPIDER_INIT_TOWARD_ANGLE)
        {
            // The spider's child GameObjects have ArticulationBody components, 
            // so it is necessary to disable the ArticulationBodies before rotating.
            for (int i = articulationBodyGameObjects.Length - 1; i >= 0; i--) 
            {
                articulationBodyGameObjects[i].GetComponent<ArticulationBody>().enabled = false;
            }

            spider.transform.rotation = Quaternion.Euler(0, spiderTowardAngle, 0);

            for (int i = articulationBodyGameObjects.Length - 1; i >= 0; i--) 
            {
                articulationBodyGameObjects[i].GetComponent<ArticulationBody>().enabled = true;
            }

            trainingPausePublisher.GetComponent<TrainingPausePublisher>().PublishBreakPointInfoToROS(false);
            spiderTowardAngle = SPIDER_INIT_TOWARD_ANGLE;
        }
    }       
    

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string jsonString = e.Data;
        var genericMessage = JsonUtility.FromJson<GenericRosMessage>(jsonString);

        if (genericMessage.topic == SpiderTowardAngleTopic)
        {
            RosFloatMessage message = JsonUtility.FromJson<RosFloatMessage>(jsonString);
            spiderTowardAngle = message.msg.data;
        }
    }

    private void SubscribeToTopic(string topic)
    {
        string typeMsg = "std_msgs/Float32";
        string subscribeMessage = "{\"op\":\"subscribe\",\"id\":\"1\",\"topic\":\"" + topic + "\",\"type\":\"" + typeMsg + "\"}";
        rosBridgeConnector.ws.Send(subscribeMessage);
    }


    [System.Serializable]
    public class GenericRosMessage
    {
        public string op;
        public string topic;
    }

    [System.Serializable]
    public class RosFloatMessage
    {
        public string op;
        public string topic;
        public FloatMsg msg;
    }

    [System.Serializable]
    public class FloatMsg
    {
        public float data;
    }
}
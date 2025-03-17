using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class SpiderSceneResetSubscriber : MonoBehaviour
{
    public ConnectRosBridge connectRos;
    public string resetTopicName = "/reset_unity";
    private bool reset_scene_signal = false;

    void Start()
    {
        connectRos.ws.OnMessage += OnWebSocketMessage;
        SubscribeToTopic(resetTopicName);
    }  

    /// <summary>
    /// Resets the Unity scene and sends a message to ROS to turn off the pros_AI training breakpoint.
    /// </summary>
    /// <remarks>
    /// During the pros_AI training reset phase, it is crucial to ensure that the Unity scene has fully reset 
    /// before sending commands to rotate the spider object's transform. To guarantee the spider object's 
    /// rotation occurs after the scene reset, the pros_AI system waits for the "BreakPoint: false" signal, which 
    /// confirms the Unity scene reset completion.
    /// </remarks>
    void Update()
    {
        if (reset_scene_signal == true)
        {
            /// <remarks>
            /// The Unity scene must include a GameObject named "TrainingPausePublisher" that has the 
            /// TrainingPausePublisher.cs script attached.
            /// </remarks>
            GameObject trainingPausePublisher = GameObject.Find("TrainingPausePublisher");
        
            if (trainingPausePublisher == null)
            {
                Debug.LogError("GameObject 'TrainingPausePublisher' not found in the scene. Please ensure that the scene includes this GameObject with the correct script attached.");
            }

            reset_scene_signal = false;
            ResetScene();
            trainingPausePublisher.GetComponent<TrainingPausePublisher>().PublishBreakPointInfoToROS(false);
        }
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string jsonString = e.Data;
        var genericMessage = JsonUtility.FromJson<GenericRosMessage>(jsonString);

        if (genericMessage.topic == resetTopicName)
        {
            RosBoolMessage message = JsonUtility.FromJson<RosBoolMessage>(jsonString);
            reset_scene_signal = message.msg.data;
        }
    }

    private void SubscribeToTopic(string topic)
    {
        string typeMsg = "std_msgs/Bool";
        string subscribeMessage = "{\"op\":\"subscribe\",\"id\":\"1\",\"topic\":\"" + topic + "\",\"type\":\"" + typeMsg + "\"}";
        connectRos.ws.Send(subscribeMessage);
    }

    /// <summary>
    /// Resets the current scene by reloading it asynchronously. 
    /// </summary>
    /// <remarks>
    /// This method must be executed on the main thread as Unity requires scene loading operations 
    /// to be performed on the main thread. If called from a non-main thread, it will not execute.
    /// </remarks>
    private void ResetScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
        Debug.Log("Scene Reset");
    }

    [System.Serializable]
    public class GenericRosMessage
    {
        public string op;
        public string topic;
    }

    [System.Serializable]
    public class RosBoolMessage
    {
        public string op;
        public string topic;
        public BoolMsg msg;
    }

    [System.Serializable]
    public class BoolMsg
    {
        public bool data;
    }
}

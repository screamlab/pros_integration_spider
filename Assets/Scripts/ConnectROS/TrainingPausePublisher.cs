using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

/// <summary>
/// This class publishes a training pause flag to pros_AI.
/// </summary>
public class TrainingPausePublisher : MonoBehaviour
{
    [SerializeField] private ConnectRosBridge rosBridgeConnector;
    [SerializeField] private string breakPointTopic = "/is_training_pause";

    void Start()
    {
        if (rosBridgeConnector == null)
        {
            Debug.LogWarning("rosBridgeConnector is not assigned.");
        }
        else
        {
            rosBridgeConnector.Advertise(breakPointTopic, "std_msgs/Bool");
        }
    }

    /// <summary>
    /// Publishes the training pause flag to pros_AI training.
    /// </summary>
    /// <param name="isTrainingPaused">Indicating if the training at pros_AI will be paused (true) or not (false).</param>
    public void PublishBreakPointInfoToROS(bool isTrainingPaused)
    {
        rosBridgeConnector.PublishBool(breakPointTopic, isTrainingPaused);
    }
}
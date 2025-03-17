using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject obstacle;
    public GameObject obstacleParent;
    public GameObject lidar;
    public SpawnObject spawnObject;

    Queue<System.Action> actionsToExecuteOnMainThread = new Queue<System.Action>();
    float lidarPositionX = 0f;
    float lidarPositionZ = 0f;

    private void Update()
    {
        lidarPositionX = lidar.transform.position.x;
        lidarPositionZ = lidar.transform.position.z;
        while (actionsToExecuteOnMainThread.Count > 0)
        {
            System.Action action = actionsToExecuteOnMainThread.Dequeue();
            action.Invoke();
        }
    }

    public void QueueAction(System.Action action)
    {
        lock (actionsToExecuteOnMainThread)
        {
            actionsToExecuteOnMainThread.Enqueue(action);
        }
    }

    public void MapGenerate(float resolution, uint width, float originX, float originY, int[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == 100)
            {
                float x = lidarPositionX - originY + (i % width) * resolution;
                float z = lidarPositionZ + originX + (i / width) * resolution;
                QueueAction(() => InstantiateObstacle(new Vector3(x, 0, z)));
            }
        }
    }

    private void InstantiateObstacle(Vector3 position)
    {
        spawnObject.Spawn(obstacle, position, Quaternion.identity, obstacleParent);       
    }
}

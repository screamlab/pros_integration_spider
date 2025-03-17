using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObject : MonoBehaviour
{
    public void Spawn(GameObject objectModel, Vector3 position, Quaternion rotation, GameObject objectParent)
    { 
        GameObject spawnObject = Instantiate(objectModel, position, rotation);
        spawnObject.transform.parent = objectParent.transform;
    }
}

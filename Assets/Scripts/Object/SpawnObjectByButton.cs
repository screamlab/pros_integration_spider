using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectByButton : MonoBehaviour
{
    public GameObject spawnObjectModel;
    public GameObject spawnObjectParent;
    public SpawnObject spawnObject;

    Vector3 position = new Vector3(2f, 2f, 32f);
    Quaternion rotation = Quaternion.identity;

    public void SpawnByButton()
    {
        spawnObject.Spawn(spawnObjectModel, position, rotation, spawnObjectParent);
    }
}

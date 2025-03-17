using UnityEngine;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class SaveAndLoadItem : MonoBehaviour
{
    public GameObject itemList;
    public TMP_InputField loadMapName;
    public TMP_InputField saveMapName;
    public SpawnObject spawnObject;
    public FileBrowser fileBrowser;    

    [SerializeField]
    GameObject firstCubeModel;
    [SerializeField]
    GameObject firstSphereModel;
    [SerializeField]
    GameObject firstCubeParent;
    [SerializeField]
    GameObject firstSphereParent;
    [SerializeField]
    Color32 defaultColor = new Color32(0, 200, 255, 255);

    public void LoadData()
    {
        string filePath = fileBrowser.LoadPathName();
        if (!IsValidPath(filePath)) return;

        SetInputField(loadMapName, Path.GetFileName(filePath) + " loaded!", defaultColor);

        List<GameObject> toBeDeleted = new List<GameObject>();
        AddDeleteChild(itemList, "", toBeDeleted);
        DeleteChild(toBeDeleted);

        string json = File.ReadAllText(filePath);
        Item data = JsonUtility.FromJson<Item>(json);
        SpawnByjson(data);
    }

    public void SaveData()
    {
        string filePath = fileBrowser.SavePathName();
        if (!IsValidPath(filePath)) return;

        Item data = new Item();
        AddChild(itemList, data);
        string json = JsonUtility.ToJson(data);
        
        SaveJsonToFile(json, filePath);
        SetInputField(saveMapName, Path.GetFileName(filePath) + " saved!", defaultColor);    
    }

    // =================
    // === LOAD DATA ===
    // =================
    void SetInputField(TMP_InputField inputfield, string message, Color color)
    {
        inputfield.textComponent.color = color;
        inputfield.text = message;
    }

    void AddDeleteChild(GameObject item, string type, List<GameObject> toBeDeleted)
    {
        foreach (Transform child in item.transform)
        {
            if (type != "")
            {
                toBeDeleted.Add(child.gameObject);
            }
            AddDeleteChild(child.gameObject, child.gameObject.name, toBeDeleted);
        }
    }

    void DeleteChild(List<GameObject> toBeDeleted)
    {
        foreach(GameObject child in toBeDeleted)
            Destroy(child);
    }

    void SpawnByjson(Item data)
    {
        foreach(FirstCube firstCube in data.firstCubes)
        {
            spawnObject.Spawn(firstCubeModel, firstCube.position, firstCube.rotation, firstCubeParent);
        }
        foreach(FirstSphere firstSphere in data.firstSpheres)
        {
            spawnObject.Spawn(firstSphereModel, firstSphere.position, firstSphere.rotation, firstSphereParent);
        }
    }

    // =================
    // === SAVE DATA ===
    // =================
    void AddChild(GameObject item, Item data)
    {
        foreach (Transform child in item.transform)
        {
            if (child.parent == firstCubeParent.transform)
                AddItem(new FirstCube(child.position, child.rotation), data.firstCubes);
            else if (child.parent == firstSphereParent.transform)
                AddItem(new FirstSphere(child.position, child.rotation), data.firstSpheres);

            AddChild(child.gameObject, data);
        }
    }

    void SaveJsonToFile(string json, string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        File.WriteAllText(filePath, json);
    }

    // ================
    // === ADD ITEM ===
    // ================
    void AddItem(FirstCube cube, List<FirstCube> list)
    {
        list.Add(cube);
    }

    void AddItem(FirstSphere sphere, List<FirstSphere> list)
    {
        list.Add(sphere);
    }

    // ==================
    // === CHECK PATH ===
    // ==================
    public static bool IsValidPath(string filePath)
    {
        try
        {
            if (filePath == "") return false;
            string fullPath = Path.GetFullPath(filePath);
            char[] invalidChars = Path.GetInvalidPathChars();
            if (filePath.IndexOfAny(invalidChars) >= 0) return false;
            string fileName = Path.GetFileName(filePath);
            invalidChars = Path.GetInvalidFileNameChars();
            if (fileName.IndexOfAny(invalidChars) >= 0) return false;
            return true;
        }
        catch
        {
            return false;
        }
    }


    // =================
    // === DATA TYPE ===
    // =================
    [System.Serializable]
    public class Item
    {
        public List<FirstCube> firstCubes = new List<FirstCube>();
        public List<FirstSphere> firstSpheres = new List<FirstSphere>();

        public override string ToString()
        {
            return $"Item with {firstCubes.Count} cubes and {firstSpheres.Count} spheres";
        }
    }

    public abstract class ItemBase
    {
        public Vector3 position;
        public Quaternion rotation;

        protected ItemBase(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    [System.Serializable]
    public class FirstCube : ItemBase
    {
        public FirstCube(Vector3 pos, Quaternion rot) : base(pos, rot) { }
    }

    [System.Serializable]
    public class FirstSphere : ItemBase
    {
        public FirstSphere(Vector3 pos, Quaternion rot) : base(pos, rot) { }
    }
}

using UnityEngine;
using UnityEngine.EventSystems;
using SFB;

public class FileBrowser : MonoBehaviour
{
    public void OnPointerDown(PointerEventData eventData) { }

    public string LoadPathName()
    {
        var path = StandaloneFileBrowser.OpenFilePanel("Title", "./", "map", false);
        return path.Length > 0 ? path[0] : "";
    }

    public string SavePathName()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "./", "sample", "map");
        return path;
    }

}

using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PostCatalog : MonoBehaviour
{
    public string url;
    public enum Platform
    {
        WEBGL, IOS, ANDROID, WINDOW
    }
    public Platform platform = Platform.WEBGL;
    public string tagId;
    public string[] catalogs;
    public string[] scenes;
    public Group[] groups;

    [System.Serializable]
    public class Group
    {
        public Vector2Int spawnAt;
        public string scene;
        public Vector2Int[] lands;
    }

    void Start()
    {
        StartCoroutine(Post());
    }

    public IEnumerator Post()
    {
        var cm = new CatalogMeta();
        yield return StartCoroutine(cm.Set(this));

        var www = new UnityWebRequest(url + "?t=" + tagId, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(cm));
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();
        Debug.Log(www.downloadHandler.text);
    }

    [System.Serializable]
    public class CatalogMeta
    {
        public string platform;
        public string t;
        public string[] catalogs;
        public string[] scenes;
        public Group[] groups;

        public IEnumerator Set(PostCatalog data)
        {
            platform = data.platform.ToString();
            t = data.tagId;
            catalogs = data.catalogs;
            scenes = data.scenes;
            groups = data.groups;

            yield return null;

            //var is_done = false;
            //Addressables.LoadResourceLocationsAsync(scenes, Addressables.MergeMode.UseFirst).Completed += (result) =>
            //{
            //    scenes = result.Result.Select(x => x.PrimaryKey).ToArray();
            //    is_done = true;
            //};

            //yield return new WaitUntil(() => is_done);
        }
    }

}

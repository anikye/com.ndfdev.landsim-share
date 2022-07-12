using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using System.Linq;

/// <summary>
/// in design scene
/// 1 build addressable asset
/// 2 host asset
/// 3 copy and paste catalog.json url to catalogURL
/// 4 set tagId from jakaapi
/// 5 set all scenes in catalogs
/// 6 post
/// need jaka api helper
/// </summary>
public class PostCatalog : MonoBehaviour
{
    /// <summary>
    /// end with /
    /// </summary>
    string endpoint = "https://us-central1-test-87c31.cloudfunctions.net/";
    public enum Platform
    {
        WEBGL, IOS, ANDROID, WINDOW
    }
    public Platform platform = Platform.WEBGL;
    public string tagId;
    public string catalogJsonURL;
    public AssetReference[] scenes;

    void Start()
    {
        StartCoroutine(Post());
    }

    public IEnumerator Post()
    {
        var cm = new CatalogMeta();
        cm.Set(this);

        var www = new UnityWebRequest(endpoint + "postcatalog?t=" + tagId, "POST");
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
        public string catalog;
        public string[] scenes;

        public void Set(PostCatalog data)
        {
#if UNITY_EDITOR
            platform = data.platform.ToString();
            t = data.tagId;
            catalog = data.catalogJsonURL;
            scenes = data.scenes.Select(x => x.editorAsset.ToString()).ToArray();

#endif
        }
    }
}

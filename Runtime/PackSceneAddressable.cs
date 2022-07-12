using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;

using UnityEditor.AddressableAssets;

public class PackSceneAddressable : MonoBehaviour
{
    public string RemoteName = "Remote";
    public bool useXZ;
    public int x, z;

    [ContextMenu("test")]
    void Test()
    {
        var groups = AddressableAssetSettingsDefaultObject.Settings.groups;
        var dict = new Dictionary<string, string>();
        var dict_lod = new Dictionary<string, bool>();
        foreach (var group in groups)
        {
            if (group.Name != RemoteName)
                continue;

            foreach (var item in group.entries)
            {
                dict.Add(item.guid, item.address);

                if (item.address.Contains("LOD"))
                    dict_lod.Add(item.address.Replace("LOD", ""), false);
            }
        }

        var all = GameObject.FindObjectsOfType<Transform>();
        var roots = all.Where(x => x.parent == null);
        //Debug.Log("IN SCENE");

        var scene = SceneManager.GetActiveScene();
        var root = new GameObject(scene.name);

        if (useXZ)
            root.name = "SCENE" + x + "_" + z;

        foreach (var item in roots)
        {
            var prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(item.gameObject);
            if (prefab)
            {
                var asset_path = AssetDatabase.GetAssetPath(prefab);
                var guid = AssetDatabase.AssetPathToGUID(asset_path);

                if (dict.ContainsKey(guid))
                {
                    Debug.Log("asset: " + dict[guid]);
                    var go = new GameObject(dict[guid]);
                    go.tag = "asset";
                    go.transform.position = item.position;
                    go.transform.rotation = item.rotation;
                    go.transform.SetParent(root.transform);

                    // set trigger
                    var collider = go.AddComponent<BoxCollider>();
                    collider.isTrigger = true;
                    var render = item.GetComponentInChildren<Renderer>();
                    if (render)
                    {
                        collider.center = render.localBounds.center;
                        collider.size = render.bounds.size;
                    }

                    // group
                    if (item.tag == "single")
                    {
                        var group = new GameObject("single");
                        group.transform.SetParent(go.transform, false);
                        var group_id = new GameObject(guid);
                        group_id.transform.SetParent(group.transform, false);
                    }

                    // lod
                    if (dict_lod.ContainsKey(dict[guid]))
                    {
                        //Debug.Log("has LOD");
                        var lod = new GameObject("LOD");
                        lod.transform.SetParent(go.transform);
                        lod.transform.localPosition = Vector3.zero;
                        lod.transform.localRotation = Quaternion.identity;
                    }
                }
            }
        }
    }

    public AssetReference test;

    [ContextMenu("test 2")]
    void Test2()
    {

        Debug.Log(test.editorAsset.name);
        Debug.Log(test.AssetGUID);

        var groups = AddressableAssetSettingsDefaultObject.Settings.groups;
        foreach (var group in groups)
        {
            foreach (var item in group.entries)
            {
                Debug.Log(item.address + ": " + item.guid);
            }

        }
    }
}
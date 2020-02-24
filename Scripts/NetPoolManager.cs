using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using SLQJ_POOL;

public class NetPoolManager : PoolManager, IPunPrefabPool
{
    public static Dictionary<string, GameObject> prefabResoucePrefabCache = new Dictionary<string, GameObject>();
    private bool bOnce = false;

    public void Awake()
    {
        PhotonNetwork.PrefabPool = this;
        InitailResoucesCache();
    }

    public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
    {
        Debug.Log("net instantiate " + prefabId);
        GameObject gameObj = GetGameObjFromCache(prefabId);
        return gameObject.InstantiateFromPool(gameObj, position, rotation).gameObject;
    }

    public void Destroy(GameObject _gameObject)
    {
        Debug.Log("destroy " + _gameObject.name);
        _gameObject.DestroyToPool(_gameObject);
    }

    private void InitailResoucesCache()
    {
        string prefabTmpName = string.Empty;
        if (!bOnce)
        {
            bOnce = true;
            UnityEngine.Object[] all_resources = Resources.LoadAll("", typeof(GameObject));
            for (int i = 0; i < all_resources.Length; i++)
            {
                GameObject Go = all_resources[i] as GameObject;
                prefabTmpName = Go.name;
                if (null != Go && !string.IsNullOrEmpty(prefabTmpName))
                {
                    if (!prefabResoucePrefabCache.ContainsKey(prefabTmpName))
                    {
                        prefabResoucePrefabCache.Add(prefabTmpName, Go);
                    }
                    else
                    {
                        Debug.LogError(prefabTmpName + " have more than one prefab have the same name ,check all resoures folder.");
                    }
                }
            }

        }
    }

    private GameObject GetGameObjFromCache(string prefabName)
    {
        GameObject resourceGObj = null;
        if (!prefabResoucePrefabCache.TryGetValue(prefabName, out resourceGObj))
        {
            Debug.LogError("please check ,if current " + prefabName + "not in resouce folder");
        }

        if (resourceGObj == null)
        {
            Debug.LogError("Could not Instantiate the prefab [" + prefabName + "]. Please verify this gameobject in a Resources folder.");
        }
        return resourceGObj;
    }

}
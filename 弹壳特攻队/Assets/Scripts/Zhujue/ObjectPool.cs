using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    // 存储不同类型的对象池
    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 从对象池获取对象
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // 如果对象池不存在，创建新的对象池
        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new Queue<GameObject>();
        }

        GameObject obj;

        // 如果池中有可用对象，取出复用
        if (objectPools[prefab].Count > 0)
        {
            obj = objectPools[prefab].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        // 否则创建新对象
        else
        {
            obj = Instantiate(prefab, position, rotation);
            // 添加对象池标签，方便识别
            obj.AddComponent<ObjectPoolItem>().SetOriginalPrefab(prefab);
        }

        return obj;
    }

    // 将对象返回对象池
    public void Despawn(GameObject obj)
    {
        ObjectPoolItem poolItem = obj.GetComponent<ObjectPoolItem>();

        if (poolItem != null)
        {
            GameObject originalPrefab = poolItem.GetOriginalPrefab();

            // 如果该类型的对象池存在
            if (objectPools.ContainsKey(originalPrefab))
            {
                obj.SetActive(false);
                objectPools[originalPrefab].Enqueue(obj);
            }
            else
            {
                // 如果对象池不存在，直接销毁对象
                Destroy(obj);
            }
        }
        else
        {
            // 如果对象没有池标签，直接销毁
            Destroy(obj);
        }
    }
}

// 对象池标记组件
public class ObjectPoolItem : MonoBehaviour
{
    private GameObject originalPrefab;

    public void SetOriginalPrefab(GameObject prefab)
    {
        originalPrefab = prefab;
    }

    public GameObject GetOriginalPrefab()
    {
        return originalPrefab;
    }
}
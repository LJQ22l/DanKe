using UnityEngine;
using System.Collections.Generic;

// 对象池项：用于跟踪对象的原始预制体
public class ObjectPoolItem : MonoBehaviour
{
    private GameObject originalPrefab;

    // 设置原始预制体
    public void SetOriginalPrefab(GameObject prefab)
    {
        originalPrefab = prefab;
    }

    // 获取原始预制体
    public GameObject GetOriginalPrefab()
    {
        return originalPrefab;
    }
}

// 对象池管理器
public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    private Dictionary<GameObject, Queue<GameObject>> objectPools = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // 从对象池生成对象
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!objectPools.ContainsKey(prefab))
            objectPools[prefab] = new Queue<GameObject>();

        GameObject obj;
        if (objectPools[prefab].Count > 0)
        {
            // 从池中取出
            obj = objectPools[prefab].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        else
        {
            // 创建新对象
            obj = Instantiate(prefab, position, rotation);
            obj.AddComponent<ObjectPoolItem>().SetOriginalPrefab(prefab);
        }

        return obj;
    }

    // 回收对象到池
    public void Despawn(GameObject obj)
    {
        if (obj == null) return;

        ObjectPoolItem poolItem = obj.GetComponent<ObjectPoolItem>();
        if (poolItem != null)
        {
            GameObject originalPrefab = poolItem.GetOriginalPrefab();
            if (originalPrefab != null && objectPools.ContainsKey(originalPrefab))
            {
                obj.SetActive(false);
                objectPools[originalPrefab].Enqueue(obj);
            }
            else
            {
                // 如果找不到对应的池，直接销毁
                Destroy(obj);
            }
        }
        else
        {
            // 如果对象没有ObjectPoolItem组件，直接销毁
            Destroy(obj);
        }
    }

    // 延迟回收对象
    public void Despawn(GameObject obj, float delay)
    {
        if (obj == null) return;

        // 创建临时对象处理延迟
        GameObject delayObj = new GameObject("DespawnDelay");
        delayObj.transform.SetParent(transform);
        DelayCall delayCall = delayObj.AddComponent<DelayCall>();
        delayCall.Setup(obj, delay);
    }
}

// 延迟调用辅助类
public class DelayCall : MonoBehaviour
{
    private GameObject targetObject;
    private float delayTime;
    private float startTime;

    public void Setup(GameObject obj, float delay)
    {
        targetObject = obj;
        delayTime = delay;
        startTime = Time.time;
    }

    void Update()
    {
        if (Time.time >= startTime + delayTime)
        {
            if (targetObject != null)
                ObjectPool.Instance.Despawn(targetObject);

            Destroy(gameObject);
        }
    }
}
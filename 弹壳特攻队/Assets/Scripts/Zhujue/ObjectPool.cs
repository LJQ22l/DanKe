using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    // �洢��ͬ���͵Ķ����
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

    // �Ӷ���ػ�ȡ����
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        // �������ز����ڣ������µĶ����
        if (!objectPools.ContainsKey(prefab))
        {
            objectPools[prefab] = new Queue<GameObject>();
        }

        GameObject obj;

        // ��������п��ö���ȡ������
        if (objectPools[prefab].Count > 0)
        {
            obj = objectPools[prefab].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        // ���򴴽��¶���
        else
        {
            obj = Instantiate(prefab, position, rotation);
            // ��Ӷ���ر�ǩ������ʶ��
            obj.AddComponent<ObjectPoolItem>().SetOriginalPrefab(prefab);
        }

        return obj;
    }

    // �����󷵻ض����
    public void Despawn(GameObject obj)
    {
        ObjectPoolItem poolItem = obj.GetComponent<ObjectPoolItem>();

        if (poolItem != null)
        {
            GameObject originalPrefab = poolItem.GetOriginalPrefab();

            // ��������͵Ķ���ش���
            if (objectPools.ContainsKey(originalPrefab))
            {
                obj.SetActive(false);
                objectPools[originalPrefab].Enqueue(obj);
            }
            else
            {
                // �������ز����ڣ�ֱ�����ٶ���
                Destroy(obj);
            }
        }
        else
        {
            // �������û�гر�ǩ��ֱ������
            Destroy(obj);
        }
    }
}

// ����ر�����
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
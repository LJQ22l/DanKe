using UnityEngine;
using System.Collections.Generic;

// ���������ڸ��ٶ����ԭʼԤ����
public class ObjectPoolItem : MonoBehaviour
{
    private GameObject originalPrefab;

    // ����ԭʼԤ����
    public void SetOriginalPrefab(GameObject prefab)
    {
        originalPrefab = prefab;
    }

    // ��ȡԭʼԤ����
    public GameObject GetOriginalPrefab()
    {
        return originalPrefab;
    }
}

// ����ع�����
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

    // �Ӷ�������ɶ���
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!objectPools.ContainsKey(prefab))
            objectPools[prefab] = new Queue<GameObject>();

        GameObject obj;
        if (objectPools[prefab].Count > 0)
        {
            // �ӳ���ȡ��
            obj = objectPools[prefab].Dequeue();
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
        }
        else
        {
            // �����¶���
            obj = Instantiate(prefab, position, rotation);
            obj.AddComponent<ObjectPoolItem>().SetOriginalPrefab(prefab);
        }

        return obj;
    }

    // ���ն��󵽳�
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
                // ����Ҳ�����Ӧ�ĳأ�ֱ������
                Destroy(obj);
            }
        }
        else
        {
            // �������û��ObjectPoolItem�����ֱ������
            Destroy(obj);
        }
    }

    // �ӳٻ��ն���
    public void Despawn(GameObject obj, float delay)
    {
        if (obj == null) return;

        // ������ʱ�������ӳ�
        GameObject delayObj = new GameObject("DespawnDelay");
        delayObj.transform.SetParent(transform);
        DelayCall delayCall = delayObj.AddComponent<DelayCall>();
        delayCall.Setup(obj, delay);
    }
}

// �ӳٵ��ø�����
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
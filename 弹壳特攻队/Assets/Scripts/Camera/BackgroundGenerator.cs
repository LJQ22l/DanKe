using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    [SerializeField] private GameObject backgroundPrefab; // ����Ԥ����
    [SerializeField] private float detectionDistance = 2f; // �����룬��ҽӽ���Ե��Զʱ�����±���

    private List<GameObject> backgrounds = new List<GameObject>(); // �����ɵı���
    private Vector2 minBounds; // ��С�߽�
    private Vector2 maxBounds; // ���߽�

    private void Start()
    {
        // ���ɳ�ʼ����
        GenerateInitialBackground();
    }

    private void Update()
    {
        // ������λ�ò�����Ҫʱ�����±���
        CheckAndGenerateBackground();
    }

    // ���ɳ�ʼ����
    private void GenerateInitialBackground()
    {
        // ���ɵ�һ��������
        GameObject initialBackground = Instantiate(backgroundPrefab, Vector3.zero, Quaternion.identity, transform);
        backgrounds.Add(initialBackground);

        // ���±߽�
        UpdateBounds();
    }

    // ��鲢�����±���
    private void CheckAndGenerateBackground()
    {
        if (backgrounds.Count == 0) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;

        Vector3 playerPos = player.transform.position;

        // ����Ƿ�ӽ��ұ߽�
        if (playerPos.x > maxBounds.x - detectionDistance)
        {
            GenerateBackgroundAt(new Vector3(maxBounds.x, playerPos.y, 0));
        }
        // ����Ƿ�ӽ���߽�
        else if (playerPos.x < minBounds.x + detectionDistance)
        {
            GenerateBackgroundAt(new Vector3(minBounds.x - GetBackgroundWidth(), playerPos.y, 0));
        }

        // ����Ƿ�ӽ��ϱ߽�
        if (playerPos.y > maxBounds.y - detectionDistance)
        {
            GenerateBackgroundAt(new Vector3(playerPos.x, maxBounds.y, 0));
        }
        // ����Ƿ�ӽ��±߽�
        else if (playerPos.y < minBounds.y + detectionDistance)
        {
            GenerateBackgroundAt(new Vector3(playerPos.x, minBounds.y - GetBackgroundHeight(), 0));
        }
    }

    // ��ָ��λ�����ɱ���
    private void GenerateBackgroundAt(Vector3 position)
    {
        // ����λ���Ƿ����б���
        foreach (GameObject bg in backgrounds)
        {
            if (Vector3.Distance(bg.transform.position, position) < 0.1f)
                return;
        }

        // �����±���
        GameObject newBackground = Instantiate(backgroundPrefab, position, Quaternion.identity, transform);
        backgrounds.Add(newBackground);

        // ���±߽�
        UpdateBounds();
    }

    // ���±߽�
    private void UpdateBounds()
    {
        if (backgrounds.Count == 0) return;

        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach (GameObject bg in backgrounds)
        {
            Bounds bounds = GetBackgroundBounds(bg);
            minX = Mathf.Min(minX, bounds.min.x);
            minY = Mathf.Min(minY, bounds.min.y);
            maxX = Mathf.Max(maxX, bounds.max.x);
            maxY = Mathf.Max(maxY, bounds.max.y);
        }

        minBounds = new Vector2(minX, minY);
        maxBounds = new Vector2(maxX, maxY);
    }

    // ��ȡ�����ı߽�
    private Bounds GetBackgroundBounds(GameObject background)
    {
        Renderer renderer = background.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }

        // ���û����Ⱦ�������Դ���ײ����ȡ
        Collider2D collider = background.GetComponent<Collider2D>();
        if (collider != null)
        {
            return collider.bounds;
        }

        // �����û�У�����Ĭ�ϱ߽�
        return new Bounds(background.transform.position, Vector3.one);
    }

    // ��ȡ�������
    private float GetBackgroundWidth()
    {
        if (backgrounds.Count > 0)
        {
            return GetBackgroundBounds(backgrounds[0]).size.x;
        }
        return 1f;
    }

    // ��ȡ�����߶�
    private float GetBackgroundHeight()
    {
        if (backgrounds.Count > 0)
        {
            return GetBackgroundBounds(backgrounds[0]).size.y;
        }
        return 1f;
    }

    // ��ȡ��ǰ�߽�
    public Vector2 GetMinBounds()
    {
        return minBounds;
    }

    public Vector2 GetMaxBounds()
    {
        return maxBounds;
    }

}

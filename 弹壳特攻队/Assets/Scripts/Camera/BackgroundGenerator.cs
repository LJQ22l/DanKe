using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    [SerializeField] private GameObject backgroundPrefab; // 背景预制体
    [SerializeField] private float detectionDistance = 2f; // 检测距离，玩家接近边缘多远时生成新背景

    private List<GameObject> backgrounds = new List<GameObject>(); // 已生成的背景
    private Vector2 minBounds; // 最小边界
    private Vector2 maxBounds; // 最大边界

    private void Start()
    {
        // 生成初始背景
        GenerateInitialBackground();
    }

    private void Update()
    {
        // 检查玩家位置并在需要时生成新背景
        CheckAndGenerateBackground();
    }

    // 生成初始背景
    private void GenerateInitialBackground()
    {
        // 生成第一个背景块
        GameObject initialBackground = Instantiate(backgroundPrefab, Vector3.zero, Quaternion.identity, transform);
        backgrounds.Add(initialBackground);

        // 更新边界
        UpdateBounds();
    }

    // 检查并生成新背景
    private void CheckAndGenerateBackground()
    {
        if (backgrounds.Count == 0) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;

        Vector3 playerPos = player.transform.position;

        // 检查是否接近右边界
        if (playerPos.x > maxBounds.x - detectionDistance)
        {
            GenerateBackgroundAt(new Vector3(maxBounds.x, playerPos.y, 0));
        }
        // 检查是否接近左边界
        else if (playerPos.x < minBounds.x + detectionDistance)
        {
            GenerateBackgroundAt(new Vector3(minBounds.x - GetBackgroundWidth(), playerPos.y, 0));
        }

        // 检查是否接近上边界
        if (playerPos.y > maxBounds.y - detectionDistance)
        {
            GenerateBackgroundAt(new Vector3(playerPos.x, maxBounds.y, 0));
        }
        // 检查是否接近下边界
        else if (playerPos.y < minBounds.y + detectionDistance)
        {
            GenerateBackgroundAt(new Vector3(playerPos.x, minBounds.y - GetBackgroundHeight(), 0));
        }
    }

    // 在指定位置生成背景
    private void GenerateBackgroundAt(Vector3 position)
    {
        // 检查该位置是否已有背景
        foreach (GameObject bg in backgrounds)
        {
            if (Vector3.Distance(bg.transform.position, position) < 0.1f)
                return;
        }

        // 生成新背景
        GameObject newBackground = Instantiate(backgroundPrefab, position, Quaternion.identity, transform);
        backgrounds.Add(newBackground);

        // 更新边界
        UpdateBounds();
    }

    // 更新边界
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

    // 获取背景的边界
    private Bounds GetBackgroundBounds(GameObject background)
    {
        Renderer renderer = background.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds;
        }

        // 如果没有渲染器，尝试从碰撞器获取
        Collider2D collider = background.GetComponent<Collider2D>();
        if (collider != null)
        {
            return collider.bounds;
        }

        // 如果都没有，返回默认边界
        return new Bounds(background.transform.position, Vector3.one);
    }

    // 获取背景宽度
    private float GetBackgroundWidth()
    {
        if (backgrounds.Count > 0)
        {
            return GetBackgroundBounds(backgrounds[0]).size.x;
        }
        return 1f;
    }

    // 获取背景高度
    private float GetBackgroundHeight()
    {
        if (backgrounds.Count > 0)
        {
            return GetBackgroundBounds(backgrounds[0]).size.y;
        }
        return 1f;
    }

    // 获取当前边界
    public Vector2 GetMinBounds()
    {
        return minBounds;
    }

    public Vector2 GetMaxBounds()
    {
        return maxBounds;
    }

}

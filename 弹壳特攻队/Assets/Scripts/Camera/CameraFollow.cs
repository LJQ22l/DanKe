using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // 跟随目标(玩家)
    [SerializeField] private float smoothSpeed = 0.125f; // 平滑跟随速度
    [SerializeField] private Vector3 offset; // 相机偏移量
    [SerializeField] private BackgroundGenerator backgroundGenerator; // 背景生成器引用

    private Vector2 minBounds; // 相机最小边界
    private Vector2 maxBounds; // 相机最大边界
    private float cameraHalfWidth; // 相机水平半宽
    private float cameraHalfHeight; // 相机垂直半高
    private bool isBackgroundReady = false; // 背景是否初始化完成

    private void Start()
    {
        // 检查引用是否设置
        if (target == null)
        {
            Debug.LogError("CameraFollow: 未设置跟随目标！");
            enabled = false;
            return;
        }

        if (backgroundGenerator == null)
        {
            Debug.LogError("CameraFollow: 未设置背景生成器引用！");
            enabled = false;
            return;
        }

        // 获取相机尺寸
        Camera mainCamera = GetComponent<Camera>();
        cameraHalfHeight = mainCamera.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        // 初始更新边界
        StartCoroutine(WaitForBackgroundInitialization());
    }

    // 等待背景初始化完成
    private IEnumerator WaitForBackgroundInitialization()
    {
        // 等待一帧确保背景已生成
        yield return null;

        UpdateBounds();
        isBackgroundReady = true;

        // 初始位置直接设置，避免过度平滑
        SetInitialPosition();
    }

    private void LateUpdate()
    {
        if (!isBackgroundReady || target == null) return;

        // 计算目标位置
        Vector3 desiredPosition = target.position + offset;

        // 限制相机在背景边界内
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x + cameraHalfWidth, maxBounds.x - cameraHalfWidth);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y + cameraHalfHeight, maxBounds.y - cameraHalfHeight);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

        // 平滑移动相机
        transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
    }

    // 设置初始位置（避免游戏开始时的延迟）
    private void SetInitialPosition()
    {
        Vector3 desiredPosition = target.position + offset;
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x + cameraHalfWidth, maxBounds.x - cameraHalfWidth);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y + cameraHalfHeight, maxBounds.y - cameraHalfHeight);
        transform.position = new Vector3(clampedX, clampedY, desiredPosition.z);
    }

    // 更新相机边界
    public void UpdateBounds()
    {
        if (backgroundGenerator != null)
        {
            minBounds = backgroundGenerator.GetMinBounds();
            maxBounds = backgroundGenerator.GetMaxBounds();

            // 确保边界有效
            if (maxBounds.x - minBounds.x < 2 * cameraHalfWidth ||
                maxBounds.y - minBounds.y < 2 * cameraHalfHeight)
            {
                Debug.LogWarning("CameraFollow: 背景尺寸过小，相机无法正确限制！");
            }
        }
    }
}

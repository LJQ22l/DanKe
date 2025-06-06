using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // ����Ŀ��(���)
    [SerializeField] private float smoothSpeed = 0.125f; // ƽ�������ٶ�
    [SerializeField] private Vector3 offset; // ���ƫ����
    [SerializeField] private BackgroundGenerator backgroundGenerator; // ��������������

    private Vector2 minBounds; // �����С�߽�
    private Vector2 maxBounds; // ������߽�
    private float cameraHalfWidth; // ���ˮƽ���
    private float cameraHalfHeight; // �����ֱ���
    private bool isBackgroundReady = false; // �����Ƿ��ʼ�����

    private void Start()
    {
        // ��������Ƿ�����
        if (target == null)
        {
            Debug.LogError("CameraFollow: δ���ø���Ŀ�꣡");
            enabled = false;
            return;
        }

        if (backgroundGenerator == null)
        {
            Debug.LogError("CameraFollow: δ���ñ������������ã�");
            enabled = false;
            return;
        }

        // ��ȡ����ߴ�
        Camera mainCamera = GetComponent<Camera>();
        cameraHalfHeight = mainCamera.orthographicSize;
        cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        // ��ʼ���±߽�
        StartCoroutine(WaitForBackgroundInitialization());
    }

    // �ȴ�������ʼ�����
    private IEnumerator WaitForBackgroundInitialization()
    {
        // �ȴ�һ֡ȷ������������
        yield return null;

        UpdateBounds();
        isBackgroundReady = true;

        // ��ʼλ��ֱ�����ã��������ƽ��
        SetInitialPosition();
    }

    private void LateUpdate()
    {
        if (!isBackgroundReady || target == null) return;

        // ����Ŀ��λ��
        Vector3 desiredPosition = target.position + offset;

        // ��������ڱ����߽���
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x + cameraHalfWidth, maxBounds.x - cameraHalfWidth);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y + cameraHalfHeight, maxBounds.y - cameraHalfHeight);
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, desiredPosition.z);

        // ƽ���ƶ����
        transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
    }

    // ���ó�ʼλ�ã�������Ϸ��ʼʱ���ӳ٣�
    private void SetInitialPosition()
    {
        Vector3 desiredPosition = target.position + offset;
        float clampedX = Mathf.Clamp(desiredPosition.x, minBounds.x + cameraHalfWidth, maxBounds.x - cameraHalfWidth);
        float clampedY = Mathf.Clamp(desiredPosition.y, minBounds.y + cameraHalfHeight, maxBounds.y - cameraHalfHeight);
        transform.position = new Vector3(clampedX, clampedY, desiredPosition.z);
    }

    // ��������߽�
    public void UpdateBounds()
    {
        if (backgroundGenerator != null)
        {
            minBounds = backgroundGenerator.GetMinBounds();
            maxBounds = backgroundGenerator.GetMaxBounds();

            // ȷ���߽���Ч
            if (maxBounds.x - minBounds.x < 2 * cameraHalfWidth ||
                maxBounds.y - minBounds.y < 2 * cameraHalfHeight)
            {
                Debug.LogWarning("CameraFollow: �����ߴ��С������޷���ȷ���ƣ�");
            }
        }
    }
}

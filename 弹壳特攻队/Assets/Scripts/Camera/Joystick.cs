using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("摇杆设置")]
    [SerializeField] private RectTransform background; // 摇杆背景(外圈)
    [SerializeField] private RectTransform handle;     // 摇杆手柄(内圈)
    [SerializeField] private float joystickRange = 50f; // 摇杆活动范围
    [SerializeField] private float moveSpeed = 5f;     // 角色移动速度

    [Header("角色引用")]
    [SerializeField] private Transform player;         // 玩家角色

    private Vector2 inputVector = Vector2.zero;        // 输入向量
    private Canvas canvas;                             // 画布引用
    private Camera mainCamera;                         // 主相机引用

    private void Start()
    {
        // 获取画布和相机引用
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;

        // 设置摇杆初始位置
        if (background && handle)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }

    private void Update()
    {
        // 移动玩家角色
        if (player && inputVector != Vector2.zero)
        {
            Vector3 moveDirection = new Vector3(inputVector.x, inputVector.y, 0f);
            player.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 开始拖动时，处理摇杆位置
        UpdateJoystickPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 拖动过程中，更新摇杆位置和输入向量
        UpdateJoystickPosition(eventData);

        // 计算输入向量
        Vector2 direction = handle.anchoredPosition.normalized;
        inputVector = direction * (handle.anchoredPosition.magnitude / joystickRange);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 结束拖动时，重置摇杆位置和输入向量
        handle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 指针进入时，如果正在触摸则更新位置
        if (eventData.pointerId >= 0)
        {
            UpdateJoystickPosition(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 指针离开时，重置摇杆位置和输入向量
        handle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }

    private void UpdateJoystickPosition(PointerEventData eventData)
    {
        if (!background || !handle || !canvas) return;

        // 将屏幕坐标转换为画布坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPoint
        );

        // 计算相对于背景中心的位置
        Vector2 center = Vector2.zero;
        Vector2 position = localPoint - center;

        // 限制摇杆在范围内
        position = Vector2.ClampMagnitude(position, joystickRange);

        // 更新摇杆位置
        handle.anchoredPosition = position;
    }
}

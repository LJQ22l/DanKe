using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("ҡ������")]
    [SerializeField] private RectTransform background; // ҡ�˱���(��Ȧ)
    [SerializeField] private RectTransform handle;     // ҡ���ֱ�(��Ȧ)
    [SerializeField] private float joystickRange = 50f; // ҡ�˻��Χ
    [SerializeField] private float moveSpeed = 5f;     // ��ɫ�ƶ��ٶ�

    [Header("��ɫ����")]
    [SerializeField] private Transform player;         // ��ҽ�ɫ

    private Vector2 inputVector = Vector2.zero;        // ��������
    private Canvas canvas;                             // ��������
    private Camera mainCamera;                         // ���������

    private void Start()
    {
        // ��ȡ�������������
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;

        // ����ҡ�˳�ʼλ��
        if (background && handle)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }

    private void Update()
    {
        // �ƶ���ҽ�ɫ
        if (player && inputVector != Vector2.zero)
        {
            Vector3 moveDirection = new Vector3(inputVector.x, inputVector.y, 0f);
            player.Translate(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ��ʼ�϶�ʱ������ҡ��λ��
        UpdateJoystickPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // �϶������У�����ҡ��λ�ú���������
        UpdateJoystickPosition(eventData);

        // ������������
        Vector2 direction = handle.anchoredPosition.normalized;
        inputVector = direction * (handle.anchoredPosition.magnitude / joystickRange);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // �����϶�ʱ������ҡ��λ�ú���������
        handle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // ָ�����ʱ��������ڴ��������λ��
        if (eventData.pointerId >= 0)
        {
            UpdateJoystickPosition(eventData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // ָ���뿪ʱ������ҡ��λ�ú���������
        handle.anchoredPosition = Vector2.zero;
        inputVector = Vector2.zero;
    }

    private void UpdateJoystickPosition(PointerEventData eventData)
    {
        if (!background || !handle || !canvas) return;

        // ����Ļ����ת��Ϊ��������
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out Vector2 localPoint
        );

        // ��������ڱ������ĵ�λ��
        Vector2 center = Vector2.zero;
        Vector2 position = localPoint - center;

        // ����ҡ���ڷ�Χ��
        position = Vector2.ClampMagnitude(position, joystickRange);

        // ����ҡ��λ��
        handle.anchoredPosition = position;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance { get; private set; }

    public GameObject floatingTextPrefab;
    public Transform canvasTransform;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CreateFloatingText(string text, Vector3 worldPosition, Color color)
    {
        if (floatingTextPrefab == null || canvasTransform == null) return;

        // ����������ת��Ϊ��Ļ����
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // ʵ���������ı�
        GameObject floatingText = Instantiate(floatingTextPrefab, screenPosition, Quaternion.identity, canvasTransform);
        Text textComponent = floatingText.GetComponent<Text>();

        if (textComponent != null)
        {
            textComponent.text = text;
            textComponent.color = color;
        }

        // �����Զ�����
        Destroy(floatingText, 1.5f);
    }
}
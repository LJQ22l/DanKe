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

        // 将世界坐标转换为屏幕坐标
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

        // 实例化浮动文本
        GameObject floatingText = Instantiate(floatingTextPrefab, screenPosition, Quaternion.identity, canvasTransform);
        Text textComponent = floatingText.GetComponent<Text>();

        if (textComponent != null)
        {
            textComponent.text = text;
            textComponent.color = color;
        }

        // 设置自动销毁
        Destroy(floatingText, 1.5f);
    }
}
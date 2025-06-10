using UnityEngine;
using UnityEngine.UI;

public class LevelUpUIManager : MonoBehaviour
{
    public static LevelUpUIManager Instance { get; private set; }

    [Header("UI���")]
    public GameObject levelUpPanel;     // �������
    public Text levelText;              // ��ʾ�ȼ����ı�
    public Button continueButton;       // ������ť
    public Text statsInfoText;          // ��ʾ�����������ı�

    private bool isLevelingUp = false;

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

        // ��ʼ����ť�¼�
        continueButton.onClick.AddListener(ContinueGame);

        // �����������
        levelUpPanel.SetActive(false);
    }

    // ��ʾ��������
    public void ShowLevelUpUI(int newLevel, string statsInfo)
    {
        isLevelingUp = true;
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; // ��ͣ��Ϸ

        // ���½�����Ϣ
        levelText.text = $"LEVEL UP!\n������{newLevel}��";
        statsInfoText.text = statsInfo;
    }

    // ������Ϸ
    public void ContinueGame()
    {
        isLevelingUp = false;
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // �ָ���Ϸ
    }

    // �ж��Ƿ���������
    public bool IsLevelingUp()
    {
        return isLevelingUp;
    }
}
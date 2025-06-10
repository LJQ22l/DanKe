using UnityEngine;
using UnityEngine.UI;

public class LevelUpUIManager : MonoBehaviour
{
    public static LevelUpUIManager Instance { get; private set; }

    [Header("UI组件")]
    public GameObject levelUpPanel;     // 升级面板
    public Text levelText;              // 显示等级的文本
    public Button continueButton;       // 继续按钮
    public Text statsInfoText;          // 显示属性提升的文本

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

        // 初始化按钮事件
        continueButton.onClick.AddListener(ContinueGame);

        // 隐藏升级面板
        levelUpPanel.SetActive(false);
    }

    // 显示升级界面
    public void ShowLevelUpUI(int newLevel, string statsInfo)
    {
        isLevelingUp = true;
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; // 暂停游戏

        // 更新界面信息
        levelText.text = $"LEVEL UP!\n现在是{newLevel}级";
        statsInfoText.text = statsInfo;
    }

    // 继续游戏
    public void ContinueGame()
    {
        isLevelingUp = false;
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // 恢复游戏
    }

    // 判断是否正在升级
    public bool IsLevelingUp()
    {
        return isLevelingUp;
    }
}
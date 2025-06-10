using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerExperience : MonoBehaviour
{
    public static PlayerExperience Instance { get; private set; }

    [Header("经验值设置")]
    public int currentLevel = 1;
    public int maxLevel = 99;
    public int currentExperience = 0;
    public int baseExperience = 100;  // 1级升2级所需经验
    public float experienceGrowthRate = 1.2f;  // 经验增长系数

    [Header("UI引用")]
    public Slider experienceSlider;
    public Text levelText;
    public GameObject levelUpEffect;
    public Transform levelUpEffectPosition;

    [Header("升级奖励")]
    public int healthIncreasePerLevel = 10;
    public float attackIncreasePerLevel = 0.5f;

    [Header("动画设置")]
    public float experienceFillSpeed = 5f;  // 经验条填充速度
    public float levelUpDisplayTime = 2f;  // 升级提示显示时间

    private int experienceToNextLevel;
    private float targetSliderValue;
    private bool isLevelingUp = false;

    public delegate void OnLevelUpDelegate(int newLevel);
    public event OnLevelUpDelegate OnLevelUp;

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

        CalculateExperienceToNextLevel();
    }

    void Start()
    {
        UpdateExperienceUI(true);
    }

    void Update()
    {
        if (experienceSlider != null && !Mathf.Approximately(experienceSlider.value, targetSliderValue))
        {
            // 平滑更新经验条
            experienceSlider.value = Mathf.MoveTowards(
                experienceSlider.value,
                targetSliderValue,
                experienceFillSpeed * Time.deltaTime
            );
        }
    }

    // 添加经验值
    public void AddExperience(int amount)
    {
        if (currentLevel >= maxLevel)
        {
            ShowMaxLevelMessage();
            return;
        }

        currentExperience += amount;
        ShowExperiencePopup(amount);

        // 更新UI
        UpdateExperienceUI(false);

        // 检查是否升级
        while (currentExperience >= experienceToNextLevel && currentLevel < maxLevel)
        {
            LevelUp();
        }
    }

    // 升级处理
    private void LevelUp()
    {
        if (isLevelingUp) return;
        isLevelingUp = true;

        // 计算溢出的经验
        int overflowExperience = currentExperience - experienceToNextLevel;

        // 提升等级
        currentLevel++;
        currentExperience = overflowExperience;
        CalculateExperienceToNextLevel();

        // 更新UI
        UpdateExperienceUI(true);

        // 应用升级奖励
        ApplyLevelUpBonuses();

        // 显示升级效果
        ShowLevelUpEffect();

        // 显示升级界面
        if (LevelUpUIManager.Instance != null)
        {
            string statsInfo = $"生命值+{healthIncreasePerLevel}\n攻击力+{attackIncreasePerLevel:F1}";
            LevelUpUIManager.Instance.ShowLevelUpUI(currentLevel, statsInfo);
        }

        // 重置升级状态
        StartCoroutine(ResetLevelingUpAfterDelay());
    }

    // 计算升级所需经验
    private void CalculateExperienceToNextLevel()
    {
        if (currentLevel >= maxLevel)
        {
            experienceToNextLevel = 0;
            currentExperience = 0;
        }
        else
        {
            experienceToNextLevel = Mathf.FloorToInt(baseExperience * Mathf.Pow(experienceGrowthRate, currentLevel - 1));
        }
    }

    // 更新经验UI
    private void UpdateExperienceUI(bool immediate = false)
    {
        if (levelText != null)
        {
            levelText.text = $"Lv.{currentLevel}";
        }

        if (experienceSlider != null)
        {
            experienceSlider.maxValue = experienceToNextLevel > 0 ? experienceToNextLevel : 1;
            targetSliderValue = currentExperience;

            if (immediate)
            {
                experienceSlider.value = targetSliderValue;
            }
        }
    }

    // 应用升级奖励
    private void ApplyLevelUpBonuses()
    {
        // 增加最大生命值
        PlayerStats.Instance?.IncreaseMaxHealth(healthIncreasePerLevel);

        // 恢复满生命值
        PlayerStats.Instance?.Heal(PlayerStats.Instance.maxHealth);

        // 增加攻击力
        WeaponController weapon = FindObjectOfType<WeaponController>();
        if (weapon != null)
        {
            weapon.IncreaseDamage(attackIncreasePerLevel);
        }
    }

    // 显示升级效果
    private void ShowLevelUpEffect()
    {
        if (levelUpEffect != null && levelUpEffectPosition != null)
        {
            GameObject effect = Instantiate(levelUpEffect, levelUpEffectPosition.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // 播放升级音效
       // AudioManager.Instance?.PlayLevelUpSound();
    }

    // 显示获得经验的浮动文本
    private void ShowExperiencePopup(int amount)
    {
        // 这里可以实例化一个浮动文本UI
        FloatingTextManager.Instance?.CreateFloatingText($"+{amount} EXP", transform.position + Vector3.up * 2f, Color.yellow);
    }

    // 显示满级消息
    private void ShowMaxLevelMessage()
    {
        FloatingTextManager.Instance?.CreateFloatingText("已达到最高等级!", transform.position + Vector3.up * 2f, Color.red);
    }

    // 延迟重置升级状态
    IEnumerator ResetLevelingUpAfterDelay()
    {
        yield return new WaitForSeconds(levelUpDisplayTime);
        isLevelingUp = false;
    }
}
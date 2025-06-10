using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerExperience : MonoBehaviour
{
    public static PlayerExperience Instance { get; private set; }

    [Header("����ֵ����")]
    public int currentLevel = 1;
    public int maxLevel = 99;
    public int currentExperience = 0;
    public int baseExperience = 100;  // 1����2�����辭��
    public float experienceGrowthRate = 1.2f;  // ��������ϵ��

    [Header("UI����")]
    public Slider experienceSlider;
    public Text levelText;
    public GameObject levelUpEffect;
    public Transform levelUpEffectPosition;

    [Header("��������")]
    public int healthIncreasePerLevel = 10;
    public float attackIncreasePerLevel = 0.5f;

    [Header("��������")]
    public float experienceFillSpeed = 5f;  // ����������ٶ�
    public float levelUpDisplayTime = 2f;  // ������ʾ��ʾʱ��

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
            // ƽ�����¾�����
            experienceSlider.value = Mathf.MoveTowards(
                experienceSlider.value,
                targetSliderValue,
                experienceFillSpeed * Time.deltaTime
            );
        }
    }

    // ��Ӿ���ֵ
    public void AddExperience(int amount)
    {
        if (currentLevel >= maxLevel)
        {
            ShowMaxLevelMessage();
            return;
        }

        currentExperience += amount;
        ShowExperiencePopup(amount);

        // ����UI
        UpdateExperienceUI(false);

        // ����Ƿ�����
        while (currentExperience >= experienceToNextLevel && currentLevel < maxLevel)
        {
            LevelUp();
        }
    }

    // ��������
    private void LevelUp()
    {
        if (isLevelingUp) return;
        isLevelingUp = true;

        // ��������ľ���
        int overflowExperience = currentExperience - experienceToNextLevel;

        // �����ȼ�
        currentLevel++;
        currentExperience = overflowExperience;
        CalculateExperienceToNextLevel();

        // ����UI
        UpdateExperienceUI(true);

        // Ӧ����������
        ApplyLevelUpBonuses();

        // ��ʾ����Ч��
        ShowLevelUpEffect();

        // ��ʾ��������
        if (LevelUpUIManager.Instance != null)
        {
            string statsInfo = $"����ֵ+{healthIncreasePerLevel}\n������+{attackIncreasePerLevel:F1}";
            LevelUpUIManager.Instance.ShowLevelUpUI(currentLevel, statsInfo);
        }

        // ��������״̬
        StartCoroutine(ResetLevelingUpAfterDelay());
    }

    // �����������辭��
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

    // ���¾���UI
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

    // Ӧ����������
    private void ApplyLevelUpBonuses()
    {
        // �����������ֵ
        PlayerStats.Instance?.IncreaseMaxHealth(healthIncreasePerLevel);

        // �ָ�������ֵ
        PlayerStats.Instance?.Heal(PlayerStats.Instance.maxHealth);

        // ���ӹ�����
        WeaponController weapon = FindObjectOfType<WeaponController>();
        if (weapon != null)
        {
            weapon.IncreaseDamage(attackIncreasePerLevel);
        }
    }

    // ��ʾ����Ч��
    private void ShowLevelUpEffect()
    {
        if (levelUpEffect != null && levelUpEffectPosition != null)
        {
            GameObject effect = Instantiate(levelUpEffect, levelUpEffectPosition.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // ����������Ч
       // AudioManager.Instance?.PlayLevelUpSound();
    }

    // ��ʾ��þ���ĸ����ı�
    private void ShowExperiencePopup(int amount)
    {
        // �������ʵ����һ�������ı�UI
        FloatingTextManager.Instance?.CreateFloatingText($"+{amount} EXP", transform.position + Vector3.up * 2f, Color.yellow);
    }

    // ��ʾ������Ϣ
    private void ShowMaxLevelMessage()
    {
        FloatingTextManager.Instance?.CreateFloatingText("�Ѵﵽ��ߵȼ�!", transform.position + Vector3.up * 2f, Color.red);
    }

    // �ӳ���������״̬
    IEnumerator ResetLevelingUpAfterDelay()
    {
        yield return new WaitForSeconds(levelUpDisplayTime);
        isLevelingUp = false;
    }
}
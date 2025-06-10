using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("基础属性")]
    public int maxHealth = 100;
    public int currentHealth;
    public int coins = 0;

    [Header("UI引用")]
    public Slider healthSlider;
    public Text coinText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentHealth = maxHealth;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 受到伤害
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth == 0)
        {
            Die();
        }
    }

    // 恢复生命值
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHealthUI();
    }

    // 增加最大生命值
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        UpdateHealthUI();
    }

    // 添加金币
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinUI();
    }

    // 更新生命值UI
    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // 更新金币UI
    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
    }

    // 玩家死亡
    private void Die()
    {
        // 播放死亡动画
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // 禁用玩家控制
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // 显示死亡菜单或重生
        //GameManager.Instance?.ShowGameOverMenu();
    }
}
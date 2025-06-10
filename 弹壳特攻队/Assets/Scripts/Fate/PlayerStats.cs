using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("��������")]
    public int maxHealth = 100;
    public int currentHealth;
    public int coins = 0;

    [Header("UI����")]
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

    // �ܵ��˺�
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

    // �ָ�����ֵ
    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHealthUI();
    }

    // �����������ֵ
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        UpdateHealthUI();
    }

    // ��ӽ��
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateCoinUI();
    }

    // ��������ֵUI
    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    // ���½��UI
    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = coins.ToString();
        }
    }

    // �������
    private void Die()
    {
        // ������������
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // ������ҿ���
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        // ��ʾ�����˵�������
        //GameManager.Instance?.ShowGameOverMenu();
    }
}
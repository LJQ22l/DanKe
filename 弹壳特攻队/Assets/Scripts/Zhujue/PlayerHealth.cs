using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // 受到伤害的方法
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // 玩家死亡的方法
    private void Die()
    {
        // 这里可以添加玩家死亡逻辑（如游戏结束、重生等）
        Debug.Log("Player died!");
    }
}
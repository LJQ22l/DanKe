using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // �ܵ��˺��ķ���
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // ��������ķ���
    private void Die()
    {
        // ������������������߼�������Ϸ�����������ȣ�
        Debug.Log("Player died!");
    }
}
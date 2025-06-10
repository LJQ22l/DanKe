using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("��������")]
    public float health = 100f;
    public float moveSpeed = 3f;
    public float attackDamage = 10f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public int experienceValue = 10; // ����ʱ����ľ���ֵ

    [Header("��������")]
    public GameObject coinPrefab;
    public int minCoins = 1;
    public int maxCoins = 3;

    private Transform player;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private Animator animator;
    private bool isAlive = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        // �������Ƕ���
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("�Ҳ�����Ҷ��󣬵����޷�׷�٣�");
    }

    void Update()
    {
        if (!isAlive || player == null) return;

        // ��������ҵľ���
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
        else
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (player == null) return;

        // �����ƶ�����
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        // ���¶���״̬
        if (animator != null)
            animator.SetBool("isMoving", true);
    }

    private void Attack()
    {
        lastAttackTime = Time.time;

        // ���Ź�������
        if (animator != null)
            animator.SetTrigger("Attack");

        // ���������˺�
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;

        health -= damage;

        // ����������Ч����Ч
        if (animator != null)
            animator.SetTrigger("Hurt");

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;
        rb.velocity = Vector2.zero;

        // ������ײ������ֹ��������
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        // ������������
        if (animator != null)
        {
            animator.SetTrigger("Die");

            // �ӳٻ����Բ��Ŷ���
            ObjectPool.Instance.Despawn(gameObject, 0.5f);
        }
        else
        {
            // û�ж���ʱ��������
            ObjectPool.Instance.Despawn(gameObject);
        }

        // ���ɵ�����Ʒ
        SpawnCoins();

        // ������Ҿ���
        PlayerExperience playerExp = FindObjectOfType<PlayerExperience>();
        if (playerExp != null)
            playerExp.AddExperience(experienceValue);
    }

    private void SpawnCoins()
    {
        int coinCount = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < coinCount; i++)
        {
            Vector2 spawnPos = transform.position + new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f),
                0
            );

            ObjectPool.Instance.Spawn(coinPrefab, spawnPos, Quaternion.identity);
        }
    }
}
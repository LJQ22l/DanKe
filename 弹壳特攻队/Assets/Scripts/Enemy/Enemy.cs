using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("基本属性")]
    public float health = 100f;
    public float moveSpeed = 3f;
    public float attackDamage = 10f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public int experienceValue = 10; // 死亡时给予的经验值

    [Header("掉落配置")]
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
        // 查找主角对象
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("找不到玩家对象，敌人无法追踪！");
    }

    void Update()
    {
        if (!isAlive || player == null) return;

        // 计算与玩家的距离
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

        // 计算移动方向
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;

        // 更新动画状态
        if (animator != null)
            animator.SetBool("isMoving", true);
    }

    private void Attack()
    {
        lastAttackTime = Time.time;

        // 播放攻击动画
        if (animator != null)
            animator.SetTrigger("Attack");

        // 对玩家造成伤害
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;

        health -= damage;

        // 播放受伤特效或音效
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

        // 禁用碰撞器，防止继续攻击
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;

        // 播放死亡动画
        if (animator != null)
        {
            animator.SetTrigger("Die");

            // 延迟回收以播放动画
            ObjectPool.Instance.Despawn(gameObject, 0.5f);
        }
        else
        {
            // 没有动画时立即回收
            ObjectPool.Instance.Despawn(gameObject);
        }

        // 生成掉落物品
        SpawnCoins();

        // 增加玩家经验
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
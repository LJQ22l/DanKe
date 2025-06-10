using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;         // 硬币价值
    public int experienceValue = 5; // 经验值
    public float collectSpeed = 5f; // 收集速度
    public float collectDistance = 1f; // 自动收集距离

    private Transform player;
    private bool isCollected = false;

    void Start()
    {
        // 查找玩家
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("找不到玩家，硬币无法自动收集");
        }
    }

    void Update()
    {
        if (player == null || isCollected) return;

        // 检查是否接近玩家
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= collectDistance)
        {
            Collect();
        }
    }

    void Collect()
    {
        isCollected = true;

        // 移动到玩家位置
        StartCoroutine(MoveToPlayer());
    }

    System.Collections.IEnumerator MoveToPlayer()
    {
        while (Vector2.Distance(transform.position, player.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.position,
                collectSpeed * Time.deltaTime
            );
            yield return null;
        }

        // 添加到玩家金币和经验
        PlayerStats.Instance?.AddCoins(value);
        PlayerExperience.Instance?.AddExperience(experienceValue);

        // 播放收集音效
        //AudioManager.Instance?.PlayCoinCollectSound();

        // 回收硬币
        ObjectPool.Instance.Despawn(gameObject);
    }
}
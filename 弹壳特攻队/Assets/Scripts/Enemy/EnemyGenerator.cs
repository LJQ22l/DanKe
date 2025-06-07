using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject enemyPrefab; // 敌人预制体
    public float spawnRate = 2f; // 敌人生成速率
    public float detectionRange = 10f; // 敌人生成范围
    public LayerMask playerLayer; // 玩家图层

    private float nextSpawnTime = 0f; // 下次生成时间

    void Update()
    {
        // 如果可以生成敌人
        if (Time.time >= nextSpawnTime)
        {
            // 检查玩家是否在范围内
            Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, detectionRange, playerLayer);

            if (players.Length > 0)
            {
                // 生成敌人
                SpawnEnemy();

                // 更新下次生成时间
                nextSpawnTime = Time.time + 1f / spawnRate;
            }
        }
    }

    // 生成敌人的方法
    private void SpawnEnemy()
    {
        // 获取随机位置
        Vector3 randomPosition = new Vector3(
            transform.position.x + Random.Range(-detectionRange, detectionRange),
            transform.position.y + Random.Range(-detectionRange, detectionRange),
            0f
        );

        // 实例化敌人
        Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
    }
}
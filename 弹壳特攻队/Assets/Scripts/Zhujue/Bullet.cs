using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private Vector2 velocity;
    private int level;
    private float lifeTime = 1f;  // 子弹生命周期（秒）
    private float spawnTime;      // 生成时间

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        // 移动子弹
        transform.Translate(velocity * Time.deltaTime);

        // 检查是否超过生命周期
        if (Time.time - spawnTime >= lifeTime)
        {
            ObjectPool.Instance.Despawn(gameObject);
        }
    }

    public void Setup(float damage, Vector2 velocity, int level)
    {
        this.damage = damage;
        this.velocity = velocity;
        this.level = level;
        spawnTime = Time.time;  // 重置生成时间

        // 更新子弹外观
        UpdateBulletVisuals();
    }

    private void UpdateBulletVisuals()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // 根据等级调整子弹大小和颜色
            sr.transform.localScale = Vector3.one * (0.5f + level * 0.1f);

            switch (level)
            {
                case 1: sr.color = Color.white; break;
                case 2: sr.color = Color.yellow; break;
                case 3: sr.color = Color.red; break;
                default: sr.color = Color.magenta; break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 击中敌人
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // 播放击中音效
                //AudioManager.Instance?.PlayBulletHitSound();
            }

            // 回收子弹
            ObjectPool.Instance.Despawn(gameObject);
        }
        //// 击中障碍物
        //else if (collision.CompareTag("Obstacle"))
        //{
        //    // 播放障碍物击中音效
        //    AudioManager.Instance?.PlayObstacleHitSound();

        //    // 回收子弹
        //    ObjectPool.Instance.Despawn(gameObject);
        //}
    }
}
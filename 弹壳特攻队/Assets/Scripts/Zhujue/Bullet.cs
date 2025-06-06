using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private float damage;
    private Vector2 velocity;
    private int level;

    // 子弹外观
    private SpriteRenderer spriteRenderer;
    public Sprite[] levelSprites;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 设置子弹属性
    public void Setup(float damage, Vector2 velocity, int level)
    {
        this.damage = damage;
        this.velocity = velocity;
        this.level = level;

        // 更新子弹外观
        UpdateBulletVisuals();
    }

    void Update()
    {
        // 移动子弹
        transform.Translate(velocity * Time.deltaTime);
    }

    // 更新子弹外观
    private void UpdateBulletVisuals()
    {
        if (spriteRenderer && levelSprites.Length > 0)
        {
            int spriteIndex = Mathf.Clamp(level - 1, 0, levelSprites.Length - 1);
            spriteRenderer.sprite = levelSprites[spriteIndex];

            // 根据等级调整大小
            transform.localScale = Vector3.one * (1 + (level - 1) * 0.2f);
        }
    }

    //碰撞检测
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 如果碰撞到敌人
        if (collision.CompareTag("Enemy"))
        {
            // 造成伤害
            //Enemy enemy = collision.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(damage);
            //}

            // 播放击中音效
            //AudioManager.Instance.PlayBulletHitSound();

            // 回收子弹
            ObjectPool.Instance.Despawn(gameObject);
        }
        // 如果碰撞到障碍物
        //else if (collision.CompareTag("Obstacle"))
        //{
        //    // 播放击中障碍物音效
        //    //AudioManager.Instance.PlayObstacleHitSound();

        //    // 回收子弹
        //    ObjectPool.Instance.Despawn(gameObject);
        //}
    }
}
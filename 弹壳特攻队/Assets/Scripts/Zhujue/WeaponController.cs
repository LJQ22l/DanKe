using UnityEngine;
using UnityEngine.Pool;

public class WeaponController : MonoBehaviour
{
    // 武器配置
    [Header("武器配置")]
    public float fireRate = 0.5f;    // 发射间隔
    public float bulletSpeed = 10f;  // 子弹速度
    public float damage = 10f;       // 子弹伤害
    public GameObject bulletPrefab;  // 子弹预制体

    // 发射点
    [Header("发射点")]
    public Transform firePoint;      // 子弹发射位置

    // 武器外观
    [Header("武器外观")]
    public SpriteRenderer weaponRenderer;
    public Sprite[] weaponSprites;   // 不同等级的武器外观

    // 音效
    [Header("音效")]
    public AudioSource audioSource;
    public AudioClip[] fireSounds;   // 不同武器等级的射击音效

    // 状态
    private float nextFireTime = 0f; // 下次发射时间
    private bool canFire = true;     // 是否可以发射
    private int currentLevel = 1;    // 当前武器等级

    // 敌人检测
    [Header("敌人检测")]
    public float detectionRange = 10f; // 敌人检测范围
    public LayerMask enemyLayer;       // 敌人图层

    void Start()
    {
        // 初始化武器外观
        UpdateWeaponVisuals(currentLevel);
    }

    void Update()
    {
        // 自动检测并攻击敌人
        if (canFire && Time.time >= nextFireTime)
        {
            // 检测前方是否有敌人
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);

            if (enemies.Length > 0)
            {
                // 找到最近的敌人
                Collider2D closestEnemy = GetClosestEnemy(enemies);

                // 计算射击方向
                Vector2 direction = (closestEnemy.transform.position - transform.position).normalized;

                // 发射子弹
                FireBullet(direction);

                // 更新下次发射时间
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    // 获取最近的敌人
    private Collider2D GetClosestEnemy(Collider2D[] enemies)
    {
        Collider2D closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb == null) continue;

            // 将 Vector3 转换为 Vector2 进行计算
            Vector2 enemyPosition = enemy.transform.position;
            Vector2 enemyVelocity = enemyRb.velocity;
            Vector2 myPosition = transform.position;

            // 预测敌人位置
            float flightTime = Vector2.Distance(myPosition, enemyPosition) / bulletSpeed;
            Vector2 predictedPosition = enemyPosition + (enemyVelocity * flightTime);

            float distance = Vector2.Distance(myPosition, predictedPosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    // 发射子弹
    private void FireBullet(Vector2 direction)
    {
        // 实例化子弹
        GameObject bullet = ObjectPool.Instance.Spawn(bulletPrefab, firePoint.position, Quaternion.identity);

        // 设置子弹属性
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.Setup(damage, direction * bulletSpeed, currentLevel);
        }

        // 播放射击特效和音效
        PlayFireEffect();
        PlayFireSound();
    }

    // 播放射击特效
    private void PlayFireEffect()
    {
        // 这里可以添加武器射击的特效
        // 例如：Instantiate(fireEffect, firePoint.position, firePoint.rotation);
    }

    // 播放射击音效
    private void PlayFireSound()
    {
        if (audioSource && fireSounds.Length > 0)
        {
            // 根据武器等级选择对应的音效
            int soundIndex = Mathf.Clamp(currentLevel - 1, 0, fireSounds.Length - 1);
            audioSource.PlayOneShot(fireSounds[soundIndex]);
        }
    }

    // 武器升级方法
    public void UpgradeWeapon(int level)
    {
        currentLevel = level;

        // 根据等级提升武器属性
        fireRate = Mathf.Max(0.1f, fireRate * (1 - (level - 1) * 0.1f));
        damage *= 1 + (level - 1) * 0.2f;
        bulletSpeed *= 1 + (level - 1) * 0.1f;

        // 更新武器外观和音效
        UpdateWeaponVisuals(level);
    }

    // 更新武器外观
    private void UpdateWeaponVisuals(int level)
    {
        if (weaponRenderer && weaponSprites.Length > 0)
        {
            // 根据武器等级选择对应的外观
            int spriteIndex = Mathf.Clamp(level - 1, 0, weaponSprites.Length - 1);
            weaponRenderer.sprite = weaponSprites[spriteIndex];

            // 可以添加颜色或大小变化
            weaponRenderer.color = GetColorForLevel(level);
            weaponRenderer.transform.localScale = Vector3.one * (1 + (level - 1) * 0.1f);
        }
    }

    // 根据武器等级获取颜色
    private Color GetColorForLevel(int level)
    {
        switch (level)
        {
            case 1: return Color.white;      // 普通
            case 2: return new Color(0.8f, 0.8f, 0, 1);  // 稀有(黄色)
            case 3: return new Color(0.8f, 0, 0.8f, 1);  // 史诗(紫色)
            default: return new Color(1, 0.5f, 0, 1);    // 传说(橙色)
        }
    }
    public void IncreaseDamage(float amount)
    {
        damage += amount;
        ShowDamageIncreasePopup(amount);
    }

    // 显示攻击力提升提示
    private void ShowDamageIncreasePopup(float amount)
    {
        FloatingTextManager.Instance?.CreateFloatingText($"攻击力 +{amount:F1}", transform.position + Vector3.up * 1.5f, Color.cyan);
    }
}
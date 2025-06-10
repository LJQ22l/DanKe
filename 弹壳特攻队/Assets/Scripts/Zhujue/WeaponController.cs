using UnityEngine;
using UnityEngine.Pool;

public class WeaponController : MonoBehaviour
{
    // ��������
    [Header("��������")]
    public float fireRate = 0.5f;    // ������
    public float bulletSpeed = 10f;  // �ӵ��ٶ�
    public float damage = 10f;       // �ӵ��˺�
    public GameObject bulletPrefab;  // �ӵ�Ԥ����

    // �����
    [Header("�����")]
    public Transform firePoint;      // �ӵ�����λ��

    // �������
    [Header("�������")]
    public SpriteRenderer weaponRenderer;
    public Sprite[] weaponSprites;   // ��ͬ�ȼ����������

    // ��Ч
    [Header("��Ч")]
    public AudioSource audioSource;
    public AudioClip[] fireSounds;   // ��ͬ�����ȼ��������Ч

    // ״̬
    private float nextFireTime = 0f; // �´η���ʱ��
    private bool canFire = true;     // �Ƿ���Է���
    private int currentLevel = 1;    // ��ǰ�����ȼ�

    // ���˼��
    [Header("���˼��")]
    public float detectionRange = 10f; // ���˼�ⷶΧ
    public LayerMask enemyLayer;       // ����ͼ��

    void Start()
    {
        // ��ʼ���������
        UpdateWeaponVisuals(currentLevel);
    }

    void Update()
    {
        // �Զ���Ⲣ��������
        if (canFire && Time.time >= nextFireTime)
        {
            // ���ǰ���Ƿ��е���
            Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);

            if (enemies.Length > 0)
            {
                // �ҵ�����ĵ���
                Collider2D closestEnemy = GetClosestEnemy(enemies);

                // �����������
                Vector2 direction = (closestEnemy.transform.position - transform.position).normalized;

                // �����ӵ�
                FireBullet(direction);

                // �����´η���ʱ��
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
    }

    // ��ȡ����ĵ���
    private Collider2D GetClosestEnemy(Collider2D[] enemies)
    {
        Collider2D closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider2D enemy in enemies)
        {
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            if (enemyRb == null) continue;

            // �� Vector3 ת��Ϊ Vector2 ���м���
            Vector2 enemyPosition = enemy.transform.position;
            Vector2 enemyVelocity = enemyRb.velocity;
            Vector2 myPosition = transform.position;

            // Ԥ�����λ��
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

    // �����ӵ�
    private void FireBullet(Vector2 direction)
    {
        // ʵ�����ӵ�
        GameObject bullet = ObjectPool.Instance.Spawn(bulletPrefab, firePoint.position, Quaternion.identity);

        // �����ӵ�����
        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.Setup(damage, direction * bulletSpeed, currentLevel);
        }

        // ���������Ч����Ч
        PlayFireEffect();
        PlayFireSound();
    }

    // ���������Ч
    private void PlayFireEffect()
    {
        // ���������������������Ч
        // ���磺Instantiate(fireEffect, firePoint.position, firePoint.rotation);
    }

    // ���������Ч
    private void PlayFireSound()
    {
        if (audioSource && fireSounds.Length > 0)
        {
            // ���������ȼ�ѡ���Ӧ����Ч
            int soundIndex = Mathf.Clamp(currentLevel - 1, 0, fireSounds.Length - 1);
            audioSource.PlayOneShot(fireSounds[soundIndex]);
        }
    }

    // ������������
    public void UpgradeWeapon(int level)
    {
        currentLevel = level;

        // ���ݵȼ�������������
        fireRate = Mathf.Max(0.1f, fireRate * (1 - (level - 1) * 0.1f));
        damage *= 1 + (level - 1) * 0.2f;
        bulletSpeed *= 1 + (level - 1) * 0.1f;

        // ����������ۺ���Ч
        UpdateWeaponVisuals(level);
    }

    // �����������
    private void UpdateWeaponVisuals(int level)
    {
        if (weaponRenderer && weaponSprites.Length > 0)
        {
            // ���������ȼ�ѡ���Ӧ�����
            int spriteIndex = Mathf.Clamp(level - 1, 0, weaponSprites.Length - 1);
            weaponRenderer.sprite = weaponSprites[spriteIndex];

            // ���������ɫ���С�仯
            weaponRenderer.color = GetColorForLevel(level);
            weaponRenderer.transform.localScale = Vector3.one * (1 + (level - 1) * 0.1f);
        }
    }

    // ���������ȼ���ȡ��ɫ
    private Color GetColorForLevel(int level)
    {
        switch (level)
        {
            case 1: return Color.white;      // ��ͨ
            case 2: return new Color(0.8f, 0.8f, 0, 1);  // ϡ��(��ɫ)
            case 3: return new Color(0.8f, 0, 0.8f, 1);  // ʷʫ(��ɫ)
            default: return new Color(1, 0.5f, 0, 1);    // ��˵(��ɫ)
        }
    }
    public void IncreaseDamage(float amount)
    {
        damage += amount;
        ShowDamageIncreasePopup(amount);
    }

    // ��ʾ������������ʾ
    private void ShowDamageIncreasePopup(float amount)
    {
        FloatingTextManager.Instance?.CreateFloatingText($"������ +{amount:F1}", transform.position + Vector3.up * 1.5f, Color.cyan);
    }
}
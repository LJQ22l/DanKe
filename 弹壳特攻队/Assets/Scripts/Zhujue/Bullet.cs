using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private Vector2 velocity;
    private int level;
    private float lifeTime = 1f;  // �ӵ��������ڣ��룩
    private float spawnTime;      // ����ʱ��

    void Start()
    {
        spawnTime = Time.time;
    }

    void Update()
    {
        // �ƶ��ӵ�
        transform.Translate(velocity * Time.deltaTime);

        // ����Ƿ񳬹���������
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
        spawnTime = Time.time;  // ��������ʱ��

        // �����ӵ����
        UpdateBulletVisuals();
    }

    private void UpdateBulletVisuals()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            // ���ݵȼ������ӵ���С����ɫ
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
        // ���е���
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                // ���Ż�����Ч
                //AudioManager.Instance?.PlayBulletHitSound();
            }

            // �����ӵ�
            ObjectPool.Instance.Despawn(gameObject);
        }
        //// �����ϰ���
        //else if (collision.CompareTag("Obstacle"))
        //{
        //    // �����ϰ��������Ч
        //    AudioManager.Instance?.PlayObstacleHitSound();

        //    // �����ӵ�
        //    ObjectPool.Instance.Despawn(gameObject);
        //}
    }
}
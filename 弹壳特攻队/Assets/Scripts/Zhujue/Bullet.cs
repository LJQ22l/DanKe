using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private float damage;
    private Vector2 velocity;
    private int level;

    // �ӵ����
    private SpriteRenderer spriteRenderer;
    public Sprite[] levelSprites;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // �����ӵ�����
    public void Setup(float damage, Vector2 velocity, int level)
    {
        this.damage = damage;
        this.velocity = velocity;
        this.level = level;

        // �����ӵ����
        UpdateBulletVisuals();
    }

    void Update()
    {
        // �ƶ��ӵ�
        transform.Translate(velocity * Time.deltaTime);
    }

    // �����ӵ����
    private void UpdateBulletVisuals()
    {
        if (spriteRenderer && levelSprites.Length > 0)
        {
            int spriteIndex = Mathf.Clamp(level - 1, 0, levelSprites.Length - 1);
            spriteRenderer.sprite = levelSprites[spriteIndex];

            // ���ݵȼ�������С
            transform.localScale = Vector3.one * (1 + (level - 1) * 0.2f);
        }
    }

    //��ײ���
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �����ײ������
        if (collision.CompareTag("Enemy"))
        {
            // ����˺�
            //Enemy enemy = collision.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(damage);
            //}

            // ���Ż�����Ч
            //AudioManager.Instance.PlayBulletHitSound();

            // �����ӵ�
            ObjectPool.Instance.Despawn(gameObject);
        }
        // �����ײ���ϰ���
        //else if (collision.CompareTag("Obstacle"))
        //{
        //    // ���Ż����ϰ�����Ч
        //    //AudioManager.Instance.PlayObstacleHitSound();

        //    // �����ӵ�
        //    ObjectPool.Instance.Despawn(gameObject);
        //}
    }
}
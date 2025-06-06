using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public GameObject enemyPrefab; // ����Ԥ����
    public float spawnRate = 2f; // ������������
    public float detectionRange = 10f; // �������ɷ�Χ
    public LayerMask playerLayer; // ���ͼ��

    private float nextSpawnTime = 0f; // �´�����ʱ��

    void Update()
    {
        // ����������ɵ���
        if (Time.time >= nextSpawnTime)
        {
            // �������Ƿ��ڷ�Χ��
            Collider2D[] players = Physics2D.OverlapCircleAll(transform.position, detectionRange, playerLayer);

            if (players.Length > 0)
            {
                // ���ɵ���
                SpawnEnemy();

                // �����´�����ʱ��
                nextSpawnTime = Time.time + 1f / spawnRate;
            }
        }
    }

    // ���ɵ��˵ķ���
    private void SpawnEnemy()
    {
        // ��ȡ���λ��
        Vector3 randomPosition = new Vector3(
            transform.position.x + Random.Range(-detectionRange, detectionRange),
            transform.position.y + Random.Range(-detectionRange, detectionRange),
            0f
        );

        // ʵ��������
        Instantiate(enemyPrefab, randomPosition, Quaternion.identity);
    }
}
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;         // Ӳ�Ҽ�ֵ
    public int experienceValue = 5; // ����ֵ
    public float collectSpeed = 5f; // �ռ��ٶ�
    public float collectDistance = 1f; // �Զ��ռ�����

    private Transform player;
    private bool isCollected = false;

    void Start()
    {
        // �������
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogWarning("�Ҳ�����ң�Ӳ���޷��Զ��ռ�");
        }
    }

    void Update()
    {
        if (player == null || isCollected) return;

        // ����Ƿ�ӽ����
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= collectDistance)
        {
            Collect();
        }
    }

    void Collect()
    {
        isCollected = true;

        // �ƶ������λ��
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

        // ��ӵ���ҽ�Һ;���
        PlayerStats.Instance?.AddCoins(value);
        PlayerExperience.Instance?.AddExperience(experienceValue);

        // �����ռ���Ч
        //AudioManager.Instance?.PlayCoinCollectSound();

        // ����Ӳ��
        ObjectPool.Instance.Despawn(gameObject);
    }
}
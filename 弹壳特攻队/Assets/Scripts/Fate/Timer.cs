using UnityEngine;
using UnityEngine.UI;

public class AutoTimer : MonoBehaviour
{
    [Header("��ʱ������")]
    public float totalTime = 300f; // 5���ӣ�300�룩

    private Text timerText;
    private float currentTime;
    private bool isRunning = true;
    private Color originalColor;

    void Start()
    {
        // ��ȡText���
        timerText = GetComponent<Text>();
        originalColor = timerText.color;

        // ��ʼ����ʱ��
        currentTime = 0f;
        UpdateTimerDisplay();

        // �Զ���ʼ��ʱ
        StartTimer();
    }

    void Update()
    {
        if (isRunning)
        {
            // ����ʱ��
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();

            // ���30����
            if (totalTime - currentTime <= 30f)
            {
                timerText.color = Color.red;
            }

            // ����Ƿ����
            if (currentTime >= totalTime)
            {
                TimerCompleted();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        // �������
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        // �����ı���ʾ
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void StartTimer() => isRunning = true;

    void TimerCompleted()
    {
        isRunning = false;
        timerText.text = "05:00";
        timerText.color = Color.red;
    }
}
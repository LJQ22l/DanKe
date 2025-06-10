using UnityEngine;
using UnityEngine.UI;

public class AutoTimer : MonoBehaviour
{
    [Header("计时器设置")]
    public float totalTime = 300f; // 5分钟（300秒）

    private Text timerText;
    private float currentTime;
    private bool isRunning = true;
    private Color originalColor;

    void Start()
    {
        // 获取Text组件
        timerText = GetComponent<Text>();
        originalColor = timerText.color;

        // 初始化计时器
        currentTime = 0f;
        UpdateTimerDisplay();

        // 自动开始计时
        StartTimer();
    }

    void Update()
    {
        if (isRunning)
        {
            // 更新时间
            currentTime += Time.deltaTime;
            UpdateTimerDisplay();

            // 最后30秒变红
            if (totalTime - currentTime <= 30f)
            {
                timerText.color = Color.red;
            }

            // 检查是否完成
            if (currentTime >= totalTime)
            {
                TimerCompleted();
            }
        }
    }

    void UpdateTimerDisplay()
    {
        // 计算分秒
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);

        // 更新文本显示
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